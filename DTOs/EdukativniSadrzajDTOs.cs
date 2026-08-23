using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajEdukativniSadrzajDTO
    {
        [Required(ErrorMessage = "Naslov je obavezan")]
        public string Naslov { get; set; }

        [Required(ErrorMessage = "Opis je obavezan")]
        public string Opis { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna")]
        public string Kategorija { get; set; } // "Članak", "Video", "Meditacija", "Vežba"

        [Required(ErrorMessage = "URL je obavezan")]
        public string Url { get; set; }

        public string Autor { get; set; }
    }

    public class PrikazEdukativnogSadrzajaDTO
    {
        public int SadrzajId { get; set; }
        public string Naslov { get; set; }
        public string Opis { get; set; }
        public string Kategorija { get; set; }
        public string Url { get; set; }
        public string Autor { get; set; }
        public DateTime DatumObjave { get; set; }
    }
}