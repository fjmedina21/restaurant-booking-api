using Microsoft.EntityFrameworkCore;
using RestaurantBooking.API.Helpers;
using RestaurantBooking.API.Models.Entities;

namespace RestaurantBooking.API.Data;

public class RestaurantBookingContext(DbContextOptions<RestaurantBookingContext> options)  : DbContext(options)
{
    public DbSet<RestaurantStaff> RestaurantStaff { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique indexes for email
        modelBuilder.Entity<RestaurantStaff>()
            .HasIndex(r => r.Email).IsUnique();

        // Configuring Enum to be stored as a string in the database
        modelBuilder.Entity<Reservation>()
            .Property(r => r.Status).HasConversion<string>();

        // Foreign Key relationships
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TableId);

        //seed data
        var user = new RestaurantStaff()
         {
            StaffId = Guid.NewGuid().ToString(),
            FirstName = "Francisco",
            LastName = "Medina",
            Email = "fjmedina21@gmail.com",
            Password = Utils.HashPassword("123456")
        };

        modelBuilder.Entity<RestaurantStaff>().HasData(user);
    }
}
