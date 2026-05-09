using System.ComponentModel.DataAnnotations;

namespace APBD_WebAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "RoomId musi być dodatnie")]
        public int RoomId { get; set; }
        
        [Required(ErrorMessage = "Imię i nazwisko organizatora jest wymagane")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Imię i nazwisko musi mieć od 3 do 150 znaków")]
        public required string OrganizerName { get; set; }
        
        [Required(ErrorMessage = "Temat rezerwacji jest wymagany")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Temat musi mieć od 1 do 200 znaków")]
        public required string Topic { get; set; }
        
        [Required(ErrorMessage = "Data rezerwacji jest wymagana")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Godzina rozpoczęcia jest wymagana")]
        public TimeSpan StartTime { get; set; }
        
        [Required(ErrorMessage = "Godzina zakończenia jest wymagana")]
        public TimeSpan EndTime { get; set; }
        
        [Required(ErrorMessage = "Status rezerwacji jest wymagany")]
        [RegularExpression(@"^(planned|confirmed|cancelled)$", 
            ErrorMessage = "Status musi być jednym z: planned, confirmed, cancelled")]
        public required string Status { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsValid()
        {
            return EndTime > StartTime;
        }
    }
}