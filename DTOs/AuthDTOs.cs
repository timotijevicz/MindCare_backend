using System.ComponentModel.DataAnnotations;

namespace MentalHealth.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera")]
        public string Lozinka { get; set; }

        [Required(ErrorMessage = "Ime je obavezno")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno")]
        public string Prezime { get; set; }

        public bool AnonimnaRegistracija { get; set; }

        [Required(ErrorMessage = "Tip korisnika je obavezan")]
        public string TipKorisnika { get; set; } // "Klijent" ili "Terapeut"

        // Podaci za terapeuta (opciono)
        public string? Zvanje { get; set; }
        public string? Licenca { get; set; }
        public string? OpisPrakse { get; set; }
        public decimal? SatnicaKonzultacije { get; set; }
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        public string Lozinka { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime IstekTokena { get; set; }
        public string KorisnikId { get; set; }
        public string Email { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Rola { get; set; }
        public bool JeTerapeut { get; set; }
    }

    public class PromenaLozinkeDTO
    {
        [Required(ErrorMessage = "Trenutna lozinka je obavezna")]
        public string TrenutnaLozinka { get; set; }

        [Required(ErrorMessage = "Nova lozinka je obavezna")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera")]
        public string NovaLozinka { get; set; }
    }

    public class ResetLozinkeDTO
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }
    }

    public class PotvrdiResetLozinkeDTO
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token je obavezan")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Nova lozinka je obavezna")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera")]
        public string NovaLozinka { get; set; }
    }
}
