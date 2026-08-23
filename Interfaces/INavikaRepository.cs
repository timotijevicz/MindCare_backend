using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface INavikaRepository
    {
        Task<Navika> KreirajNaviku(Navika navika);
        Task<Navika> GetNavika(int navikaId);
        Task<List<Navika>> GetNavikeKorisnika(string korisnikId);
        Task<List<Navika>> GetAktivneNavike(string korisnikId);
        Task<Navika> AzurirajNaviku(Navika navika);
        Task ObrisiNaviku(int navikaId);
        Task AktivirajNaviku(int navikaId);
        Task DeaktivirajNaviku(int navikaId);
        Task<PracenjeNavike> ZabeleziPracenje(PracenjeNavike pracenje);
        Task<List<PracenjeNavike>> GetPracenjaNavike(int navikaId);
        Task<List<PracenjeNavike>> GetPracenjaZaPeriod(int navikaId, DateTime od, DateTime dok);
        Task<double> GetProcenatIspunjavanja(int navikaId, int brojDana);
    }
}