namespace Pos.Web.Models;

public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateProductViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
}

public class CategoryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCustomerViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}

public class OrderItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderViewModel
{
    public Guid Id { get; set; }
    public Guid? TableId { get; set; }
    public int? TableNumber { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string StatusName => Status switch
    {
        0 => "Alındı", 1 => "Mutfakta", 2 => "Hazır",
        3 => "Servis Edildi", 4 => "Tamamlandı", 5 => "İptal",
        _ => "Bilinmiyor"
    };
    public string StatusBadge => Status switch
    {
        0 => "secondary", 1 => "warning", 2 => "info",
        3 => "primary", 4 => "success", 5 => "danger",
        _ => "secondary"
    };
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderViewModel
{
    public Guid CustomerId { get; set; }
    public Guid? TableId { get; set; }
    public string? Notes { get; set; }
    public List<CreateOrderItemViewModel> Items { get; set; } = [];
}

public class CreateOrderItemViewModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class PaymentViewModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public string MethodName => Method switch
    {
        0 => "Nakit",
        1 => "Kredi Kartı",
        2 => "Banka Kartı",
        3 => "Dijital Ödeme",
        _ => "Bilinmiyor"
    };
    public int Status { get; set; }
    public string StatusName => Status switch
    {
        0 => "Bekliyor",
        1 => "Tamamlandı",
        2 => "Başarısız",
        3 => "İade Edildi",
        _ => "Bilinmiyor"
    };
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProcessPaymentViewModel
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Garanti Bankası 3D Secure kart bilgisi giriş formu
/// </summary>
public class GatewayPayViewModel
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    public string Language { get; set; } = "tr";
    public string CardHolderName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryDateMonth { get; set; } = string.Empty;
    public string ExpiryDateYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// API'den dönen HTML markup modeli
/// </summary>
public class GatewayMarkupViewModel
{
    public string Markup { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}

// ── Tables ─────────────────────────────────────────────────
public class TableViewModel
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string? Label { get; set; }
    public int Capacity { get; set; }
    public int Status { get; set; }
    public string StatusName => Status switch { 0 => "Boş", 1 => "Dolu", 2 => "Rezerve", _ => "?" };
    public string StatusBadge => Status switch { 0 => "success", 1 => "danger", 2 => "warning", _ => "secondary" };
    public string StatusIcon => Status switch { 0 => "bi-check-circle-fill", 1 => "bi-x-circle-fill", 2 => "bi-clock-fill", _ => "bi-question" };
}

public class CreateTableViewModel
{
    public int Number { get; set; }
    public int Capacity { get; set; } = 4;
    public string? Label { get; set; }
}

public class UpdateTableStatusViewModel
{
    public Guid Id { get; set; }
    public int Status { get; set; }
}

