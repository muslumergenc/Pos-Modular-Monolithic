using MediatR;
using Pos.Modules.Catalog.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Catalog.Application.Commands;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<Result<CategoryDto>>;
public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest<Result<CategoryDto>>;
public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

public record CreateProductCommand(string Name, string? Description, decimal Price, int Stock, Guid CategoryId) : IRequest<Result<ProductDto>>;
public record UpdateProductCommand(Guid Id, string Name, string? Description, decimal Price, int Stock, Guid CategoryId) : IRequest<Result<ProductDto>>;
public record DeleteProductCommand(Guid Id) : IRequest<Result>;

