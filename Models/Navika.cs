namespace MentalHealth.Data.Models
{
    public class Navika
    {
        public int NavikaId { get; set; }
        public string KorisnikId { get; set; }
        public string NazivNavike { get; set; }
        public string Opis { get; set; }
        public string Kategorija { get; set; } // "Meditacija", "Vežbanje", "Čitanje", "Dnevnik"
        public string Ucestalost { get; set; } // "Dnevno", "Nedeljno", "Mesečno"
        public DateTime DatumPocetka { get; set; }
        public bool Aktivna { get; set; }

        public virtual Korisnik Korisnik { get; set; }
        public virtual List<PracenjeNavike> Pracenja { get; set; } = new();
    }
}