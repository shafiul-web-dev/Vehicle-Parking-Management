namespace Vehicle_Parking_Management.DTO
{
	using ParkingManagementAPI.Validations;
	using System.ComponentModel.DataAnnotations;

	public class CreateReservationDto
	{
		[Required(ErrorMessage = "ParkingSlotId is required.")]
		public int ParkingSlotId { get; set; }

		[Required(ErrorMessage = "VehicleId is required.")]
		public int VehicleId { get; set; }

		[Required(ErrorMessage = "Start Time is required.")]
		public DateTime StartTime { get; set; }

		[Required(ErrorMessage = "End Time is required.")]
		[FutureDate(ErrorMessage = "End time must be in the future.")]
		public DateTime EndTime { get; set; }
	}
}
