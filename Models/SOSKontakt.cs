namespace MentalHealth.Data.Models
{
    public class SOSKontakt
    {
        public int SOSKontaktId { get; set; }
        public string KorisnikId { get; set; }
        public string ImeKontakta { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Napomena { get; set; }
        public bool Aktivan { get; set; }
        public DateTime DatumKreiranja { get; set; }

        public virtual Korisnik Korisnik { get; set; }
    }
}