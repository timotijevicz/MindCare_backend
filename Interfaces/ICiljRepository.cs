using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface ICiljRepository
    {
        Task<Cilj> KreirajCilj(Cilj cilj);
        Task<Cilj> GetCilj(int ciljId);
        Task<List<Cilj>> GetCiljeviKorisnika(string korisnikId);
        Task<List<Cilj>> GetAktivniCiljevi(string korisnikId);
        Task<Cilj> AzurirajCilj(Cilj cilj);
        Task ObrisiCilj(int ciljId);
        Task<KorakCilja> DodajKorak(KorakCilja korak);
        Task<KorakCilja> ZavrsiKorak(int korakId);
        Task<List<KorakCilja>> GetKoraciCilja(int ciljId);
        Task AzurirajProcenatNapretka(int ciljId);
    }
}