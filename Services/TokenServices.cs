using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APICrudBasica.Models;
using Microsoft.IdentityModel.Tokens;

namespace APICrudBasica
{

    public static class TokenServices
    {

        public static string GenarateToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]{
     new Claim(ClaimTypes.Name , user.Id.ToString()),
     new Claim(ClaimTypes.Role , user.Role)
 }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

        var token = tokenHandler.CreateToken(tokenDescriptor);
            return $"Bearer {tokenHandler.WriteToken(token)}";
        }
    }
}