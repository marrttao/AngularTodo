using TodoApp.Interfaces.DTOs.Auth;
using TodoApp.Interfaces.Entities;

namespace TodoApp.Interfaces.Services;

public interface ITokenService
{
    /// <summary>Creates a signed JWT for the given user.</summary>
    AuthResponse CreateToken(User user);
}
