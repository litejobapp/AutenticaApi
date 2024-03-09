
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticaAPI;

public class Token : IToken
{
    private readonly IConfiguration _configuration;
    public Token(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string CreateToken(Guid guid)
    {
        List<Claim> claims = new List<Claim> {
         new Claim("Id", guid.ToString())
        };
  
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Token")));
        var isuer = _configuration.GetValue<string>("Issuer");
        var audience = _configuration.GetValue<string>("Audience");
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddSeconds(60),
               signingCredentials: creds,
               issuer: isuer,
               audience: audience
           );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
