using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Helpers;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;
using RestaurantBooking.API.Models.Enums;
using RestaurantBooking.API.Helpers.Pagination;

namespace RestaurantBooking.API.Services.ReservationService
{
    public class ReservationService(RestaurantBookingContext dbContext, IMapper mapper, IConfiguration configuration) :IReservationService
    {
        private IQueryable<Reservation> LoadData()=> dbContext.Reservations
                .Include(e => e.Customer)
                .Include(e => e.Table)
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt).ThenByDescending(e => e.ReservationStart);


        public Task<ApiResponse<ReservationGDto>> GetAllAsync(PaginationParams paginationParams) => throw new NotImplementedException();
        public async Task<ApiResponse<ReservationGDto>> GetAllAsync(PaginationParams paginationParams, string? status)
        {
            List<Reservation> entities = await LoadData().AsNoTracking().ToListAsync();
            List<ReservationGDto> dto = mapper.Map<List<ReservationGDto>>(entities);

            if (!String.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "pending":
                        dto = dto.Where(e => e.Status == ReservationStatus.Pending.ToString()).ToList();
                        break;
                    case "ppproved":
                        dto = dto.Where(e => e.Status == ReservationStatus.Approved.ToString()).ToList();
                        break;
                    case "cancelled":
                        dto = dto.Where(e => e.Status == ReservationStatus.Cancelled.ToString()).ToList();
                        break;
                    case "rejected":
                        dto = dto.Where(e => e.Status == ReservationStatus.Rejected.ToString()).ToList();
                        break;
                    case "completed":
                        dto = dto.Where(e => e.Status == ReservationStatus.Completed.ToString()).ToList();
                        break;
                    default:
                        return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK, message: "Status not available");
                        break;
                }
            }

            List<ReservationGDto> listedItems = PagedList<ReservationGDto>
                .ToPagedList(source: dto, currentPage: paginationParams.CurrentPage, pageSize: paginationParams.PageSize);

            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK, data: listedItems);
        }
        public async Task<ApiResponse<ReservationGDto>> GetByIdAsync(string uid)
        {
            Reservation? entity = await LoadData().AsNoTracking().FirstOrDefaultAsync(e => e.ReservationId == uid);
            if (entity is null) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status404NotFound);
            ReservationGDto dto = mapper.Map<ReservationGDto>(entity);
            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK, data: [dto]);
        }
        public async Task<ApiResponse<ReservationGDto>> CreateAsync <ReservationDto>(ReservationDto model)
        {
            Reservation entity = mapper.Map<Reservation>(model);
            List<Reservation> reservations = await LoadData().AsNoTracking().ToListAsync();
            if(!await Validations.TableExist(entity.TableId, dbContext)) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest);

            // Validate that the new reservation does not conflict with existing reservations
            if (reservations.Any(e => e.TableId == entity.TableId &&
                    e.ReservationStart < entity.ReservationEnd &&
                    e.ReservationEnd > entity.ReservationStart &&
                    (e.Status != ReservationStatus.Cancelled && e.Status != ReservationStatus.Rejected && e.Status != ReservationStatus.Completed)))
            {
                return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: "Reservation conflict with existing reservations");
            }

            entity.ReservationCode = new Random().Next(100, 99999).ToString();

            //Check for if customer exist
            Customer? customer = await dbContext.Customers.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == entity.Customer.Email );

            if (customer is not null) entity.CustomerId = customer.CustomerId;

            var entry = await dbContext.Reservations.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            await Utils.SendEmailAsync(
                new EmailDto(
                    To: entity.Customer.Email,
                    Subject: $"Reservation Request Created ",
                    Body: $"Your reservation request has been created for {entity.ReservationStart.ToString(CultureInfo.CurrentCulture)}. Reservation Code: {entity.ReservationCode}"),
                configuration);

            ReservationGDto dto = mapper.Map<ReservationGDto>(entry.Entity);
            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status201Created, data:[dto]);
        }

        public async Task<ApiResponse<ReservationGDto>> UpdateAsync <ReservationDto>(string code, ReservationDto model) => throw new NotImplementedException();

        public async Task<ApiResponse<ReservationGDto>> DeleteAsync(string uid)
        {
            Reservation? entity = await LoadData().FirstOrDefaultAsync(e => e.ReservationId == uid);
            if (entity is null) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest);
            if(entity.Status == ReservationStatus.Approved || entity.Status == ReservationStatus.Pending)
                return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: "Reservation cannot be deleted while is either approved or pending");
            entity.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status204NoContent);
        }

        public async Task<ApiResponse<ReservationGDto>> ChangeReservationStatusAsync(string uid, string status)
        {
            Reservation? entity = await LoadData().FirstOrDefaultAsync(e => e.ReservationId.Equals(uid));
            if (entity is null) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest);

            if((entity.Status == ReservationStatus.Cancelled
                || entity.Status == ReservationStatus.Completed
                || entity.Status == ReservationStatus.Rejected) &&
               (status.ToLower() == ReservationStatus.Rejected.ToString().ToLower()
                || status.ToLower() == ReservationStatus.Approved.ToString().ToLower()
                ||status.ToLower() == ReservationStatus.Completed.ToString().ToLower())
               )
                return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: $"Reservation cannot be {status} because it has already been {entity.Status.ToString().ToLower()}.");

            switch (status.ToLower())
            {
                case "approved":
                    entity.Status = ReservationStatus.Approved;
                    break;
                case "rejected":
                    entity.Status = ReservationStatus.Rejected;
                    break;
                case "completed":
                    entity.Status = ReservationStatus.Completed;
                    break;
                default:
                    return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message:"Status not available");
            }

            await dbContext.SaveChangesAsync();

            //Send Email to Customer with the new status
            await Utils.SendEmailAsync(
                new EmailDto(
                To: entity.Customer.Email,
                Subject: $"Reservation {status}",
                Body: $"Your reservation status has been {status} for {entity.ReservationStart.ToString(CultureInfo.CurrentCulture)}"),
                configuration);

            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK);
        }
        public async Task<ApiResponse<ReservationGDto>> CancelReservesationAsync(string reservationCode)
        {
            Reservation? entity = await LoadData().FirstOrDefaultAsync(e => e.ReservationCode == reservationCode);
            if (entity is null) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest);

            if(entity.Status == ReservationStatus.Completed || entity.Status == ReservationStatus.Cancelled || entity.Status == ReservationStatus.Rejected)
                return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: $"Reservation cannot be cancelled because it has already been {entity.Status.ToString().ToLower()}.");

            entity.Status = ReservationStatus.Cancelled;
            await dbContext.SaveChangesAsync();

            //Send Email to Customer with the new status
            await Utils.SendEmailAsync(
                new EmailDto(
                    To: entity.Customer.Email,
                    Subject: $"Reservation {ReservationStatus.Cancelled.ToString()}",
                    Body: $"your reservation has been {ReservationStatus.Cancelled.ToString()}"),
                configuration);

            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<ReservationGDto>> GetReservesationByCodeAsync(string reservationCode)
        {
            Reservation? entity = await LoadData().FirstOrDefaultAsync(e => e.ReservationCode == reservationCode);
            if (entity is null) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status404NotFound);
            ReservationGDto dto = mapper.Map<ReservationGDto>(entity);
            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK, data: [dto]);
        }

        public async Task<ApiResponse<ReservationGDto>> ModifyReservesationAsync(string reservationCode, ModifyReservationDto model)
        {
            Reservation? dbEntity = await LoadData().FirstOrDefaultAsync(e => e.ReservationCode == reservationCode);
            if (dbEntity is null) return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest);

            if (dbEntity.Status == ReservationStatus.Approved || dbEntity.Status == ReservationStatus.Rejected || dbEntity.Status == ReservationStatus.Completed)
                return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: $"Reservation cannot be modified because it has already been {dbEntity.Status}.");

            bool isTableChanged = dbEntity.TableId != model.TableId;
            bool isTimeChanged = dbEntity.ReservationStart != model.ReservationStart || dbEntity.ReservationEnd != model.ReservationEnd;

            if (isTableChanged)
            {
                bool isNewTableAvailable = await dbContext.Tables.AsNoTracking()
                    .Where(t => t.TableId == model.TableId)
                    .Where(t => t.Reservations.All(r =>
                        r.ReservationCode == dbEntity.ReservationCode || // Exclude the current reservation from conflict checks
                        (r.Status == ReservationStatus.Cancelled
                         || r.Status == ReservationStatus.Rejected
                         || r.Status == ReservationStatus.Completed)
                     || r.ReservationEnd <= model.ReservationStart // Ensure the new reservation doesn't overlap
                     || r.ReservationStart >= model.ReservationEnd)) // Ensure no overlapping reservations
                    .AnyAsync();

                if (!isNewTableAvailable)
                    return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: "The selected table is not available for the new time.");
            }

            if (isTimeChanged)
            {
                bool isCurrentTableAvailable = await dbContext.Tables
                    .Where(t => t.TableId == model.TableId).AsNoTracking() // Check the original table
                    .Where(t => t.Reservations.All(r =>
                        r.ReservationCode == dbEntity.ReservationCode || // Exclude the current reservation from conflict checks
                        (r.Status == ReservationStatus.Cancelled
                         || r.Status == ReservationStatus.Rejected
                         || r.Status == ReservationStatus.Completed ) // Ignore cancelled or rejected reservations
                     || r.ReservationEnd <= model.ReservationStart // Ensure the new reservation doesn't overlap
                     || r.ReservationStart >= model.ReservationEnd)) // Ensure no overlapping reservations
                    .AnyAsync();

                if (!isCurrentTableAvailable)
                    return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status400BadRequest, message: "The selected time is not available for the current table.");
            }

            dbEntity.ReservationStart = model.ReservationStart;
            dbEntity.ReservationEnd = model.ReservationEnd;
            dbEntity.NumberOfPeople = model.NumberOfPeople;
            dbEntity.Preferences = model.Preferences;

            if (isTableChanged) dbEntity.TableId = model.TableId; // Update the table if it was changed
            await dbContext.SaveChangesAsync();

            //Send Email to Customer with the new status
            await Utils.SendEmailAsync(
                new EmailDto(
                    To: dbEntity.Customer.Email,
                    Subject: $"Reservation Upadted",
                    Body: $"your reservation has been updated."),
                configuration);

            return new ApiResponse<ReservationGDto>(statusCode: StatusCodes.Status200OK);
        }
    }
}
