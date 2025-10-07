using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Games.Models;
using Games.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Games.Services;

public class TokenService : ITokenServices
{
    private byte[] key;

    public TokenService()
    {
        string keyString = File.ReadAllText("../key.txt");
        key = Encoding.ASCII.GetBytes(keyString);
    }

    public string GenerateAccessToken()
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(5),
            issuer: "Server",
            audience: "User",
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );
        string tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

}