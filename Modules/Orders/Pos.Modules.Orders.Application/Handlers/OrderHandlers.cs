using MediatR;
using Pos.Modules.Orders.Application.Commands;
using Pos.Modules.Orders.Application.DTOs;
using Pos.Modules.Orders.Application.Interfaces;
using Pos.Modules.Orders.Application.Queries;
using Pos.Modules.Orders.Domain.Entities;
using Pos.Modules.Orders.Domain.Enums;
using Pos.Shared.Common;

namespace Pos.Modules.Orders.Application.Handlers;

public class OrderCommandHandler :
    IRequestHandler<CreateOrderCommand, Result<OrderDto>>,
    IRequestHandler<UpdateOrderStatusCommand, Result<OrderDto>>,
    IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IOrderRepository _repo;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly ITableService _tableService;

    public OrderCommandHandler(IOrderRepository repo, IProductService productService, ICustomerService customerService, ITableService tableService)
    {
        _repo = repo;
        _productService = productService;
        _customerService = customerService;
        _tableService = tableService;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand req, CancellationToken ct)
    {
        var customerName = await _customerService.GetCustomerNameAsync(req.CustomerId, ct);
        if (customerName is null) return Result<OrderDto>.Failure("Müşteri bulunamadı.");

        int? tableNumber = null;
        if (req.TableId.HasValue)
        {
            tableNumber = await _tableService.GetTableNumberAsync(req.TableId.Value, ct);
            if (tableNumber is null) return Result<OrderDto>.Failure("Masa bulunamadı.");
            await _tableService.SetTableOccupiedAsync(req.TableId.Value, ct);
        }

        var order = Order.Create(req.CustomerId, customerName, req.Notes, req.TableId, tableNumber);

        foreach (var item in req.Items)
        {
            var product = await _productService.GetProductAsync(item.ProductId, ct);
            if (product is null) return Result<OrderDto>.Failure($"Ürün bulunamadı: {item.ProductId}");
            if (product.Value.Stock < item.Quantity)
                return Result<OrderDto>.Failure($"Yetersiz stok: {product.Value.Name}");
            order.AddItem(item.ProductId, product.Value.Name, item.Quantity, product.Value.Price);
            await _productService.DecreaseStockAsync(item.ProductId, item.Quantity, ct);
        }

        await _repo.AddAsync(order, ct);
        await _repo.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(Map(order));
    }

    public async Task<Result<OrderDto>> Handle(UpdateOrderStatusCommand req, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(req.Id, ct);
        if (order is null) return Result<OrderDto>.Failure("Sipariş bulunamadı.");
        order.UpdateStatus(req.Status);
        _repo.Update(order);
        await _repo.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(Map(order));
    }

    public async Task<Result> Handle(CancelOrderCommand req, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(req.Id, ct);
        if (order is null) return Result.Failure("Sipariş bulunamadı.");
        order.Cancel();
        _repo.Update(order);
        await _repo.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static OrderDto Map(Order o) => new(
        o.Id, o.TableId, o.TableNumber, o.CustomerId, o.CustomerName, o.Status, StatusLabel(o.Status), o.TotalAmount, o.Notes,
        o.Items.Select(i => new OrderItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)),
        o.CreatedAt);

    private static string StatusLabel(OrderStatus s) => s switch
    {
        OrderStatus.Received  => "Alındı",
        OrderStatus.InKitchen => "Mutfakta",
        OrderStatus.Ready     => "Hazır",
        OrderStatus.Served    => "Servis Edildi",
        OrderStatus.Completed => "Tamamlandı",
        OrderStatus.Cancelled => "İptal",
        _ => s.ToString()
    };
}

public class OrderQueryHandler :
    IRequestHandler<GetAllOrdersQuery, Result<IEnumerable<OrderDto>>>,
    IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderRepository _repo;
    public OrderQueryHandler(IOrderRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<OrderDto>>> Handle(GetAllOrdersQuery req, CancellationToken ct)
    {
        var list = req.CustomerId.HasValue
            ? await _repo.FindAsync(o => o.CustomerId == req.CustomerId.Value, ct)
            : await _repo.GetAllAsync(ct);
        return Result<IEnumerable<OrderDto>>.Success(list.Select(o => new OrderDto(
            o.Id, o.TableId, o.TableNumber, o.CustomerId, o.CustomerName, o.Status, StatusLabel(o.Status), o.TotalAmount, o.Notes,
            o.Items.Select(i => new OrderItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)),
            o.CreatedAt)));
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery req, CancellationToken ct)
    {
        var o = await _repo.GetByIdAsync(req.Id, ct);
        if (o is null) return Result<OrderDto>.Failure("Sipariş bulunamadı.");
        return Result<OrderDto>.Success(new OrderDto(
            o.Id, o.TableId, o.TableNumber, o.CustomerId, o.CustomerName, o.Status, StatusLabel(o.Status), o.TotalAmount, o.Notes,
            o.Items.Select(i => new OrderItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice)),
            o.CreatedAt));
    }

    private static string StatusLabel(OrderStatus s) => s switch
    {
        OrderStatus.Received  => "Alındı",
        OrderStatus.InKitchen => "Mutfakta",
        OrderStatus.Ready     => "Hazır",
        OrderStatus.Served    => "Servis Edildi",
        OrderStatus.Completed => "Tamamlandı",
        OrderStatus.Cancelled => "İptal",
        _ => s.ToString()
    };
}

