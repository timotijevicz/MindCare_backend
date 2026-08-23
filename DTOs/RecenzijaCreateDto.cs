using System.ComponentModel.DataAnnotations;

namespace MentalHealth.Data.DTOs
{
    public class RecenzijaCreateDto
    {
        [Required]
        [StringLength(2000)]
        public string Tekst { get; set; }

        [Required]
        [Range(1, 5)]
        public int Ocena { get; set; }
    }
}
