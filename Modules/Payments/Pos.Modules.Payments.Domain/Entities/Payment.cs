using Pos.Modules.Payments.Domain.Enums;
using Pos.Shared.Domain;

namespace Pos.Modules.Payments.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? TransactionReference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, decimal amount, PaymentMethod method, string? notes = null)
    {
        if (amount <= 0) throw new ArgumentException("Ödeme tutarı sıfırdan büyük olmalı.", nameof(amount));
        return new Payment { OrderId = orderId, Amount = amount, Method = method, Notes = notes };
    }

    public void Complete(string? transactionReference = null)
    {
        Status = PaymentStatus.Completed;
        TransactionReference = transactionReference;
        ProcessedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Fail()
    {
        Status = PaymentStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Yalnızca tamamlanmış ödemeler iade edilebilir.");
        Status = PaymentStatus.Refunded;
        SetUpdatedAt();
    }
}

