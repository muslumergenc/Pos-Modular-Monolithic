using MediatR;
using Pos.Modules.Identity.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Identity.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;

