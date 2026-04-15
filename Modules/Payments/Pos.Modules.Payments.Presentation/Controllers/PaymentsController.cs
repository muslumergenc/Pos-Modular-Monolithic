using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.Modules.Payments.Application.Commands;
using Pos.Modules.Payments.Application.DTOs;
using Pos.Modules.Payments.Application.Queries;

namespace Pos.Modules.Payments.Presentation.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? orderId = null)
    {
        var result = await _mediator.Send(new GetAllPaymentsQuery(orderId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] ProcessPaymentDto dto)
    {
        var result = await _mediator.Send(new ProcessPaymentCommand(dto.OrderId, dto.Amount, dto.Method, dto.Notes));
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> Refund(Guid id)
    {
        var result = await _mediator.Send(new RefundPaymentCommand(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// 3D Secure ödeme başlatır. HTML form markup'ı döner; client tarayıcıda submit etmelidir.
    /// </summary>
    [HttpPost("gateway/initiate")]
    public async Task<IActionResult> InitiateGateway([FromBody] InitiateGatewayPaymentDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var result = await _mediator.Send(new InitiateGatewayPaymentCommand(
            dto.OrderId, dto.Currency, dto.Language,
            dto.CardNumber, dto.Cvv, dto.ExpiryDateYear, dto.ExpiryDateMonth,
            ip, dto.Email, dto.CardHolderName, dto.CallbackBaseUrl));

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Garanti Bankası 3D Secure callback'ini işler.
    /// Pos.Web bu endpoint'i dahili olarak çağırır; kullanıcı tarayıcısı doğrudan bağlanmaz.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("{paymentId:guid}/callback")]
    public async Task<IActionResult> GatewayCallback(
        Guid paymentId,
        [FromBody] Dictionary<string, string> parameters)
    {
        var result = await _mediator.Send(
            new ProcessGatewayCallbackCommand(paymentId, parameters));

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}

