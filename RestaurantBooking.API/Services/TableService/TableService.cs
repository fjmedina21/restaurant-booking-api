using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;
using RestaurantBooking.API.Models.Enums;
using RestaurantBooking.API.Models.Pagination;

namespace RestaurantBooking.API.Services.TableService
{
    public class TableService(RestaurantBookingContext dbContext, IMapper mapper):ITableService
    {
        private IQueryable<Table> LoadData()
        {
            return dbContext.Tables
                .Include(e => e.Reservations).Where(e=> !e.IsDeleted)
                .Where(e => !e.IsDeleted)
                .OrderBy(e=> e.Capacity)
                .AsQueryable();
        }

        public async Task<ApiResponse<TableGDto>> GetAllAsync(PaginationParams paginationParams)
        {
            List<Table> entities = await LoadData().AsNoTracking().ToListAsync();
            List<TableGDto> dto = mapper.Map<List<TableGDto>>(entities);
            List<TableGDto> listedItems = PagedList<TableGDto>
                .ToPagedList(source: dto, currentPage: paginationParams.CurrentPage, pageSize: paginationParams.PageSize);

            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status200OK, data: listedItems);
        }

        public async Task<ApiResponse<TableGDto>> GetByIdAsync(string uid)
        {
            Table? entity = await LoadData().AsNoTracking().FirstOrDefaultAsync(e => e.TableId == uid);
            if (entity is null) return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status404NotFound);
            TableGDto dto = mapper.Map<TableGDto>(entity);
            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status200OK, data: [dto]);
        }
        public async Task<ApiResponse<TableGDto>> CreateAsync <TableDto>(TableDto model)
        {
            Table entity = mapper.Map<Table>(model);
            var entry = await dbContext.Tables.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            TableGDto dto = mapper.Map<TableGDto>(entry.Entity);
            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status201Created, data:[dto]);
        }

        public async Task<ApiResponse<TableGDto>> UpdateAsync <TableDto>(string uid, TableDto model) => throw new NotImplementedException();

        public async Task<ApiResponse<TableGDto>> DeleteAsync(string uid)
        {
            Table? entity = await LoadData().FirstOrDefaultAsync(e => e.TableId == uid);
            if (entity is null) return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status400BadRequest);
            if(entity.Reservations.Any(e => e.Status == ReservationStatus.Approved || e.Status == ReservationStatus.Pending))
                return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status400BadRequest, detail: "Table cannot be deleted while is has either approved or pending reservations");
            entity.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status204NoContent);
        }
        public async Task<ApiResponse<TableGDto>> GetAvailableTablesAsync(DateTime reservationStart)
        {
            List<Table> entities = await LoadData().AsNoTracking().ToListAsync();
            List<Table> availableTables = entities
                .Where(e => e.Reservations.All(r =>
                    (r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.Rejected || r.Status == ReservationStatus.Completed) // Only allow cancelled or completed reservations
                 || r.ReservationEnd.Date != reservationStart.Date // Allow if the dates are different
                 || r.ReservationEnd <= reservationStart)) // Ensure that any reservation ends before the new reservation starts if on the same date
                .ToList();

            List<TableGDto> dto = mapper.Map<List<TableGDto>>(availableTables);


            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status200OK, data: dto);
        }
    }
}
