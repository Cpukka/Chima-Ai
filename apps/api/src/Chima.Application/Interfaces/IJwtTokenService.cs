using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, int ExpiresInSeconds) CreateAccessToken(User user);
    string CreateOpaqueToken();
    string HashToken(string token);
}