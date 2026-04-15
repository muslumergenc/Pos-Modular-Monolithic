using MediatR;
using Pos.Modules.Payments.Application.DTOs;
using Pos.Modules.Payments.Domain.Enums;
using Pos.Shared.Common;

namespace Pos.Modules.Payments.Application.Commands;

public record ProcessPaymentCommand(Guid OrderId, decimal Amount, PaymentMethod Method, string? Notes) : IRequest<Result<PaymentDto>>;
public record RefundPaymentCommand(Guid PaymentId) : IRequest<Result<PaymentDto>>;

