namespace MentalHealth.Data.Models
{
    public class Podsetnik
    {
        public int PodsetnikId { get; set; }
        public string KorisnikId { get; set; }
        public string Tip { get; set; } // "Raspoloženje", "Dnevnik", "Meditacija", "Disanje"
        public string Tekst { get; set; }
        public bool Aktivan { get; set; }
        public string VremePodsetnika { get; set; } // HH:mm format
        public DateTime DatumKreiranja { get; set; }

        public virtual Korisnik Korisnik { get; set; }
    }
}