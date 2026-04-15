using Pos.Shared.Domain;

namespace Pos.Modules.Catalog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public static Category Create(string name, string? description = null)
    {
        return new Category { Name = name, Description = description };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
        SetUpdatedAt();
    }
}

