using RestaurantBooking.API.Models.DTO;

namespace RestaurantBooking.API.Services.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponse<StaffGDto>> LoginAsync(LoginDto login);
        Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto model, string token);
    }
}
