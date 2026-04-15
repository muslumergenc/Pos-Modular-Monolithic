using MediatR;
using Pos.Modules.Catalog.Application.Commands;
using Pos.Modules.Catalog.Application.DTOs;
using Pos.Modules.Catalog.Application.Interfaces;
using Pos.Modules.Catalog.Application.Queries;
using Pos.Modules.Catalog.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Catalog.Application.Handlers;

public class CategoryCommandHandler :
    IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>,
    IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>,
    IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly ICategoryRepository _repo;
    public CategoryCommandHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand req, CancellationToken ct)
    {
        var category = Category.Create(req.Name, req.Description);
        await _repo.AddAsync(category, ct);
        await _repo.SaveChangesAsync(ct);
        return Result<CategoryDto>.Success(Map(category));
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand req, CancellationToken ct)
    {
        var category = await _repo.GetByIdAsync(req.Id, ct);
        if (category is null) return Result<CategoryDto>.Failure("Kategori bulunamadı.");
        category.Update(req.Name, req.Description);
        _repo.Update(category);
        await _repo.SaveChangesAsync(ct);
        return Result<CategoryDto>.Success(Map(category));
    }

    public async Task<Result> Handle(DeleteCategoryCommand req, CancellationToken ct)
    {
        var category = await _repo.GetByIdAsync(req.Id, ct);
        if (category is null) return Result.Failure("Kategori bulunamadı.");
        _repo.Delete(category);
        await _repo.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static CategoryDto Map(Category c) => new(c.Id, c.Name, c.Description, c.IsActive, c.CreatedAt);
}

public class CategoryQueryHandler :
    IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryDto>>>,
    IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    public CategoryQueryHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery req, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);
        return Result<IEnumerable<CategoryDto>>.Success(list.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive, c.CreatedAt)));
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery req, CancellationToken ct)
    {
        var c = await _repo.GetByIdAsync(req.Id, ct);
        if (c is null) return Result<CategoryDto>.Failure("Kategori bulunamadı.");
        return Result<CategoryDto>.Success(new CategoryDto(c.Id, c.Name, c.Description, c.IsActive, c.CreatedAt));
    }
}

public class ProductCommandHandler :
    IRequestHandler<CreateProductCommand, Result<ProductDto>>,
    IRequestHandler<UpdateProductCommand, Result<ProductDto>>,
    IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;

    public ProductCommandHandler(IProductRepository productRepo, ICategoryRepository categoryRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand req, CancellationToken ct)
    {
        var category = await _categoryRepo.GetByIdAsync(req.CategoryId, ct);
        if (category is null) return Result<ProductDto>.Failure("Kategori bulunamadı.");
        var product = Product.Create(req.Name, req.Description, req.Price, req.Stock, req.CategoryId);
        await _productRepo.AddAsync(product, ct);
        await _productRepo.SaveChangesAsync(ct);
        return Result<ProductDto>.Success(MapProduct(product, category.Name));
    }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand req, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(req.Id, ct);
        if (product is null) return Result<ProductDto>.Failure("Ürün bulunamadı.");
        var category = await _categoryRepo.GetByIdAsync(req.CategoryId, ct);
        if (category is null) return Result<ProductDto>.Failure("Kategori bulunamadı.");
        product.Update(req.Name, req.Description, req.Price, req.Stock, req.CategoryId);
        _productRepo.Update(product);
        await _productRepo.SaveChangesAsync(ct);
        return Result<ProductDto>.Success(MapProduct(product, category.Name));
    }

    public async Task<Result> Handle(DeleteProductCommand req, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(req.Id, ct);
        if (product is null) return Result.Failure("Ürün bulunamadı.");
        _productRepo.Delete(product);
        await _productRepo.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static ProductDto MapProduct(Product p, string categoryName) =>
        new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive, p.CategoryId, categoryName, p.CreatedAt);
}

public class ProductQueryHandler :
    IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>>>,
    IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repo;
    public ProductQueryHandler(IProductRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery req, CancellationToken ct)
    {
        var list = req.CategoryId.HasValue
            ? await _repo.FindAsync(p => p.CategoryId == req.CategoryId.Value, ct)
            : await _repo.GetAllAsync(ct);
        return Result<IEnumerable<ProductDto>>.Success(list.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive, p.CategoryId, p.Category?.Name ?? "", p.CreatedAt)));
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery req, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(req.Id, ct);
        if (p is null) return Result<ProductDto>.Failure("Ürün bulunamadı.");
        return Result<ProductDto>.Success(new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.IsActive, p.CategoryId, p.Category?.Name ?? "", p.CreatedAt));
    }
}

