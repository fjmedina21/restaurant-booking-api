using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantBooking.API.Models.Enums;

namespace RestaurantBooking.API.Models.Entities
{
    public class Reservation
    {
        [Key, MaxLength(100)] public string ReservationId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("FK_Reservation_Customer"),MaxLength(100)] public string CustomerId { get; set; }
        [ForeignKey("FK_Reservation_Table"),MaxLength(100)]public string TableId { get; set; }
        [MaxLength(20)] public required string ReservationCode { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }
        public int NumberOfPeople { get; set; }
        [MaxLength(500)] public string? Preferences { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending; // Default to Pending
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public required Customer Customer { get; set; }
        public required Table Table { get; set; }
    }
}
