using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Interfaces.DTOs.Auth;
using TodoApp.Interfaces.Entities;
using TodoApp.Interfaces.Options;
using TodoApp.Interfaces.Services;

namespace TodoApp.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    // IOptions<JwtOptions> is the "Jwt" section of appsettings.json, bound to a class.
    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthResponse CreateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpiresInMinutes);

        // The server signs the token with a secret key (HMAC-SHA256).
        // Anyone can read the payload, but nobody can change it without breaking the signature.
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var descriptor = new SecurityTokenDescriptor
        {
            // Claims = data stored inside the token. "sub" (subject) holds the user id.
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ]),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email
        };
    }
}
