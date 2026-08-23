namespace MentalHealth.Data.Models
{
    public class Cilj
    {
        public int CiljId { get; set; }
        public string KorisnikId { get; set; }
        public string NazivCilja { get; set; }
        public string Opis { get; set; }
        public string Kategorija { get; set; } // "MentalnoZdravlje", "FizičkoZdravlje", "Odnosi", "Karijera"
        public DateTime DatumPocetka { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
        public string Status { get; set; } // "Aktivan", "Završen", "Na čekanju"
        public int ProcenatNapretka { get; set; } // 0-100

        public virtual Korisnik Korisnik { get; set; }
        public virtual List<KorakCilja> Koraci { get; set; } = new();
    }
}