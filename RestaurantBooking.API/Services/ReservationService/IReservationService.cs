using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Enums;

namespace RestaurantBooking.API.Services.ReservationService
{
    public interface IReservationService : IBaseService<ReservationGDto>
    {
        Task<ApiResponse<ReservationGDto>> ModifyReservationStatus(string uid, string status);
        Task<ApiResponse<ReservationGDto>> CancelReservesation(string reservationCode);
        Task<ApiResponse<ReservationGDto>> ModifyReservesation(string reservationCode, ModifyReservationDto model);
        public Task<ApiResponse<ReservationGDto>> GetReservesationByCode(string reservationCode);

    }
}
