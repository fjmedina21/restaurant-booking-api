using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Helpers;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;

namespace RestaurantBooking.API.Services.AuthService
{
    public class AuthService(RestaurantBookingContext dbContext, IMapper mapper, IConfiguration configuration) : IAuthService
    {
        public async Task<ApiResponse<StaffGDto>> LoginAsync(LoginDto credentials)
        {
            RestaurantStaff? user = await dbContext.RestaurantStaff
                .Where(e => !e.IsDeleted)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Email.Equals(credentials.Email));

            if (user is null || !Utils.ComparePassword(credentials.Password, user.Password))
                return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status400BadRequest, detail: "Credenciales Incorrectas. Por favor verificar e intentar otra vez.");

            var dto = mapper.Map<StaffGDto>(user);
            string token = Utils.GenerateSessionJwtAsync(user, configuration);

            return new ApiResponse<StaffGDto>(data: [dto], detail: "usuario logueado", token: token);
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto model, string token)
        {
            var payload = Utils.DecodeJwt(token);

            RestaurantStaff? user = await dbContext.RestaurantStaff
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(e => e.StaffId.Equals(payload.UserId));

            bool currentPasswordMatch = Utils.ComparePassword(model.OldPassword, user!.Password);
            bool newPasswordMatch = Utils.ComparePassword(model.NewPassword, user.Password);

            if (!currentPasswordMatch) return new ApiResponse<object>(
                statusCode: StatusCodes.Status400BadRequest, detail: "Su contraseña actual no coincide");
            if (newPasswordMatch) return new ApiResponse<object>(
                statusCode: StatusCodes.Status400BadRequest, detail: "Coloque una contraseña diferente a la actual");

            user.Password = Utils.HashPassword(model.NewPassword);
            await dbContext.SaveChangesAsync();

            return new ApiResponse<object>(detail: "Cambio de contraseña realizado");
        }

    }
}
