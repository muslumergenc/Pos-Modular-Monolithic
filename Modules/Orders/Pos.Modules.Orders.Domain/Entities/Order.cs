using Pos.Modules.Orders.Domain.Enums;
using Pos.Shared.Domain;

namespace Pos.Modules.Orders.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public static Order Create(Guid customerId, string customerName, string? notes = null)
        => new Order { CustomerId = customerId, CustomerName = customerName, Notes = notes };

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var item = OrderItem.Create(Id, productId, productName, quantity, unitPrice);
        Items.Add(item);
        RecalculateTotal();
        SetUpdatedAt();
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Tamamlanmış sipariş iptal edilemez.");
        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
    }

    private void RecalculateTotal() => TotalAmount = Items.Sum(i => i.TotalPrice);
}

