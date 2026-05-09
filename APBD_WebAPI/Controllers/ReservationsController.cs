using APBD_WebAPI.Data;
using APBD_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Reservation>> GetAllReservations()
        {
            return Ok(DataStore.Reservations);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Reservation> GetReservationById(int id)
        {
            var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound(new { message = $"Rezerwacja o identyfikatorze {id} nie istnieje" });
            }

            return Ok(reservation);
        }
        
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Reservation>> FilterReservations(
            [FromQuery] string? date = null,
            [FromQuery] string? status = null,
            [FromQuery] int? roomId = null)
        {
            var reservations = DataStore.Reservations.AsEnumerable();

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
            {
                reservations = reservations.Where(r => r.Date.Date == parsedDate.Date);
            }

            if (!string.IsNullOrEmpty(status))
            {
                reservations = reservations.Where(r => 
                    r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (roomId.HasValue && roomId.Value > 0)
            {
                reservations = reservations.Where(r => r.RoomId == roomId.Value);
            }

            return Ok(reservations.ToList());
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<Reservation> CreateReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!reservation.IsValid())
            {
                return BadRequest(new 
                { 
                    message = "Godzina zakończenia musi być później niż godzina rozpoczęcia" 
                });
            }

            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);

            if (room == null)
            {
                return NotFound(new 
                { 
                    message = $"Sala o identyfikatorze {reservation.RoomId} nie istnieje" 
                });
            }

            if (!room.IsActive)
            {
                return Conflict(new 
                { 
                    message = "Nie można zarezerwować nieaktywnej sali" 
                });
            }

            var conflictingReservation = DataStore.Reservations.FirstOrDefault(r =>
                r.RoomId == reservation.RoomId &&
                r.Date.Date == reservation.Date.Date &&
                !(r.EndTime <= reservation.StartTime || r.StartTime >= reservation.EndTime)
            );

            if (conflictingReservation != null)
            {
                return Conflict(new
                {
                    message = $"Rezerwacja koliduje czasowo z istniejącą rezerwacją (ID: {conflictingReservation.Id}). " +
                              $"Sala jest zajęta od {conflictingReservation.StartTime:hh\\:mm\\:ss} do {conflictingReservation.EndTime:hh\\:mm\\:ss}",
                    conflictingReservationId = conflictingReservation.Id
                });
            }

            reservation.Id = DataStore.GetNextReservationId();
            reservation.CreatedAt = DateTime.UtcNow;

            DataStore.Reservations.Add(reservation);

            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<Reservation> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!reservation.IsValid())
            {
                return BadRequest(new 
                { 
                    message = "Godzina zakończenia musi być później niż godzina rozpoczęcia" 
                });
            }

            var existingReservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

            if (existingReservation == null)
            {
                return NotFound(new { message = $"Rezerwacja o identyfikatorze {id} nie istnieje" });
            }

            var conflictingReservation = DataStore.Reservations.FirstOrDefault(r =>
                r.Id != id &&
                r.RoomId == reservation.RoomId &&
                r.Date.Date == reservation.Date.Date &&
                !(r.EndTime <= reservation.StartTime || r.StartTime >= reservation.EndTime)
            );

            if (conflictingReservation != null)
            {
                return Conflict(new
                {
                    message = $"Rezerwacja koliduje czasowo z istniejącą rezerwacją (ID: {conflictingReservation.Id}). " +
                              $"Sala jest zajęta od {conflictingReservation.StartTime:hh\\:mm\\:ss} do {conflictingReservation.EndTime:hh\\:mm\\:ss}",
                    conflictingReservationId = conflictingReservation.Id
                });
            }

            existingReservation.RoomId = reservation.RoomId;
            existingReservation.OrganizerName = reservation.OrganizerName;
            existingReservation.Topic = reservation.Topic;
            existingReservation.Date = reservation.Date;
            existingReservation.StartTime = reservation.StartTime;
            existingReservation.EndTime = reservation.EndTime;
            existingReservation.Status = reservation.Status;

            return Ok(existingReservation);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteReservation(int id)
        {
            var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound(new { message = $"Rezerwacja o identyfikatorze {id} nie istnieje" });
            }

            DataStore.Reservations.Remove(reservation);

            return NoContent();
        }
    }
}