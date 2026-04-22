using Pos.Modules.Identity.Domain.Entities;

namespace Pos.Modules.Identity.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(AppUser user, IList<string> roles);
}

