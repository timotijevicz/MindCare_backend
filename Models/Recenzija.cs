using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentalHealth.Data.Models
{
    public class Recenzija
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string KorisnikId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Tekst { get; set; }

        [Required]
        [Range(1, 5)]
        public int Ocena { get; set; }

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "NaCekanju"; // "Odobreno", "Odbijeno", "NaCekanju"

        [ForeignKey("KorisnikId")]
        public virtual Korisnik Korisnik { get; set; }
    }
}
