using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;
using RestaurantBooking.API.Models.Pagination;

namespace RestaurantBooking.API.Services.RoleService
{
    public class RoleService(RestaurantBookingContext dbContext, IMapper mapper):IRoleService
    {
        private IQueryable<Role> LoadData()
        {
            return dbContext.Roles
                .Include(e => e.RestaurantStaff).Where(e => !e.IsDeleted)
                    .Where(e => e.IsDeleted == false)
                    .OrderByDescending(e=>e.CreatedAt)
                    .AsQueryable();
        }

        public async Task<ApiResponse<RoleGDto>> GetAllAsync(PaginationParams paginationParams)
        {
            List<Role> entities = await LoadData().AsNoTracking().ToListAsync();
            List<RoleGDto> dto = mapper.Map<List<RoleGDto>>(entities);
            List<RoleGDto> listedItems = PagedList<RoleGDto>
                .ToPagedList(source: dto, currentPage: paginationParams.CurrentPage, pageSize: paginationParams.PageSize);

            return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status200OK, data: listedItems);
        }

        public async Task<ApiResponse<RoleGDto>> GetByIdAsync(string uid)
        {
            Role? entity = await LoadData().AsNoTracking().FirstOrDefaultAsync(e => e.RoleId == uid);
            if (entity is null) return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status404NotFound);
            RoleGDto dto = mapper.Map<RoleGDto>(entity);
            return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status200OK, data: [dto]);
        }

        public async Task<ApiResponse<RoleGDto>> CreateAsync<RoleDto>(RoleDto model)
        {
            Role entity = mapper.Map<Role>(model);
            var entry = await dbContext.Roles.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            RoleGDto dto = mapper.Map<RoleGDto>(entry.Entity);
            return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status201Created, data:[dto]);
        }

        public async Task<ApiResponse<RoleGDto>> UpdateAsync<RoleDto>(string uid, RoleDto model) => throw new NotImplementedException();

        public async Task<ApiResponse<RoleGDto>> DeleteAsync(string uid)
        {
            Role? entity = await LoadData().FirstOrDefaultAsync(e => e.RoleId == uid);
            if (entity is null) return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status400BadRequest);
            if(entity.RestaurantStaff.Any())
                return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status400BadRequest, detail: "This role cannot be deleted while it has staff assigned to it");
            entity.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return new ApiResponse<RoleGDto>(statusCode: StatusCodes.Status204NoContent);
        }
    }
}
