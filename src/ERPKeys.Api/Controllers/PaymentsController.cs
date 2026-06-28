using ERPKeys.Application.Common.Security;
using ERPKeys.Application.Modules.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPKeys.Api.Controllers;

[Authorize]
[Authorize(Policy = PermissionKeys.CashBankAccess)]
[ApiController]
[Route("api/payments")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentSetupService _service;

    public PaymentsController(IPaymentSetupService service) => _service = service;

    [HttpGet("processors")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsView)]
    public async Task<IActionResult> GetProcessors(
        [FromQuery] bool activeOnly, CancellationToken ct) =>
        Ok(await _service.GetProcessorsAsync(activeOnly, ct));

    [HttpPost("processors")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public async Task<IActionResult> CreateProcessor(
        [FromBody] CreatePaymentProcessorConfigurationRequest request,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _service.CreateProcessorAsync(request, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("processors/{id:guid}")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public async Task<IActionResult> UpdateProcessor(
        Guid id,
        [FromBody] SavePaymentProcessorConfigurationRequest request,
        CancellationToken ct)
    {
        try { return Ok(await _service.UpdateProcessorAsync(id, request, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("processors/{id:guid}/activate")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public Task<IActionResult> ActivateProcessor(Guid id, CancellationToken ct) =>
        SetProcessorActive(id, true, ct);

    [HttpPost("processors/{id:guid}/deactivate")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public Task<IActionResult> DeactivateProcessor(Guid id, CancellationToken ct) =>
        SetProcessorActive(id, false, ct);

    [HttpGet("methods")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsView)]
    public async Task<IActionResult> GetMethods(
        [FromQuery] bool activeOnly,
        [FromQuery] string? usage,
        CancellationToken ct)
    {
        try { return Ok(await _service.GetMethodsAsync(activeOnly, usage, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("methods")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public async Task<IActionResult> CreateMethod(
        [FromBody] CreateMethodOfPaymentRequest request,
        CancellationToken ct)
    {
        try { return StatusCode(201, await _service.CreateMethodAsync(request, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("methods/{id:guid}")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public async Task<IActionResult> UpdateMethod(
        Guid id,
        [FromBody] SaveMethodOfPaymentRequest request,
        CancellationToken ct)
    {
        try { return Ok(await _service.UpdateMethodAsync(id, request, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("methods/{id:guid}/activate")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public Task<IActionResult> ActivateMethod(Guid id, CancellationToken ct) =>
        SetMethodActive(id, true, ct);

    [HttpPost("methods/{id:guid}/deactivate")]
    [Authorize(Policy = PermissionKeys.PaymentMethodsManage)]
    public Task<IActionResult> DeactivateMethod(Guid id, CancellationToken ct) =>
        SetMethodActive(id, false, ct);

    private async Task<IActionResult> SetProcessorActive(
        Guid id, bool active, CancellationToken ct)
    {
        try
        {
            await _service.SetProcessorActiveAsync(id, active, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private async Task<IActionResult> SetMethodActive(
        Guid id, bool active, CancellationToken ct)
    {
        try
        {
            await _service.SetMethodActiveAsync(id, active, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
