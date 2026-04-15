using Pos.Modules.Catalog.Domain.Entities;
using Pos.Shared.Abstractions;

namespace Pos.Modules.Catalog.Application.Interfaces;

public interface IProductRepository : IRepository<Product> { }
public interface ICategoryRepository : IRepository<Category> { }

