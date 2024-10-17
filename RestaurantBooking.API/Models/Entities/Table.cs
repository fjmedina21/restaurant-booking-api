using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.API.Models.Entities
{
    public class Table
    {
        [Key, MaxLength(100)] public string TableId { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(5)] public required string Name { get; set; }
        public int Capacity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property for related Reservations
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
