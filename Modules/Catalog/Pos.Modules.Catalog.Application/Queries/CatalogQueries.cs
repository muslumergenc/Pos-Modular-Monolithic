using MediatR;
using Pos.Modules.Catalog.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Catalog.Application.Queries;

public record GetAllCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>;
public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;
public record GetAllProductsQuery(Guid? CategoryId = null) : IRequest<Result<IEnumerable<ProductDto>>>;
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

