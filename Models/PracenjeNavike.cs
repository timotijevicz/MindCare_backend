namespace MentalHealth.Data.Models
{
    public class PracenjeNavike
    {
        public int PracenjeId { get; set; }
        public int NavikaId { get; set; }
        public DateTime Datum { get; set; }
        public bool Zavrseno { get; set; }
        public string Komentar { get; set; }

        public virtual Navika Navika { get; set; }
    }
}