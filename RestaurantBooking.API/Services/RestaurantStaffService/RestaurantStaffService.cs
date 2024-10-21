using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Helpers;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;
using RestaurantBooking.API.Helpers.Pagination;

namespace RestaurantBooking.API.Services.RestaurantStaffService
{
    public class RestaurantStaffService(RestaurantBookingContext dbContext, IMapper mapper) : IRestaurantStaffService
    {
        private IQueryable<RestaurantStaff> LoadData()
        {
            return dbContext.RestaurantStaff
                .Where(e => e.IsDeleted == false)
                .Include(e => e.Role)
                .OrderByDescending(e=>e.CreatedAt)
                .AsQueryable();
        }

        public async Task<ApiResponse<StaffGDto>> GetAllAsync(PaginationParams paginationParams)
        {
            List<RestaurantStaff> entities = await LoadData().AsNoTracking().ToListAsync();
            List<StaffGDto> dto = mapper.Map<List<StaffGDto>>(entities);
            List<StaffGDto> listedItems = PagedList<StaffGDto>
                .ToPagedList(source: dto, currentPage: paginationParams.CurrentPage, pageSize: paginationParams.PageSize);

            return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status200OK, data: listedItems);
        }

        public async Task<ApiResponse<StaffGDto>> GetByIdAsync(string uid)
        {
            RestaurantStaff? entity = await LoadData().AsNoTracking().FirstOrDefaultAsync(e => e.StaffId == uid);
            if (entity is null) return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status404NotFound);
            StaffGDto dto = mapper.Map<StaffGDto>(entity);
            return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status200OK, data: [dto]);
        }

        public async Task<ApiResponse<StaffGDto>> CreateAsync<RegisterStaffDto>(RegisterStaffDto model)
        {
            RestaurantStaff entity = mapper.Map<RestaurantStaff>(model);
            entity.Password = Utils.HashPassword(entity.Password);
            var entry = await dbContext.RestaurantStaff.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            StaffGDto dto = mapper.Map<StaffGDto>(entry.Entity);
            return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status201Created, data:[dto]);
        }

        public async Task<ApiResponse<StaffGDto>> UpdateAsync<RegisterStaffDto>(string uid, RegisterStaffDto model) => throw new NotImplementedException();

        public async Task<ApiResponse<StaffGDto>> DeleteAsync(string uid)
        {
            RestaurantStaff? entity = await LoadData().FirstOrDefaultAsync(e => e.StaffId == uid);
            if (entity is null) return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status400BadRequest);
            entity.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return new ApiResponse<StaffGDto>(statusCode: StatusCodes.Status204NoContent);
        }
    }
}
