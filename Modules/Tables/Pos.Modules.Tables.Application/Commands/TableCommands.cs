using MediatR;
using Pos.Modules.Tables.Application.DTOs;
using Pos.Modules.Tables.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Tables.Application.Commands;

public record CreateTableCommand(int Number, int Capacity, string? Label) : IRequest<Result<TableDto>>;
public record UpdateTableCommand(Guid Id, int Number, int Capacity, string? Label) : IRequest<Result<TableDto>>;
public record UpdateTableStatusCommand(Guid Id, TableStatus Status) : IRequest<Result<TableDto>>;
public record DeleteTableCommand(Guid Id) : IRequest<Result>;
