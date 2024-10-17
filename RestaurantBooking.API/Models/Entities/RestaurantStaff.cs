using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantBooking.API.Models.Entities
{
    public class RestaurantStaff
    {
        [Key, MaxLength(100)] public string StaffId { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("FK_RestaurantStaff_Role"),MaxLength(100)]public string RoleId { get; set; }
        [MaxLength(100)] public required string FirstName { get; set; }
        [MaxLength(100)] public required string LastName { get; set; }
        [MaxLength(100)] public required string Email { get; set; }
        [MaxLength(1000)] public required string Password { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property to RoleDto
        public required Role Role { get; set; }
    }
}
