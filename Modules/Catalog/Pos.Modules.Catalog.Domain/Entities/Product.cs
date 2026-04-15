using Pos.Shared.Domain;

namespace Pos.Modules.Catalog.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private Product() { }

    public static Product Create(string name, string? description, decimal price, int stock, Guid categoryId)
    {
        if (price < 0) throw new ArgumentException("Fiyat negatif olamaz.", nameof(price));
        if (stock < 0) throw new ArgumentException("Stok negatif olamaz.", nameof(stock));
        return new Product { Name = name, Description = description, Price = price, Stock = stock, CategoryId = categoryId };
    }

    public void Update(string name, string? description, decimal price, int stock, Guid categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        SetUpdatedAt();
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity > Stock) throw new InvalidOperationException("Yetersiz stok.");
        Stock -= quantity;
        SetUpdatedAt();
    }

    public void IncreaseStock(int quantity)
    {
        Stock += quantity;
        SetUpdatedAt();
    }
}

