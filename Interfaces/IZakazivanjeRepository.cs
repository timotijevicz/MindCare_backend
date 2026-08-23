using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IZakazivanjeRepository
    {
        Task<ZakazivanjeSesije> KreirajZakazivanje(ZakazivanjeSesije zakazivanje);
        Task<ZakazivanjeSesije> GetZakazivanje(int zakazivanjeId);
        Task<List<ZakazivanjeSesije>> GetZakazivanjaKlijenta(string klijentId);
        Task<List<ZakazivanjeSesije>> GetZakazivanjaTerapeuta(int terapeutId);
        Task<List<ZakazivanjeSesije>> GetZakazivanjaZaDatum(DateTime datum);
        Task<ZakazivanjeSesije> AzurirajStatusZakazivanja(int zakazivanjeId, string status);
        Task<bool> ProveriDostupnostTermina(int terapeutId, DateTime datum);
        Task OtkaziZakazivanje(int zakazivanjeId);
    }
}