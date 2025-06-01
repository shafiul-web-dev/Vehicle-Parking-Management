using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_Parking_Management.Data;
using Vehicle_Parking_Management.DTO;
using Vehicle_Parking_Management.Models;

namespace ParkingManagementAPI.Controllers
{
	[Route("api/reservations")]
	[ApiController]
	public class ParkingController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public ParkingController(ApplicationDbContext context)
		{
			_context = context;
		}

		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations(
			[FromQuery] string vehicleType,
			[FromQuery] string sortBy = "startTime",
			[FromQuery] string sortDirection = "asc",
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var query = _context.Reservations
				.Include(r => r.ParkingSlot)
				.Include(r => r.Vehicle)
				.AsQueryable();

			
			if (!string.IsNullOrEmpty(vehicleType))
			{
				query = query.Where(r => r.Vehicle.Type == vehicleType);
			}

			
			query = sortBy switch
			{
				"parkingLocation" => sortDirection == "asc" ? query.OrderBy(r => r.ParkingSlot.Location) : query.OrderByDescending(r => r.ParkingSlot.Location),
				"startTime" => sortDirection == "asc" ? query.OrderBy(r => r.StartTime) : query.OrderByDescending(r => r.StartTime),
				_ => query
			};

			
			var totalRecords = await query.CountAsync();
			var reservations = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(r => new ReservationDto
				{
					LicensePlate = r.Vehicle.LicensePlate,
					ParkingLocation = r.ParkingSlot.Location,
					StartTime = r.StartTime,
					EndTime = r.EndTime
				})
				.ToListAsync();

			return Ok(new { TotalRecords = totalRecords, PageNumber = pageNumber, PageSize = pageSize, Data = reservations });
		}

		
		[HttpGet("{id}")]
		public async Task<ActionResult<ReservationDto>> GetReservationById(int id)
		{
			var reservation = await _context.Reservations
				.Include(r => r.ParkingSlot)
				.Include(r => r.Vehicle)
				.Where(r => r.Id == id)
				.Select(r => new ReservationDto
				{
					LicensePlate = r.Vehicle.LicensePlate,
					ParkingLocation = r.ParkingSlot.Location,
					StartTime = r.StartTime,
					EndTime = r.EndTime
				})
				.FirstOrDefaultAsync();

			return reservation == null ? NotFound(new { message = "Reservation not found" }) : Ok(reservation);
		}

		
		[HttpPost]
		public async Task<ActionResult<Reservation>> AddReservation(CreateReservationDto reservationDto)
		{
			var slotExists = await _context.ParkingSlots.AnyAsync(s => s.Id == reservationDto.ParkingSlotId);
			var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == reservationDto.VehicleId);

			if (!slotExists || !vehicleExists)
			{
				return BadRequest(new { message = "Invalid ParkingSlotId or VehicleId" });
			}

			
			var isSlotTaken = await _context.Reservations.AnyAsync(r =>
				r.ParkingSlotId == reservationDto.ParkingSlotId &&
				(reservationDto.StartTime < r.EndTime && reservationDto.EndTime > r.StartTime));

			if (isSlotTaken)
			{
				return BadRequest(new { message = "Parking slot is already reserved for this time." });
			}

			var reservation = new Reservation
			{
				ParkingSlotId = reservationDto.ParkingSlotId,
				VehicleId = reservationDto.VehicleId,
				StartTime = reservationDto.StartTime,
				EndTime = reservationDto.EndTime
			};

			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
		}

		
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateReservation(int id, CreateReservationDto reservationDto)
		{
			var reservation = await _context.Reservations.FindAsync(id);
			if (reservation == null)
			{
				return NotFound(new { message = "Reservation not found" });
			}

			
			if (reservation.EndTime < DateTime.Now)
			{
				return BadRequest(new { message = "Past reservations cannot be rescheduled." });
			}

			reservation.StartTime = reservationDto.StartTime;
			reservation.EndTime = reservationDto.EndTime;

			await _context.SaveChangesAsync();
			return Ok(new { message = "Reservation updated successfully" });
		}

		
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteReservation(int id)
		{
			var reservation = await _context.Reservations.FindAsync(id);
			if (reservation == null)
			{
				return NotFound(new { message = "Reservation not found" });
			}

			_context.Reservations.Remove(reservation);
			await _context.SaveChangesAsync();
			return Ok(new { message = "Reservation deleted successfully" });
		}
	}
}