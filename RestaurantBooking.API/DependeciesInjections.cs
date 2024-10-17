using RestaurantBooking.API.Middlewares;
using RestaurantBooking.API.Services.AuthService;
using RestaurantBooking.API.Services.ReservationService;
using RestaurantBooking.API.Services.RestaurantStaffService;
using RestaurantBooking.API.Services.RoleService;
using RestaurantBooking.API.Services.TableService;

namespace RestaurantBooking.API
{
    public static class DependeciesInjections
    {
        public static void AddTransient(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<GlobalErrorHandler>();
        }

        public static void AddSingleton(WebApplicationBuilder builder)
        {
        }

        public static void AddScoped(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRestaurantStaffService, RestaurantStaffService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ITableService, TableService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

        }
    }
}
