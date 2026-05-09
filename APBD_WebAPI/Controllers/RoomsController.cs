using Microsoft.AspNetCore.Mvc;
using APBD_WebAPI.Data;
using APBD_WebAPI.Models;

namespace APBD_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Room>> GetAllRooms()
        {
            return Ok(DataStore.Rooms);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Room> GetRoomById(int id)
        {
            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new { message = $"Sala o identyfikatorze {id} nie istnieje" });
            }

            return Ok(room);
        }
        
        [HttpGet("building/{buildingCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Room>> GetRoomsByBuilding(string buildingCode)
        {
            var rooms = DataStore.Rooms
                .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (rooms.Count == 0)
            {
                return NotFound(new { message = $"Brak sal w budynku '{buildingCode}'" });
            }

            return Ok(rooms);
        }
        
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Room>> FilterRooms(
            [FromQuery] int? minCapacity = null,
            [FromQuery] bool? hasProjector = null,
            [FromQuery] bool? activeOnly = null)
        {
            var rooms = DataStore.Rooms.AsEnumerable();
            
            if (minCapacity.HasValue && minCapacity.Value > 0)
            {
                rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
            }

            if (hasProjector.HasValue)
            {
                rooms = rooms.Where(r => r.HasProjector == hasProjector.Value);
            }

            if (activeOnly.HasValue && activeOnly.Value)
            {
                rooms = rooms.Where(r => r.IsActive);
            }

            return Ok(rooms.ToList());
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Room> CreateRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            room.Id = DataStore.GetNextRoomId();
            room.CreatedAt = DateTime.UtcNow;

            DataStore.Rooms.Add(room);

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Room> UpdateRoom(int id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRoom = DataStore.Rooms.FirstOrDefault(r => r.Id == id);

            if (existingRoom == null)
            {
                return NotFound(new { message = $"Sala o identyfikatorze {id} nie istnieje" });
            }

            existingRoom.Name = room.Name;
            existingRoom.BuildingCode = room.BuildingCode;
            existingRoom.Floor = room.Floor;
            existingRoom.Capacity = room.Capacity;
            existingRoom.HasProjector = room.HasProjector;
            existingRoom.IsActive = room.IsActive;

            return Ok(existingRoom);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult DeleteRoom(int id)
        {
            var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new { message = $"Sala o identyfikatorze {id} nie istnieje" });
            }

            var hasReservations = DataStore.Reservations.Any(res => res.RoomId == id);

            if (hasReservations)
            {
                return Conflict(new 
                { 
                    message = $"Nie można usunąć sali, ponieważ posiada powiązane rezerwacje" 
                });
            }

            DataStore.Rooms.Remove(room);

            return NoContent();
        }
    }
}