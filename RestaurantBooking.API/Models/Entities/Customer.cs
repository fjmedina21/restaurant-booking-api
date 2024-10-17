using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RestaurantBooking.API.Models.Entities
{
    public class Customer
    {
        [Key, MaxLength(100)] public string CustomerId { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(100)] public required string FullName { get; set; }
        [MaxLength(100)] public required string Email { get; set; }
        [MaxLength(15)] public required string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;

        // Navigation property for related Reservations
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
