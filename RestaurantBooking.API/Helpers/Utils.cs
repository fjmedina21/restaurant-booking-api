using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;
using BC = BCrypt.Net.BCrypt;

namespace RestaurantBooking.API.Helpers
{
    public static class Utils
    {
        //Jwt
        public static string GenerateSessionJwtAsync(RestaurantStaff entity, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["JWTSettings:key"]!);
            var validHours = int.Parse(configuration["JWTSettings:expiresIn"]!);

            List<Claim> authClaims =
            [
                new Claim("userId", entity.StaffId),
                new Claim("email", entity.Email),
                new Claim("role", entity.Role.Name)
            ];

            var secretKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(claims: authClaims, expires: DateTime.Now.AddHours(validHours), signingCredentials: credentials);
            var handledToken = new JwtSecurityTokenHandler().WriteToken(token);

            return handledToken;
        }

        public static TokenPayload DecodeJwt(string token)
        {
            string[] bearerToken = token.Split(' ');
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(bearerToken[1]);

            var to = new TokenPayload(
                UserId:jwt.Claims.First(c => c.Type == "userId").Value,
                Email: jwt.Claims.First(c => c.Type == "email").Value,
                Role: jwt.Claims.First(c => c.Type == "role").Value
                );

            return to;
        }

        public static string GenerateResetPasswordJwtAsync(string email, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["JWTSetting:ResetPasswordKey"]!);
            var validHours = int.Parse(configuration["JWTSetting:TokenValidityInHours"]!);
            var validMinutes = int.Parse(configuration["JWTSetting:ResetPasswordTokenValidityInMinutes"]!);

            List<Claim> resetPasswordClaims =
            [
                new("correoElectronico", email)
            ];

            var secretKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.Aes256CbcHmacSha512);
            var token = new JwtSecurityToken(claims: resetPasswordClaims, expires: DateTime.Now.AddMinutes(validMinutes), signingCredentials: credentials);
            var handledToken = new JwtSecurityTokenHandler().WriteToken(token);

            return handledToken;
        }

        public static string DecodeResetPasswordJwt(string token)
        {
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            string email = jwt.Claims.First(c => c.Type == "correoElectronico").Value;
            return email;
        }

        public static bool ValidateExpJwt(string token)
        {
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            int exp = int.Parse(jwt.Claims.First(c => c.Type == "exp").Value);
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return currentTime < exp;
        }

        //BCrypt
        public static string HashPassword(string password, int salt = 11)
        {
            return BC.EnhancedHashPassword(inputKey: password, workFactor: salt);
        }

        public static bool ComparePassword(string password, string hashPassword)
        {
            return BC.EnhancedVerify(text: password, hash: hashPassword);
        }
    }
}
