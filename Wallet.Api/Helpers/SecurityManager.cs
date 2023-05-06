using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wallet.Models;
using Wallet.Services;

namespace Wallet.Helpers
{
    public class SecurityManager
    {
        public static string HashPassword(IConfiguration config, string password, string salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon2.Salt = Encoding.UTF8.GetBytes(salt);
            argon2.Iterations = 10;
            argon2.MemorySize = 1024 * 1024;
            argon2.DegreeOfParallelism = 8;
            var hashBytes = argon2.GetBytes(32);
            var hashString = Convert.ToBase64String(hashBytes);

            return hashString;
        }

        public static bool VerifyPassword(IConfiguration config, string password, string salt, string hash)
        {
            string passwordHash = HashPassword(config, password, salt);

            return hash == passwordHash;
        }

        public static string GenerateJWT(IConfiguration config, User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //public static bool VerifyJWT(IConfiguration config, string token, string? salt = null)
        //{
        //    TokenValidationParameters validationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer= true,
        //        ValidIssuer = config["Jwt:Issuer"],
        //        ValidateAudience = true,
        //        ValidAudience = config["Jwt:Audience"],
        //        ValidateLifetime= true,
        //        //IssuerSigningKey = salt
        //    };

        //    var handler = new JwtSecurityTokenHandler();

        //    var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

        //    var claims = claimsPrincipal.Claims;

        //    return true;
        //}
    }
}
