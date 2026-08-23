using MentalHealth.Data.Models;

namespace MentalHealth.Interfejsi
{
    public interface ISesijaRepository
    {
        Task<Sesija> KreirajSesiju(Sesija sesija);
        Task<Sesija> GetSesija(int sesijaId);
        Task<List<Sesija>> GetSesijeKlijenta(string klijentId);
        Task<List<Sesija>> GetSesijeTerapeuta(string terapeutId);
        Task<List<Sesija>> GetAktivneSesije();
        Task<List<Sesija>> GetZakazaneSesijeZaDanas();
        Task<Sesija> AzurirajStatusSesije(int sesijaId, string status);
        Task<Sesija> AzurirajSesiju(Sesija sesija);
        Task ObrisiSesiju(int sesijaId);
        Task DodajBeliskeTerapeuta(int sesijaId, string beleske);
        Task DodajFeedbackKlijenta(int sesijaId, string feedback);
    }
}