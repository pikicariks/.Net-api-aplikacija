using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using API.Entities;
using API.Intefaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens; 

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key");

        if(tokenKey.Length<64) throw new Exception("It needs to be longer");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier,user.Id.ToString()),
             new(ClaimTypes.Name,user.UserName)
        };

        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject= new ClaimsIdentity(claims),
            Expires=DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
