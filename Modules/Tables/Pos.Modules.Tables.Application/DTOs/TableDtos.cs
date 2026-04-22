using Pos.Modules.Tables.Domain.Entities;

namespace Pos.Modules.Tables.Application.DTOs;

public record TableDto(Guid Id, int Number, string? Label, int Capacity, TableStatus Status, DateTime CreatedAt);
public record CreateTableDto(int Number, int Capacity, string? Label);
public record UpdateTableDto(int Number, int Capacity, string? Label);
public record UpdateTableStatusDto(TableStatus Status);
