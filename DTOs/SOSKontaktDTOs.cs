using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class KreirajSOSKontaktDTO
    {
        [Required(ErrorMessage = "Ime kontakta je obavezno")]
        public string ImeKontakta { get; set; }

        [Required(ErrorMessage = "Telefon je obavezan")]
        public string Telefon { get; set; }

        public string Email { get; set; }

        public string Napomena { get; set; }
    }

    public class PrikazSOSKontaktaDTO
    {
        public int SOSKontaktId { get; set; }
        public string ImeKontakta { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Napomena { get; set; }
        public bool Aktivan { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }

    public class AzurirajSOSKontaktDTO
    {
        public string ImeKontakta { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Napomena { get; set; }
        public bool Aktivan { get; set; }
    }
}