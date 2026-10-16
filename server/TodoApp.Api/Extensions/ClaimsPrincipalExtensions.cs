using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TodoApp.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Reads the current user's id from the "sub" claim of the validated JWT.
    /// Controllers call it as User.GetUserId().
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("The token doesn't contain a 'sub' claim.");

        return int.Parse(sub);
    }
}
