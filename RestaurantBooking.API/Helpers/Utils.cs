using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
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
                new Claim("firstName", entity.FirstName),
                new Claim("lastName", entity.LastName),
                new Claim("email", entity.Email),
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
                FirstName: jwt.Claims.First(c => c.Type == "firstName").Value,
                LastName: jwt.Claims.First(c => c.Type == "lastName").Value,
                Email: jwt.Claims.First(c => c.Type == "email").Value
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

        public async static Task SendEmailAsync(EmailDto request, IConfiguration configuration)
        {
            string host = configuration["MailSettings:host"]!;
            int port = int.Parse(configuration["MailSettings:port"]!);
            string user = configuration["MailSettings:auth:user"]!;
            string password = configuration["MailSettings:auth:pass"]!;

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(user));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = $"<p>{request.Body}</p>" };

            var smtp = new SmtpClient();
            await smtp.ConnectAsync(host, port);
            await smtp.AuthenticateAsync(user, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
