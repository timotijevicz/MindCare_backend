using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajNavikuDTO
    {
        [Required(ErrorMessage = "Naziv navike je obavezan")]
        public string NazivNavike { get; set; }

        public string Opis { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna")]
        public string Kategorija { get; set; } // "Meditacija", "Vežbanje", "Čitanje", "Dnevnik"

        [Required(ErrorMessage = "Učestalost je obavezna")]
        public string Ucestalost { get; set; } // "Dnevno", "Nedeljno", "Mesečno"
    }

    public class PrikazNavikeDTO
    {
        public int NavikaId { get; set; }
        public string NazivNavike { get; set; }
        public string Opis { get; set; }
        public string Kategorija { get; set; }
        public string Ucestalost { get; set; }
        public DateTime DatumPocetka { get; set; }
        public bool Aktivna { get; set; }
    }

    public class KreirajPracenjeNavikeDTO
    {
        [Required(ErrorMessage = "Datum je obavezan")]
        public DateTime Datum { get; set; }

        public bool Zavrseno { get; set; } = true;
        public string Komentar { get; set; }
    }

    public class PrikazPracenjaNavikeDTO
    {
        public int PracenjeId { get; set; }
        public int NavikaId { get; set; }
        public DateTime Datum { get; set; }
        public bool Zavrseno { get; set; }
        public string Komentar { get; set; }
    }
}