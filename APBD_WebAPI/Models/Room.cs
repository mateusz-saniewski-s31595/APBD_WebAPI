using System.ComponentModel.DataAnnotations;

namespace APBD_WebAPI.Models
{

    public class Room
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nazwa sali jest wymagana")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Nazwa musi mieć od 1 do 100 znaków")]
        public required string Name { get; set; }
        
        [Required(ErrorMessage = "Kod budynku jest wymagany")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Kod budynku musi mieć od 1 do 10 znaków")]
        public required string BuildingCode { get; set; }
        
        [Range(0, 10, ErrorMessage = "Piętro musi być między 0 a 10")]
        public int Floor { get; set; }
        
        [Range(1, 500, ErrorMessage = "Pojemność musi być między 1 a 500")]
        public int Capacity { get; set; }
        
        public bool HasProjector { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}