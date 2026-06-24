using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ERPKeys.Application.Modules.Rag;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERPKeys.Infrastructure.Modules.Rag;

public sealed class OpenAiRagClient : IOpenAiRagClient
{
    private readonly HttpClient _http;
    private readonly ILogger<OpenAiRagClient> _logger;
    private readonly string _apiKey;
    private readonly string _embeddingModel;
    private readonly string _chatModel;
    private readonly int _embeddingDimensions;

    public OpenAiRagClient(
        HttpClient http,
        IConfiguration configuration,
        ILogger<OpenAiRagClient> logger)
    {
        _http = http;
        _logger = logger;
        _apiKey = configuration["OpenAI:ApiKey"]?.Trim() ?? string.Empty;
        _embeddingModel = configuration["OpenAI:EmbeddingModel"]?.Trim()
            ?? "text-embedding-3-small";
        _chatModel = configuration["OpenAI:ChatModel"]?.Trim() ?? "gpt-5-mini";
        _embeddingDimensions = configuration.GetValue("OpenAI:EmbeddingDimensions", 1536);
    }

    public async Task<IReadOnlyList<float[]>> CreateEmbeddingsAsync(
        IReadOnlyList<string> inputs,
        CancellationToken ct = default)
    {
        EnsureConfigured();
        if (inputs.Count == 0)
            return [];

        var results = new List<float[]>(inputs.Count);
        foreach (var batch in inputs.Chunk(64))
            results.AddRange(await CreateEmbeddingBatchAsync(batch, ct));
        return results;
    }

    private async Task<IReadOnlyList<float[]>> CreateEmbeddingBatchAsync(
        IReadOnlyList<string> inputs,
        CancellationToken ct)
    {
        using var request = CreateRequest(
            HttpMethod.Post,
            "v1/embeddings",
            new
            {
                model = _embeddingModel,
                input = inputs,
                dimensions = _embeddingDimensions,
                encoding_format = "float"
            });
        using var response = await _http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
        return document.RootElement.GetProperty("data")
            .EnumerateArray()
            .OrderBy(item => item.GetProperty("index").GetInt32())
            .Select(item => item.GetProperty("embedding")
                .EnumerateArray()
                .Select(value => value.GetSingle())
                .ToArray())
            .ToList();
    }

    public async Task<string> CreateGroundedAnswerAsync(
        string question,
        IReadOnlyList<RagSearchHit> context,
        IReadOnlyList<RagConversationTurnDto>? history = null,
        CancellationToken ct = default)
    {
        EnsureConfigured();
        var contextText = new StringBuilder();
        for (var index = 0; index < context.Count; index++)
        {
            var hit = context[index];
            contextText.AppendLine(
                $"[Source {index + 1}: {hit.DocumentName}, chunk {hit.ChunkIndex + 1}]");
            contextText.AppendLine(hit.Content);
            contextText.AppendLine();
        }

        var historyText = new StringBuilder();
        foreach (var turn in RagService.NormalizeHistory(history))
            historyText.AppendLine($"{turn.Role}: {turn.Text}");

        using var request = CreateRequest(
            HttpMethod.Post,
            "v1/responses",
            new
            {
                model = _chatModel,
                instructions =
                    "You are the ERP Keys knowledge assistant. Answer only from the supplied " +
                    "organization knowledge-base excerpts. If the answer is not supported by " +
                    "the excerpts, say that you could not find it. Cite supporting excerpts " +
                    "inline as [Source 1], [Source 2], and so on unless the user asks for a " +
                    "clean report without sources. Do not invent ERP data. " +
                    "If the user asks to reformat, summarize, or tabulate a previous answer, " +
                    "use the recent conversation plus the supplied excerpts to answer. " +
                    "When the user asks for a report or table, return a concise Markdown table " +
                    "with clear column headers, and do not include a Sources column if the user asks to hide sources.",
                input =
                    $"Recent conversation, if any:\n{historyText}\n\n" +
                    $"Current question:\n{question}\n\n" +
                    $"Knowledge-base excerpts:\n{contextText}",
                reasoning = new { effort = "low" },
                max_output_tokens = 1_800
            });
        using var response = await _http.SendAsync(request, ct);
        await EnsureSuccessAsync(response, ct);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(ct));
        if (IsIncompleteBecauseOfOutputLimit(document.RootElement))
        {
            _logger.LogWarning("OpenAI response reached max_output_tokens before producing complete text.");
        }

        var text = ExtractResponseText(document.RootElement);
        return string.IsNullOrWhiteSpace(text)
            ? "I could not generate a visible answer from the model response. Try asking again with a little more detail."
            : text.Trim();
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, object body)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = JsonContent.Create(body);
        return request;
    }

    private static string ExtractResponseText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out var outputText) &&
            outputText.ValueKind == JsonValueKind.String)
            return outputText.GetString() ?? string.Empty;

        if (!root.TryGetProperty("output", out var output))
            return string.Empty;

        var parts = new List<string>();
        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content))
                continue;
            foreach (var part in content.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var text) &&
                    text.ValueKind == JsonValueKind.String)
                    parts.Add(text.GetString() ?? string.Empty);
            }
        }
        return string.Join(Environment.NewLine, parts);
    }

    private static bool IsIncompleteBecauseOfOutputLimit(JsonElement root)
    {
        if (!root.TryGetProperty("incomplete_details", out var details) ||
            details.ValueKind != JsonValueKind.Object ||
            !details.TryGetProperty("reason", out var reason) ||
            reason.ValueKind != JsonValueKind.String)
            return false;

        return reason.GetString() == "max_output_tokens";
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("OpenAI is not configured.");
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        _logger.LogWarning(
            "OpenAI request failed with status {StatusCode}. Response: {ResponseBody}",
            (int)response.StatusCode,
            responseBody);
        throw new InvalidOperationException(
            "The AI provider is temporarily unavailable. Check the OpenAI configuration and try again.");
    }
}
