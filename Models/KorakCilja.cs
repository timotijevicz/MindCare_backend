namespace MentalHealth.Data.Models
{
    public class KorakCilja
    {
        public int KorakId { get; set; }
        public int CiljId { get; set; }
        public string OpisKoraka { get; set; }
        public bool Zavrsen { get; set; }
        public DateTime? DatumZavrsetka { get; set; }

        public virtual Cilj Cilj { get; set; }
    }
}