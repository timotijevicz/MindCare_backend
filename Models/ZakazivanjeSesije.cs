namespace MentalHealth.Data.Models
{
    public class ZakazivanjeSesije
    {
        public int ZakazivanjeSesijeId { get; set; }
        public string KlijentId { get; set; }
        public int TerapeutId { get; set; }
        public DateTime DatumZakazane { get; set; }
        public string Status { get; set; } // "Aktivno", "Otkazano", "Završeno"
        public string Napomena { get; set; }
        public DateTime DatumKreiranja { get; set; }

        public virtual Korisnik Klijent { get; set; }
        public virtual Terapeut Terapeut { get; set; }
    }
}