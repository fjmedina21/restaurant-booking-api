using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.DTOs;

namespace RestaurantBooking.API.Helpers
{
    public static class JwtBearerConfiguration
    {
        public static AuthenticationBuilder AddJwtBearerConfiguration(this AuthenticationBuilder authBuilder, WebApplicationBuilder webAppBuilder)
        {
            var key = Encoding.UTF8.GetBytes(webAppBuilder.Configuration["JwtSettings:key"]!);

            return authBuilder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Ensure we always have an error and error description.
                        if (string.IsNullOrEmpty(context.Error)) context.Error = "invalid_token";
                        if (string.IsNullOrEmpty(context.ErrorDescription)) context.ErrorDescription = "No Autorizado, necesita autentincarse antes de poder continuar";

                        if (context.AuthenticateFailure is not null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.ErrorDescription = $"session expired";
                        }

                        var error = new ApiErrorResponse(StatusCode: context.Response.StatusCode, Error: context.Error, ErrorDetail: context.ErrorDescription);
                        return context.Response.WriteAsJsonAsync(error);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.ContentType = "application/json";
                        var error = new ApiErrorResponse(StatusCode: context.Response.StatusCode, Error: "Prohibido", ErrorDetail: "No posee los niveles de acceso necesario para esta acción o recurso");
                        return context.Response.WriteAsJsonAsync(error);
                    }
                };
            });
        }
    }
}
