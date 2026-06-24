using ERPKeys.Application.Common.Security;
using ERPKeys.Application.Modules.Rag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPKeys.Api.Controllers;

[ApiController]
[Route("api/rag")]
[Authorize]
[Authorize(Policy = PermissionKeys.KnowledgeAccess)]
public sealed class RagController : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".txt", ".md", ".csv", ".json", ".xml" };

    private readonly IRagService _rag;

    public RagController(IRagService rag) => _rag = rag;

    [HttpGet("documents")]
    public async Task<IActionResult> GetDocuments(CancellationToken ct) =>
        Ok(await _rag.GetDocumentsAsync(ct));

    [HttpPost("documents")]
    [Authorize(Policy = PermissionKeys.KnowledgeManage)]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument(
        IFormFile file,
        [FromForm] string? requiredPermission,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "Select a non-empty document." });

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest(new
            {
                error = "Supported document types are TXT, Markdown, CSV, JSON, and XML."
            });
        }

        try
        {
            await using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
            var content = await reader.ReadToEndAsync(ct);
            var document = await _rag.IngestAsync(
                Path.GetFileName(file.FileName),
                extension.TrimStart('.'),
                content,
                requiredPermission,
                ct);
            return Ok(document);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("documents/{documentId:guid}")]
    [Authorize(Policy = PermissionKeys.KnowledgeManage)]
    public async Task<IActionResult> DeleteDocument(Guid documentId, CancellationToken ct)
    {
        try
        {
            await _rag.DeleteDocumentAsync(documentId, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] RagQuestionRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _rag.AskAsync(request.Question, request.History, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record RagQuestionRequest(
    string Question,
    IReadOnlyList<RagConversationTurnDto>? History = null);
