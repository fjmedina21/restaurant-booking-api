using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;
using RestaurantBooking.API.Models.Enums;
using RestaurantBooking.API.Helpers.Pagination;

namespace RestaurantBooking.API.Services.TableService
{
    public class TableService(RestaurantBookingContext dbContext, IMapper mapper):ITableService
    {
        private IQueryable<Table> LoadData() => dbContext.Tables
            .Include(e => e.Reservations).Where(e => !e.IsDeleted)
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt);

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
                return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status400BadRequest, message: "Table cannot be deleted while is has either approved or pending reservations");
            entity.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status204NoContent);
        }
        public async Task<ApiResponse<TableGDto>> GetAvailableTablesAsync(DateTime reservationStart, DateTime reservationEnd)
        {
            List<Table> entities = await LoadData().AsNoTracking().ToListAsync();
            List<Table> availableTables = entities
                .Where(e => e.Reservations.All(r =>
                    // Si el estado no es Pending o Approved, se considera no bloqueante
                    r.Status != ReservationStatus.Pending && r.Status != ReservationStatus.Approved ||
                    // Para reservas en Pending o Approved, validar que no haya solapamiento
                    (reservationStart < r.ReservationStart || reservationStart >= r.ReservationEnd) &&
                    (reservationEnd <= r.ReservationStart || reservationEnd > r.ReservationEnd)))
                .ToList();



            List<TableGDto> dto = mapper.Map<List<TableGDto>>(availableTables);


            return new ApiResponse<TableGDto>(statusCode: StatusCodes.Status200OK, data: dto);
        }
    }
}
