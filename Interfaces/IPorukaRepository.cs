using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IPorukaRepository
    {
        Task<Poruka> PosaljiPoruku(Poruka poruka);
        Task<Poruka> GetPoruka(int porukaId);
        Task<List<Poruka>> GetPrimljenePoruke(string korisnikId);
        Task<List<Poruka>> GetPoslatePoruke(string korisnikId);
        Task<List<Poruka>> GetKonverzacija(string korisnik1Id, string korisnik2Id);
        Task<List<Poruka>> GetPorukeSesije(int sesijaId);
        Task<List<Poruka>> GetNeprocitanePoruke(string korisnikId);
        Task<int> GetBrojNeprocitanihPoruka(string korisnikId);
        Task OznaciKaoProcitanu(int porukaId);
        Task OznaciSveKaoProcitane(string korisnikId);
        Task ObrisiPoruku(int porukaId);
        Task<string> SifrujPoruku(string sadrzaj);
        Task<string> DesifrujPoruku(string sifrovaniSadrzaj);
    }
}