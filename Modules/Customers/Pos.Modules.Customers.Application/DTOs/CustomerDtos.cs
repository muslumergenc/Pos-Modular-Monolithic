namespace Pos.Modules.Customers.Application.DTOs;

public record CustomerDto(Guid Id, string FullName, string Email, string Phone, string? Address, bool IsActive, DateTime CreatedAt);
public record CreateCustomerDto(string FullName, string Email, string Phone, string? Address);
public record UpdateCustomerDto(string FullName, string Email, string Phone, string? Address);

