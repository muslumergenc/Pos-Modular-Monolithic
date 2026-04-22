using MediatR;
using Pos.Modules.Tables.Application.DTOs;
using Pos.Modules.Tables.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Tables.Application.Queries;

public record GetAllTablesQuery : IRequest<Result<IEnumerable<TableDto>>>;
public record GetTableByIdQuery(Guid Id) : IRequest<Result<TableDto>>;
