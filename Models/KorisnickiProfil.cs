namespace MentalHealth.Data.Models
{
    public class KorisnickiProfil
    {
        public int ProfilId { get; set; }
        public string KorisnikId { get; set; }
        public string OpisProfila { get; set; }
        public string ProfilnaSlika { get; set; }
        public string PreferiraniSat { get; set; } // HH:mm format
        public bool PrimiMotivacionuPoruku { get; set; }
        public bool PrimiPodsetnik { get; set; }
        public string Cilj { get; set; }
        public DateTime DatumAzuriranja { get; set; }

        public virtual Korisnik Korisnik { get; set; }
    }
}