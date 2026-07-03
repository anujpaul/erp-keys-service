using ERPKeys.Application.Common.Security;
using ERPKeys.Application.Modules.SystemAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPKeys.Api.Controllers;

[ApiController]
[Route("api/sysadmin/integrations")]
[Authorize(Roles = "Admin")]
[Authorize(Policy = PermissionKeys.SystemSettingsManage)]
public class IntegrationConfigurationsController : ControllerBase
{
    private readonly IIntegrationConfigurationService _service;

    public IntegrationConfigurationsController(
        IIntegrationConfigurationService service) =>
        _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateIntegrationConfigurationRequest request,
        CancellationToken ct) =>
        await Execute(() => _service.CreateAsync(request, ct), created: true);

    [HttpPut("{id:guid}/configuration")]
    public async Task<IActionResult> Save(
        Guid id,
        [FromBody] SaveIntegrationConfigurationRequest request,
        CancellationToken ct) =>
        await Execute(() => _service.SaveAsync(id, request, ct));

    [HttpPost("{id:guid}/enable")]
    public async Task<IActionResult> Enable(Guid id, CancellationToken ct) =>
        await Execute(() => _service.SetEnabledAsync(id, true, ct));

    [HttpPost("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id, CancellationToken ct) =>
        await Execute(() => _service.SetEnabledAsync(id, false, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ReviewIntegrationConfigurationRequest request,
        CancellationToken ct) =>
        await Execute(() => _service.ApproveAsync(id, request.Notes, ct));

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] ReviewIntegrationConfigurationRequest request,
        CancellationToken ct) =>
        await Execute(() => _service.RejectAsync(id, request.Notes, ct));

    private async Task<IActionResult> Execute(
        Func<Task<IntegrationConfigurationDto>> operation,
        bool created = false)
    {
        try
        {
            var result = await operation();
            return created ? StatusCode(StatusCodes.Status201Created, result) : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
