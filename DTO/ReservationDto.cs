namespace Vehicle_Parking_Management.DTO
{
	public class ReservationDto
	{
		public string LicensePlate { get; set; }
		public string ParkingLocation { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
