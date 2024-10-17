using AutoMapper;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Entities;

namespace RestaurantBooking.API
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<RegisterStaffDto, RestaurantStaff>();
            CreateMap<RestaurantStaff, StaffGDto>();

            CreateMap<RoleDto, Role>();
            CreateMap<Role, RoleGDto>();

            CreateMap<TableDto, Table>();
            CreateMap<Table, TableGDto>();

            CreateMap<CustomerDto, Customer>();
            CreateMap<Customer, CustomerGDto>();

            CreateMap<ReservationDto, Reservation>();
            CreateMap<Reservation, ReservationGDto>();
        }
    }
}
