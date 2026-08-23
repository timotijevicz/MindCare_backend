namespace MentalHealth.Data.Models
{
    public class Poruka
    {
        public int PorukaId { get; set; }
        public string PosiljaocId { get; set; }
        public string PrimalacId { get; set; }
        public string Sadrzaj { get; set; }
        public string SadrzajSifrovane { get; set; }
        public bool Procitana { get; set; }
        public DateTime DatumSlanja { get; set; }
        public DateTime? DatumCitanja { get; set; }
        public int? SesijaId { get; set; }

        public virtual Korisnik Posiljaoc { get; set; }
        public virtual Korisnik Primalac { get; set; }
        public virtual Sesija Sesija { get; set; }
    }
}