namespace MentalHealth.Data.Models
{
    public class DnevnikMisli
    {
        public int BeleskaId { get; set; }
        public string KorisnikId { get; set; }
        public string Naslov { get; set; }
        public string Sadrzaj { get; set; }
        public string Kategorija { get; set; } // "Anksioznost", "Depresija", "Sreća", "Motivacija"
        public bool Deljena { get; set; }
        public string DeljenjeTerapeutId { get; set; } // Null ako nije deljena
        public DateTime DatumKreiranja { get; set; }
        public DateTime? DatumAzuriranja { get; set; }

        public virtual Korisnik Korisnik { get; set; }
    }
}