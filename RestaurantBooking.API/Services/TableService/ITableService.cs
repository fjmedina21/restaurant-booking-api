using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;

namespace RestaurantBooking.API.Services.TableService
{
    public interface ITableService :IBaseService<TableGDto>
    {
        Task<ApiResponse<TableGDto>> GetAvailableTablesAsync(DateTime reservationStart, DateTime reservationEnd);
    }
}
