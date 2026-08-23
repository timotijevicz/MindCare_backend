namespace MentalHealth.Data.Models
{
    public class Sesija
    {
        public int SesijaId { get; set; }
        public string KlijentId { get; set; }
        public string TerapeutId { get; set; }
        public string Tip { get; set; } // "Tekstualna", "Audio", "Video"
        public string Status { get; set; } // "Zakazana", "Aktivna", "Završena", "Otkazana"
        public DateTime DatumSesije { get; set; }
        public int TrajanjeSesijeMinuta { get; set; }
        public string BeleskeTerapeuta { get; set; }
        public string FeedbackKlijenta { get; set; }
        public DateTime DatumKreiranja { get; set; }

        public virtual Korisnik Klijent { get; set; }
        public virtual Korisnik Terapeut { get; set; }
        public virtual List<Poruka> Poruke { get; set; } = new();
    }
}