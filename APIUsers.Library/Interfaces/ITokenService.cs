using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace APIUsers.Library.Interfaces
{
    public interface ITokenService
    {
        //string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateAccessToken(IEnumerable<Claim> claims, SymmetricSecurityKey secretKey);
        string GenerateRefreshToken();
        //ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, SymmetricSecurityKey IssuerSigningKey);
    }
}
