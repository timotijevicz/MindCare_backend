namespace MentalHealth.Data.Models
{
    public class Terapeut
    {
        public int TerapeutId { get; set; }
        public string KorisnikId { get; set; }
        public string Zvanje { get; set; } // "Psiholog", "Savetnik", "Terapeut"
        public string Licenca { get; set; }
        public string OpisPrakse { get; set; }
        public bool Dostupan { get; set; }
        public decimal SatnicaKonzultacije { get; set; }
        public DateTime DatumRegistracije { get; set; }

        // === NOVA POLJA (dodajte ovo) ===
        public double ProsecnaOcena { get; set; }
        public int BrojRecenzija { get; set; }

        public virtual Korisnik Korisnik { get; set; }
        public virtual List<ZakazivanjeSesije> ZakazaneSesije { get; set; } = new();
    }
}