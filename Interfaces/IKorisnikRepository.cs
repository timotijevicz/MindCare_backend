using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IKorisnikRepository
    {
        Task<Korisnik> GetKorisnikSaProfilom(string korisnikId);
        Task<KorisnickiProfil> GetProfil(string korisnikId);
        Task<KorisnickiProfil> AzurirajProfil(string korisnikId, KorisnickiProfil profil);
        Task<List<Terapeut>> GetSviTerapeuti();
        Task<List<Terapeut>> GetDostupniTerapeuti();
        Task<List<Terapeut>> GetTerapeutiZaKlijenta(string klijentId);
        Task<Terapeut> GetTerapeut(int terapeutId);
        Task<Terapeut> GetTerapeutPoKorisnikId(string korisnikId);
        Task<Terapeut> AzurirajTerapeuta(int terapeutId, Terapeut terapeut);
        Task<List<Korisnik>> GetSviKlijenti();
        Task<List<Korisnik>> GetKlijentiZaTerapeuta(string terapeutKorisnikId);
        Task<Korisnik> GetKlijentDetalji(string klijentId);
        Task<Korisnik> PostaviAktivnostNaloga(string korisnikId, bool aktivan);
    }
}