using Pos.Modules.Identity.Application.DTOs;
using Pos.Shared.Common;
using MediatR;

namespace Pos.Modules.Identity.Application.Commands.Register;

public record RegisterCommand(string FullName, string Email, string Password, string ConfirmPassword)
    : IRequest<Result<UserDto>>;

