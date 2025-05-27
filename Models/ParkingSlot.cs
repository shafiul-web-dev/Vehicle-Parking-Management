namespace Vehicle_Parking_Management.Models
{
	public class ParkingSlot
	{
		public int Id { get; set; }
		public string Location { get; set; }
		public bool IsAvailable { get; set; }
		public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
	}
}
