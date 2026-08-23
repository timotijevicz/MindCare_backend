using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajCiljDTO
    {
        [Required(ErrorMessage = "Naziv cilja je obavezan")]
        public string NazivCilja { get; set; }

        public string Opis { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna")]
        public string Kategorija { get; set; } // "MentalnoZdravlje", "FizičkoZdravlje", "Odnosi", "Karijera"

        public DateTime? DatumZavrsetka { get; set; }
    }

    public class PrikazCiljaDTO
    {
        public int CiljId { get; set; }
        public string NazivCilja { get; set; }
        public string Opis { get; set; }
        public string Kategorija { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
        public string Status { get; set; }
        public int ProcenatNapretka { get; set; }
        public List<PrikazKorakaCiljaDTO> Koraci { get; set; } = new();
    }

    public class KreirajKorakCiljaDTO
    {
        [Required(ErrorMessage = "Opis koraka je obavezan")]
        public string OpisKoraka { get; set; }
    }

    public class PrikazKorakaCiljaDTO
    {
        public int KorakId { get; set; }
        public int CiljId { get; set; }
        public string OpisKoraka { get; set; }
        public bool Zavrsen { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
    }
}