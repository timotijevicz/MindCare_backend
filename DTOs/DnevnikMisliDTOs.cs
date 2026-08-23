using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajDnevnikDTO
    {
        [Required(ErrorMessage = "Naslov je obavezan")]
        public string Naslov { get; set; }

        [Required(ErrorMessage = "Sadržaj je obavezan")]
        public string Sadrzaj { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna")]
        public string Kategorija { get; set; } // "Anksioznost", "Depresija", "Sreća", "Motivacija"

        public bool Deljena { get; set; }
        public string? DeljenjeTerapeutId { get; set; }
    }

    public class AzurirajDnevnikDTO
    {
        [Required(ErrorMessage = "Naslov je obavezan")]
        public string Naslov { get; set; }

        [Required(ErrorMessage = "Sadržaj je obavezan")]
        public string Sadrzaj { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna")]
        public string Kategorija { get; set; }

        public bool Deljena { get; set; }
        public string? DeljenjeTerapeutId { get; set; }
    }

    public class PrikazDnevnikaDTO
    {
        public int BeleskaId { get; set; }
        public string Naslov { get; set; }
        public string Sadrzaj { get; set; }
        public string Kategorija { get; set; }
        public bool Deljena { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime? DatumAzuriranja { get; set; }
        public string KorisnikIme { get; set; }
    }
}