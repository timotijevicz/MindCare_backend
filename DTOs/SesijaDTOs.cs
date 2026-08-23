using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajSesijuDTO
    {
        [Required(ErrorMessage = "Klijent je obavezan")]
        public string KlijentId { get; set; }

        [Required(ErrorMessage = "Tip sesije je obavezan")]
        public string Tip { get; set; } // "Tekstualna", "Audio", "Video"

        [Required(ErrorMessage = "Datum sesije je obavezan")]
        public DateTime DatumSesije { get; set; }

        [Required(ErrorMessage = "Trajanje sesije je obavezno")]
        public int TrajanjeSesijeMinuta { get; set; }
    }

    public class PrikazSesijeDTO
    {
        public int SesijaId { get; set; }
        public string KlijentId { get; set; }
        public string TerapeutId { get; set; }
        public string ImeKlijenta { get; set; }
        public string ImeTerapeuta { get; set; }
        public string Tip { get; set; }
        public string Status { get; set; }
        public DateTime DatumSesije { get; set; }
        public int TrajanjeSesijeMinuta { get; set; }
        public string BeleskeTerapeuta { get; set; }
        public string FeedbackKlijenta { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }

    public class AzurirajSesijuDTO
    {
        public string Status { get; set; }
        public string BeleskeTerapeuta { get; set; }
        public string FeedbackKlijenta { get; set; }
    }
}