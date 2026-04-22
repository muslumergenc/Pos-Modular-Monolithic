using MediatR;
using Pos.Modules.Tables.Application.Commands;
using Pos.Modules.Tables.Application.DTOs;
using Pos.Modules.Tables.Application.Interfaces;
using Pos.Modules.Tables.Application.Queries;
using Pos.Modules.Tables.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Tables.Application.Handlers;

public class TableCommandHandler :
    IRequestHandler<CreateTableCommand, Result<TableDto>>,
    IRequestHandler<UpdateTableCommand, Result<TableDto>>,
    IRequestHandler<UpdateTableStatusCommand, Result<TableDto>>,
    IRequestHandler<DeleteTableCommand, Result>
{
    private readonly ITableRepository _repo;
    public TableCommandHandler(ITableRepository repo) => _repo = repo;

    public async Task<Result<TableDto>> Handle(CreateTableCommand req, CancellationToken ct)
    {
        var existing = await _repo.GetByNumberAsync(req.Number, ct);
        if (existing is not null) return Result<TableDto>.Failure($"Masa {req.Number} zaten mevcut.");
        var table = RestaurantTable.Create(req.Number, req.Capacity, req.Label);
        await _repo.AddAsync(table, ct);
        await _repo.SaveChangesAsync(ct);
        return Result<TableDto>.Success(Map(table));
    }

    public async Task<Result<TableDto>> Handle(UpdateTableCommand req, CancellationToken ct)
    {
        var table = await _repo.GetByIdAsync(req.Id, ct);
        if (table is null) return Result<TableDto>.Failure("Masa bulunamadı.");
        table.Update(req.Number, req.Capacity, req.Label);
        _repo.Update(table);
        await _repo.SaveChangesAsync(ct);
        return Result<TableDto>.Success(Map(table));
    }

    public async Task<Result<TableDto>> Handle(UpdateTableStatusCommand req, CancellationToken ct)
    {
        var table = await _repo.GetByIdAsync(req.Id, ct);
        if (table is null) return Result<TableDto>.Failure("Masa bulunamadı.");
        table.SetStatus(req.Status);
        _repo.Update(table);
        await _repo.SaveChangesAsync(ct);
        return Result<TableDto>.Success(Map(table));
    }

    public async Task<Result> Handle(DeleteTableCommand req, CancellationToken ct)
    {
        var table = await _repo.GetByIdAsync(req.Id, ct);
        if (table is null) return Result.Failure("Masa bulunamadı.");
        _repo.Delete(table);
        await _repo.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static TableDto Map(RestaurantTable t) =>
        new(t.Id, t.Number, t.Label, t.Capacity, t.Status, t.CreatedAt);
}

public class TableQueryHandler :
    IRequestHandler<GetAllTablesQuery, Result<IEnumerable<TableDto>>>,
    IRequestHandler<GetTableByIdQuery, Result<TableDto>>
{
    private readonly ITableRepository _repo;
    public TableQueryHandler(ITableRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<TableDto>>> Handle(GetAllTablesQuery req, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);
        return Result<IEnumerable<TableDto>>.Success(list.Select(Map));
    }

    public async Task<Result<TableDto>> Handle(GetTableByIdQuery req, CancellationToken ct)
    {
        var t = await _repo.GetByIdAsync(req.Id, ct);
        if (t is null) return Result<TableDto>.Failure("Masa bulunamadı.");
        return Result<TableDto>.Success(Map(t));
    }

    private static TableDto Map(RestaurantTable t) =>
        new(t.Id, t.Number, t.Label, t.Capacity, t.Status, t.CreatedAt);
}
