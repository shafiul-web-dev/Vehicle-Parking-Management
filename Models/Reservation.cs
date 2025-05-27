namespace Vehicle_Parking_Management.Models
{
	public class Reservation
	{
		public int Id { get; set; }
		public int ParkingSlotId { get; set; }
		public ParkingSlot ParkingSlot { get; set; }
		public int VehicleId { get; set; }
		public Vehicle Vehicle { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
