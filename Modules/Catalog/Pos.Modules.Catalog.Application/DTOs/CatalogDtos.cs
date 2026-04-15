namespace Pos.Modules.Catalog.Application.DTOs;

public record CategoryDto(Guid Id, string Name, string? Description, bool IsActive, DateTime CreatedAt);
public record CreateCategoryDto(string Name, string? Description);
public record UpdateCategoryDto(string Name, string? Description);

public record ProductDto(Guid Id, string Name, string? Description, decimal Price, int Stock, bool IsActive, Guid CategoryId, string CategoryName, DateTime CreatedAt);
public record CreateProductDto(string Name, string? Description, decimal Price, int Stock, Guid CategoryId);
public record UpdateProductDto(string Name, string? Description, decimal Price, int Stock, Guid CategoryId);

