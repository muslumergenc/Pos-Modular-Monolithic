using MediatR;
using Microsoft.AspNetCore.Identity;
using Pos.Modules.Identity.Application.DTOs;
using Pos.Modules.Identity.Application.Interfaces;
using Pos.Modules.Identity.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<LoginResponseDto>.Failure("E-posta veya şifre hatalı.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return Result<LoginResponseDto>.Failure("E-posta veya şifre hatalı.");

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
        return Result<LoginResponseDto>.Success(new LoginResponseDto(token, user.Email!, user.FullName, user.Id, expiresAt, roles));
    }
}
