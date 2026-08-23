using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajRaspolozenjeDTO
    {
        [Required(ErrorMessage = "Ocena raspoloženja je obavezna")]
        [Range(1, 10, ErrorMessage = "Ocena mora biti između 1 i 10")]
        public int OcenaRaspolozenja { get; set; }

        public string Emotikon { get; set; }
        public string Opis { get; set; }
        public string UzrociStresa { get; set; }
        public string Afirmacija { get; set; }

        [Range(1, 10, ErrorMessage = "Kvalitet sna mora biti između 1 i 10")]
        public int KvalitetSna { get; set; }

        [Range(1, 10, ErrorMessage = "Kvalitet ishrane mora biti između 1 i 10")]
        public int KvalitetIshrane { get; set; }

        [Range(1, 10, ErrorMessage = "Nivo fizičke aktivnosti mora biti između 1 i 10")]
        public int NivoFizickeAktivnosti { get; set; }
    }

    public class PrikazRaspolozenjaDTO
    {
        public int RaspolozenjeId { get; set; }
        public int OcenaRaspolozenja { get; set; }
        public string Emotikon { get; set; }
        public string Opis { get; set; }
        public string UzrociStresa { get; set; }
        public string Afirmacija { get; set; }
        public int KvalitetSna { get; set; }
        public int KvalitetIshrane { get; set; }
        public int NivoFizickeAktivnosti { get; set; }
        public DateTime DatumUnosa { get; set; }
    }

    public class StatistikaRaspolozenjaDTO
    {
        public double ProsecnaOcena { get; set; }
        public int NajvisaOcena { get; set; }
        public int NajnizaOcena { get; set; }
        public int BrojUnosa { get; set; }
        public List<DnevniProsekDTO> DnevniProseci { get; set; } = new();
    }

    public class DnevniProsekDTO
    {
        public DateTime Datum { get; set; }
        public double ProsecnaOcena { get; set; }
    }
}