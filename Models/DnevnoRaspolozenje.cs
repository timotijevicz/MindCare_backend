namespace MentalHealth.Data.Models
{
    public class DnevnoRaspolozenje
    {
        public int RaspolozenjeId { get; set; }
        public string KorisnikId { get; set; }
        public int OcenaRaspolozenja { get; set; } // 1-10
        public string Emotikon { get; set; }
        public string Opis { get; set; }
        public string UzrociStresa { get; set; }
        public string Afirmacija { get; set; }
        public int KvalitetSna { get; set; } // 1-10
        public int KvalitetIshrane { get; set; } // 1-10
        public int NivoFizickeAktivnosti { get; set; } // 1-10
        public DateTime DatumUnosa { get; set; }

        public virtual Korisnik Korisnik { get; set; }
    }
}