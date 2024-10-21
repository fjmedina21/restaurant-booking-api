using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Helpers.Pagination;

namespace RestaurantBooking.API.Services
{
    public interface IBaseService<T> where T : class
    {
        Task<ApiResponse<T>> GetAllAsync(PaginationParams paginationParams);
        Task<ApiResponse<T>> GetByIdAsync(string uid);
        Task<ApiResponse<T>> CreateAsync <T2>(T2 model);
        Task<ApiResponse<T>> UpdateAsync <T2>(string uid , T2 model);
        Task<ApiResponse<T>> DeleteAsync(string uid);
    }
}
