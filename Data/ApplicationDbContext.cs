using Microsoft.EntityFrameworkCore;
using Vehicle_Parking_Management.Models;

namespace Vehicle_Parking_Management.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<Vehicle> Vehicles { get; set; }
		public DbSet<ParkingSlot> ParkingSlots { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
	}
}
