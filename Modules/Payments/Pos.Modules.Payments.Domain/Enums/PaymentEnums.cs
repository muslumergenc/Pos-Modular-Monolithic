namespace Pos.Modules.Payments.Domain.Enums;

public enum PaymentMethod
{
    Cash = 0,
    CreditCard = 1,
    DebitCard = 2,
    Digital = 3
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}

