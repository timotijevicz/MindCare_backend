using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajPorukuDTO
    {
        [Required(ErrorMessage = "Primalac je obavezan")]
        public string PrimalacId { get; set; }

        [Required(ErrorMessage = "Sadržaj je obavezan")]
        public string Sadrzaj { get; set; }

        public int? SesijaId { get; set; }
    }

    public class PrikazPorukeDTO
    {
        public int PorukaId { get; set; }
        public string PosiljaocId { get; set; }
        public string PrimalacId { get; set; }
        public string ImePosiljaoca { get; set; }
        public string ImePrimaoca { get; set; }
        public string Sadrzaj { get; set; }
        public bool Procitana { get; set; }
        public DateTime DatumSlanja { get; set; }
        public DateTime? DatumCitanja { get; set; }
        public int? SesijaId { get; set; }
    }
}