using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestaurantBooking.API.Models.ApiResponse;

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
                        if (string.IsNullOrEmpty(context.Error)) context.Error = StatusCodes.Status401Unauthorized.ToString();
                        if (string.IsNullOrEmpty(context.ErrorDescription)) context.ErrorDescription = "please login to access this resource";

                        if (context.AuthenticateFailure is not null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.ErrorDescription = $"expired session, please login again.";
                        }

                        var error = new ApiErrorResponse(
                            statusCode: context.Response.StatusCode,
                            errormessage: context.ErrorDescription);
                        return context.Response.WriteAsJsonAsync(error);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.ContentType = "application/json";
                        var error = new ApiErrorResponse(
                            statusCode: context.Response.StatusCode,
                            errormessage: "You are not authorized to access this resource");
                        return context.Response.WriteAsJsonAsync(error);
                    }
                };
            });
        }
    }
}
