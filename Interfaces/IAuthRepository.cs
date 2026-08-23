using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IAuthRepository
    {
        Task<Korisnik> Registracija(Korisnik korisnik, string lozinka);
        Task<Korisnik> Prijava(string email, string lozinka);
        Task<bool> ProveriLozinku(Korisnik korisnik, string lozinka);
        Task<IList<string>> GetUloge(Korisnik korisnik);
        Task DodajUlogu(Korisnik korisnik, string uloga);
        Task<Korisnik> GetKorisnikPoEmailu(string email);
        Task<Korisnik> GetKorisnikPoId(string korisnikId);
        Task AzurirajKorisnika(Korisnik korisnik);
        Task<bool> DeaktivirajNalog(string korisnikId);
        Task PromeniLozinku(Korisnik korisnik, string trenutnaLozinka, string novaLozinka);
    }
}