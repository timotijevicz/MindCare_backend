namespace MentalHealth.Data.Models
{
    public class EdukativniSadrzaj
    {
        public int SadrzajId { get; set; }
        public string Naslov { get; set; }
        public string Opis { get; set; }
        public string Kategorija { get; set; } // "Članak", "Video", "Meditacija", "Vežba"
        public string Url { get; set; }
        public string Autor { get; set; }
        public DateTime DatumObjave { get; set; }
    }
}