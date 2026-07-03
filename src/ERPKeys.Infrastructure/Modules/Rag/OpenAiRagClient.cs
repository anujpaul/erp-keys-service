using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ERPKeys.Application.Modules.Rag;
using ERPKeys.Application.Modules.SystemAdmin.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERPKeys.Infrastructure.Modules.Rag;

public sealed class OpenAiRagClient : IOpenAiRagClient
{
    private readonly HttpClient _http;
    private readonly ILogger<OpenAiRagClient> _logger;
    private readonly IIntegrationConfigurationReader _integrations;
    private readonly OpenAiConfiguration _fallback;

    public OpenAiRagClient(
        HttpClient http,
        IConfiguration configuration,
        IIntegrationConfigurationReader integrations,
        ILogger<OpenAiRagClient> logger)
    {
        _http = http;
        _logger = logger;
        _integrations = integrations;
        _fallback = new OpenAiConfiguration(
            configuration["OpenAI:ApiKey"]?.Trim() ?? string.Empty,
            configuration["OpenAI:EmbeddingModel"]?.Trim() ?? "text-embedding-3-small",
            configuration["OpenAI:ChatModel"]?.Trim() ?? "gpt-5-mini",
            configuration.GetValue("OpenAI:EmbeddingDimensions", 1536),
            configuration["OpenAI:BaseUrl"]?.Trim() ?? "https://api.openai.com/");
    }

    public async Task<IReadOnlyList<float[]>> CreateEmbeddingsAsync(
        IReadOnlyList<string> inputs,
        CancellationToken ct = default)
    {
        var configuration = await GetConfigurationAsync(ct);
        if (inputs.Count == 0)
            return [];

        var results = new List<float[]>(inputs.Count);
        foreach (var batch in inputs.Chunk(64))
            results.AddRange(await CreateEmbeddingBatchAsync(batch, configuration, ct));
        return results;
    }

    private async Task<IReadOnlyList<float[]>> CreateEmbeddingBatchAsync(
        IReadOnlyList<string> inputs,
        OpenAiConfiguration configuration,
        CancellationToken ct)
    {
        using var request = CreateRequest(
            HttpMethod.Post,
            new Uri(new Uri(configuration.BaseUrl), "v1/embeddings"),
            new
            {
                model = configuration.EmbeddingModel,
                input = inputs,
                dimensions = configuration.EmbeddingDimensions,
                encoding_format = "float"
            },
            configuration.ApiKey);
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
        var configuration = await GetConfigurationAsync(ct);
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
            new Uri(new Uri(configuration.BaseUrl), "v1/responses"),
            new
            {
                model = configuration.ChatModel,
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
            },
            configuration.ApiKey);
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

    private static HttpRequestMessage CreateRequest(
        HttpMethod method,
        Uri uri,
        object body,
        string apiKey)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
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

    private async Task<OpenAiConfiguration> GetConfigurationAsync(CancellationToken ct)
    {
        var configured = await _integrations.GetActiveAsync(
            "LLM", "OpenAICompatible", ct);
        var apiKey = configured?.Secrets.GetValueOrDefault("ApiKey") ?? _fallback.ApiKey;
        var baseUrl = configured?.Settings.GetValueOrDefault("BaseUrl") ?? _fallback.BaseUrl;
        var embeddingModel = configured?.Settings.GetValueOrDefault("EmbeddingModel")
            ?? _fallback.EmbeddingModel;
        var chatModel = configured?.Settings.GetValueOrDefault("ChatModel")
            ?? _fallback.ChatModel;
        var dimensionsText = configured?.Settings.GetValueOrDefault("EmbeddingDimensions");
        var dimensions = int.TryParse(dimensionsText, out var parsedDimensions)
            ? parsedDimensions
            : _fallback.EmbeddingDimensions;

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI is not configured.");
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var parsed) ||
            parsed.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("OpenAI base URL must use HTTPS.");

        return new OpenAiConfiguration(
            apiKey, embeddingModel, chatModel, dimensions, baseUrl.TrimEnd('/') + "/");
    }

    private sealed record OpenAiConfiguration(
        string ApiKey,
        string EmbeddingModel,
        string ChatModel,
        int EmbeddingDimensions,
        string BaseUrl);

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
