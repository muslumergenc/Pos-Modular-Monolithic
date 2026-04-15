using MediatR;
using Microsoft.AspNetCore.Identity;
using Pos.Modules.Identity.Application.DTOs;
using Pos.Modules.Identity.Domain.Entities;
using Pos.Shared.Common;

namespace Pos.Modules.Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result<UserDto>.Failure("Bu e-posta adresi zaten kayıtlı.");

        var user = new AppUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<UserDto>.Failure(errors);
        }

        return Result<UserDto>.Success(new UserDto(user.Id, user.FullName, user.Email!, user.CreatedAt));
    }
}

