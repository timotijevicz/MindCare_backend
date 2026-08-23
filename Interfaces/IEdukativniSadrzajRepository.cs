using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface IEdukativniSadrzajRepository
    {
        Task<EdukativniSadrzaj> KreirajSadrzaj(EdukativniSadrzaj sadrzaj);
        Task<EdukativniSadrzaj> GetSadrzaj(int sadrzajId);
        Task<List<EdukativniSadrzaj>> GetSviSadrzaji();
        Task<List<EdukativniSadrzaj>> GetSadrzajiPoKategoriji(string kategorija);
        Task<List<EdukativniSadrzaj>> PretraziSadrzaje(string termin);
        Task<EdukativniSadrzaj> AzurirajSadrzaj(EdukativniSadrzaj sadrzaj);
        Task ObrisiSadrzaj(int sadrzajId);
        Task<List<EdukativniSadrzaj>> GetNajnovijiSadrzaji(int broj);
    }
}