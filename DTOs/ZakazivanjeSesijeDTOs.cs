using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajZakazivanjeDTO
    {
        [Required(ErrorMessage = "Terapeut je obavezan")]
        public int TerapeutId { get; set; }

        [Required(ErrorMessage = "Datum zakazivanja je obavezan")]
        public DateTime DatumZakazane { get; set; }

        public string Napomena { get; set; }
    }

    public class PrikazZakazivanjaDTO
    {
        public int ZakazivanjeSesijeId { get; set; }
        public string KlijentId { get; set; }
        public string ImeKlijenta { get; set; }
        public int TerapeutId { get; set; }
        public string ImeTerapeuta { get; set; }
        public DateTime DatumZakazane { get; set; }
        public string Status { get; set; }
        public string Napomena { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }
}