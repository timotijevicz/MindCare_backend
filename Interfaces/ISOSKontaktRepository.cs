using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface ISOSKontaktRepository
    {
        Task<SOSKontakt> KreirajSOSKontakt(SOSKontakt kontakt);
        Task<SOSKontakt> GetSOSKontakt(int kontaktId);
        Task<List<SOSKontakt>> GetSOSKontaktiKorisnika(string korisnikId);
        Task<List<SOSKontakt>> GetAktivniSOSKontakti(string korisnikId);
        Task<SOSKontakt> AzurirajSOSKontakt(SOSKontakt kontakt);
        Task ObrisiSOSKontakt(int kontaktId);
        Task AktivirajSOSKontakt(int kontaktId);
        Task DeaktivirajSOSKontakt(int kontaktId);
    }
}