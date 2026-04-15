namespace Pos.Modules.Identity.Application.DTOs;

public record RegisterDto(string FullName, string Email, string Password, string ConfirmPassword);
public record LoginDto(string Email, string Password);
public record LoginResponseDto(string Token, string Email, string FullName, Guid UserId, DateTime ExpiresAt);
public record UserDto(Guid Id, string FullName, string Email, DateTime CreatedAt);

