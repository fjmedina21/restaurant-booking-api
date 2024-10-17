using System.ComponentModel.DataAnnotations;

namespace RestaurantBooking.API.Models.Entities
{
    public class Role
    {
        [Key, MaxLength(100)] public string RoleId { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(50)] public required string Name { get; set; }
        [MaxLength(500)] public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property for related RestaurantStaffService members
        public ICollection<RestaurantStaff> RestaurantStaff { get; set; } = [];
    }
}
