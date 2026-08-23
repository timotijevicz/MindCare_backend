using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IPodsetnikRepository
    {
        Task<Podsetnik> KreirajPodsetnik(Podsetnik podsetnik);
        Task<Podsetnik> GetPodsetnik(int podsetnikId);
        Task<List<Podsetnik>> GetPodsetniciKorisnika(string korisnikId);
        Task<List<Podsetnik>> GetAktivniPodsetnici(string korisnikId);
        Task<List<Podsetnik>> GetPodsetniciPoTipu(string korisnikId, string tip);
        Task<Podsetnik> AzurirajPodsetnik(Podsetnik podsetnik);
        Task AktivirajPodsetnik(int podsetnikId);
        Task DeaktivirajPodsetnik(int podsetnikId);
        Task ObrisiPodsetnik(int podsetnikId);
        Task<List<Podsetnik>> GetPodsetniciZaSlanje(string trenutnoVreme);
        Task<string> GenerisiMotivacionuPoruku();
    }
}