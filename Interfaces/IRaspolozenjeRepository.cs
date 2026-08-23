using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IRaspolozenjeRepository
    {
        Task<DnevnoRaspolozenje> KreirajRaspolozenje(DnevnoRaspolozenje raspolozenje);
        Task<DnevnoRaspolozenje> GetRaspolozenje(int raspolozenjeId);
        Task<List<DnevnoRaspolozenje>> GetRaspolozenjaKorisnika(string korisnikId);
        Task<List<DnevnoRaspolozenje>> GetRaspolozenjaPeriod(string korisnikId, DateTime od, DateTime dok);
        Task<DnevnoRaspolozenje> AzurirajRaspolozenje(DnevnoRaspolozenje raspolozenje);
        Task ObrisiRaspolozenje(int raspolozenjeId);
        Task<double> GetProsecnaOcena(string korisnikId);
        Task<int> GetBrojUnosa(string korisnikId);
        Task<DnevnoRaspolozenje> GetDanasnjeRaspolozenje(string korisnikId);
        Task<Dictionary<string, double>> GetStatistikaPoDanima(string korisnikId, int brojDana);
        Task<Dictionary<DayOfWeek, double>> GetProsekPoDanuUNedelji(string korisnikId, int brojDana);
        Task<List<DnevnoRaspolozenje>> GetRaspolozenjaKlijentaZaTerapeuta(string terapeutId, string klijentId);
    }
}