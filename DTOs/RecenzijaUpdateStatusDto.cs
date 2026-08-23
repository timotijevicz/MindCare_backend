using System.ComponentModel.DataAnnotations;

namespace MentalHealth.Data.DTOs
{
    public class RecenzijaUpdateStatusDto
    {
        [Required]
        public string Status { get; set; } // "Odobreno" ili "Odbijeno"
    }
}
