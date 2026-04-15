using Pos.Modules.Payments.Domain.Enums;

namespace Pos.Modules.Payments.Application.DTOs;

public record PaymentDto(
    Guid Id, Guid OrderId, decimal Amount, PaymentMethod Method,
    PaymentStatus Status, string? TransactionReference, string? Notes,
    DateTime? ProcessedAt, DateTime CreatedAt);

public record ProcessPaymentDto(Guid OrderId, decimal Amount, PaymentMethod Method, string? Notes);
public record RefundPaymentDto(string? Reason);

