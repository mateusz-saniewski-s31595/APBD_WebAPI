using APBD_WebAPI.Models;

namespace APBD_WebAPI.Data
{
    public static class DataStore
    {
        public static List<Room> Rooms { get; } = new();
        
        public static List<Reservation> Reservations { get; } = new();
        
        private static bool _initialized = false;
        
        public static void Initialize()
        {
            if (_initialized)
                return;

            Rooms.AddRange(new List<Room>
            {
                new Room
                {
                    Id = 1,
                    Name = "Sala konferencyjna A",
                    BuildingCode = "A",
                    Floor = 1,
                    Capacity = 30,
                    HasProjector = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new Room
                {
                    Id = 2,
                    Name = "Lab 204",
                    BuildingCode = "B",
                    Floor = 2,
                    Capacity = 24,
                    HasProjector = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                },
                new Room
                {
                    Id = 3,
                    Name = "Pracownia grafiki",
                    BuildingCode = "B",
                    Floor = 3,
                    Capacity = 18,
                    HasProjector = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new Room
                {
                    Id = 4,
                    Name = "Sala seminaryjna",
                    BuildingCode = "C",
                    Floor = 1,
                    Capacity = 12,
                    HasProjector = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new Room
                {
                    Id = 5,
                    Name = "Sala zabytkowa",
                    BuildingCode = "A",
                    Floor = 2,
                    Capacity = 40,
                    HasProjector = false,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            });

            var today = DateTime.UtcNow.Date;
            Reservations.AddRange(new List<Reservation>
            {
                new Reservation
                {
                    Id = 1,
                    RoomId = 1,
                    OrganizerName = "Anna Kowalska",
                    Topic = "Warsztaty z HTTP i REST",
                    Date = today.AddDays(1),
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(12, 30, 0),
                    Status = "confirmed",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Reservation
                {
                    Id = 2,
                    RoomId = 1,
                    OrganizerName = "Piotr Nowak",
                    Topic = "Architektura aplikacji webowych",
                    Date = today.AddDays(2),
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    Status = "confirmed",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new Reservation
                {
                    Id = 3,
                    RoomId = 2,
                    OrganizerName = "Maria Lewandowska",
                    Topic = "Bazy danych SQL",
                    Date = today.AddDays(3),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(11, 30, 0),
                    Status = "planned",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Reservation
                {
                    Id = 4,
                    RoomId = 3,
                    OrganizerName = "Tomasz Witkowski",
                    Topic = "Zaawansowana grafika komputerowa",
                    Date = today.AddDays(5),
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(15, 0, 0),
                    Status = "confirmed",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Reservation
                {
                    Id = 5,
                    RoomId = 4,
                    OrganizerName = "Katarzyna Zając",
                    Topic = "Seminarium z zarządzania projektami",
                    Date = today.AddDays(7),
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Status = "confirmed",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Reservation
                {
                    Id = 6,
                    RoomId = 2,
                    OrganizerName = "Krzysztof Duda",
                    Topic = "Testing i Quality Assurance",
                    Date = today.AddDays(4),
                    StartTime = new TimeSpan(11, 0, 0),
                    EndTime = new TimeSpan(13, 0, 0),
                    Status = "planned",
                    CreatedAt = DateTime.UtcNow
                }
            });

            _initialized = true;
        }
        
        public static int GetNextRoomId()
        {
            return Rooms.Count == 0 ? 1 : Rooms.Max(r => r.Id) + 1;
        }


        public static int GetNextReservationId()
        {
            return Reservations.Count == 0 ? 1 : Reservations.Max(r => r.Id) + 1;
        }
    }
}