using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IDnevnikMisliRepository
    {
        Task<DnevnikMisli> KreirajBelisku(DnevnikMisli beleska);
        Task<DnevnikMisli> GetBeleska(int beleskaId);
        Task<List<DnevnikMisli>> GetBeleskeKorisnika(string korisnikId);
        Task<List<DnevnikMisli>> GetBeleskePoKategoriji(string korisnikId, string kategorija);
        Task<List<DnevnikMisli>> PretraziBeleske(string korisnikId, string pojam);
        Task<List<DnevnikMisli>> GetDeljeneBeleske(string terapeutId);
        Task<DnevnikMisli> AzurirajBelisku(DnevnikMisli beleska);
        Task ObrisiBelisku(int beleskaId);
        Task PodeliBeliskuSaTerapeutom(int beleskaId, string terapeutId);
        Task UkiniDeljenjeBeliske(int beleskaId);
    }
}