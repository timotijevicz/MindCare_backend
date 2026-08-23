using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class SesijaRepository : ISesijaRepository
    {
        private readonly AppDbContext _context;

        public SesijaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sesija> KreirajSesiju(Sesija sesija)
        {
            // Provera da li klijent postoji
            var klijent = await _context.Users.FindAsync(sesija.KlijentId);
            if (klijent == null)
                throw new Exception("Klijent nije pronađen");

            // Provera da li terapeut postoji
            var terapeut = await _context.Users.FindAsync(sesija.TerapeutId);
            if (terapeut == null)
                throw new Exception("Terapeut nije pronađen");

            // Provera da li terapeut ima preklapanje termina
            var preklapanje = await _context.Sesije
                .AnyAsync(s => s.TerapeutId == sesija.TerapeutId &&
                              s.DatumSesije.Date == sesija.DatumSesije.Date &&
                              s.Status != "Otkazana" &&
                              ((s.DatumSesije <= sesija.DatumSesije &&
                                s.DatumSesije.AddMinutes(s.TrajanjeSesijeMinuta) > sesija.DatumSesije) ||
                               (sesija.DatumSesije <= s.DatumSesije &&
                                sesija.DatumSesije.AddMinutes(sesija.TrajanjeSesijeMinuta) > s.DatumSesije)));

            if (preklapanje)
                throw new Exception("Terapeut već ima zakazanu sesiju u ovom terminu");

            sesija.Status = "Zakazana";
            sesija.DatumKreiranja = DateTime.UtcNow;
            sesija.BeleskeTerapeuta ??= "";
            sesija.FeedbackKlijenta ??= "";

            _context.Sesije.Add(sesija);
            await _context.SaveChangesAsync();

            return sesija;
        }

        public async Task<Sesija> GetSesija(int sesijaId)
        {
            return await _context.Sesije
                .Include(s => s.Klijent)
                    .ThenInclude(k => k.Profil)
                .Include(s => s.Terapeut)
                    .ThenInclude(t => t.Profil)
                .Include(s => s.Poruke.OrderBy(p => p.DatumSlanja))
                .FirstOrDefaultAsync(s => s.SesijaId == sesijaId);
        }

        public async Task<List<Sesija>> GetSesijeKlijenta(string klijentId)
        {
            return await _context.Sesije
                .Include(s => s.Terapeut)
                    .ThenInclude(t => t.Profil)
                .Where(s => s.KlijentId == klijentId)
                .OrderByDescending(s => s.DatumSesije)
                .ToListAsync();
        }

        public async Task<List<Sesija>> GetSesijeTerapeuta(string terapeutId)
        {
            return await _context.Sesije
                .Include(s => s.Klijent)
                    .ThenInclude(k => k.Profil)
                .Where(s => s.TerapeutId == terapeutId)
                .OrderByDescending(s => s.DatumSesije)
                .ToListAsync();
        }

        public async Task<List<Sesija>> GetAktivneSesije()
        {
            return await _context.Sesije
                .Include(s => s.Klijent)
                .Include(s => s.Terapeut)
                .Where(s => s.Status == "Aktivna")
                .OrderBy(s => s.DatumSesije)
                .ToListAsync();
        }

        public async Task<List<Sesija>> GetZakazaneSesijeZaDanas()
        {
            var danas = DateTime.UtcNow.Date;
            var sutra = danas.AddDays(1);

            return await _context.Sesije
                .Include(s => s.Klijent)
                    .ThenInclude(k => k.Profil)
                .Include(s => s.Terapeut)
                    .ThenInclude(t => t.Profil)
                .Where(s => s.DatumSesije >= danas &&
                           s.DatumSesije < sutra &&
                           s.Status == "Zakazana")
                .OrderBy(s => s.DatumSesije)
                .ToListAsync();
        }

        public async Task<Sesija> AzurirajStatusSesije(int sesijaId, string status)
        {
            var sesija = await _context.Sesije.FindAsync(sesijaId);
            if (sesija == null)
                throw new Exception("Sesija nije pronađena");

            // Validacija statusa
            var dozvoljeniStatusi = new[] { "Zakazana", "Aktivna", "Završena", "Otkazana" };
            if (!dozvoljeniStatusi.Contains(status))
                throw new Exception("Nedozvoljen status sesije");

            // Logika prelaska statusa
            if (sesija.Status == "Završena" || sesija.Status == "Otkazana")
                throw new Exception("Ne možete menjati status završene ili otkazane sesije");

            if (status == "Aktivna" && sesija.Status != "Zakazana")
                throw new Exception("Samo zakazana sesija može biti aktivirana");

            sesija.Status = status;
            await _context.SaveChangesAsync();

            return sesija;
        }

        public async Task<Sesija> AzurirajSesiju(Sesija sesija)
        {
            var postojeca = await _context.Sesije.FindAsync(sesija.SesijaId);
            if (postojeca == null)
                throw new Exception("Sesija nije pronađena");

            if (postojeca.Status != "Zakazana")
                throw new Exception("Možete menjati samo zakazane sesije");

            postojeca.DatumSesije = sesija.DatumSesije;
            postojeca.TrajanjeSesijeMinuta = sesija.TrajanjeSesijeMinuta;
            postojeca.Tip = sesija.Tip ?? postojeca.Tip;

            await _context.SaveChangesAsync();
            return postojeca;
        }

        public async Task ObrisiSesiju(int sesijaId)
        {
            var sesija = await _context.Sesije.FindAsync(sesijaId);
            if (sesija != null)
            {
                if (sesija.Status == "Aktivna")
                    throw new Exception("Ne možete obrisati aktivnu sesiju");

                _context.Sesije.Remove(sesija);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DodajBeliskeTerapeuta(int sesijaId, string beleske)
        {
            var sesija = await _context.Sesije.FindAsync(sesijaId);
            if (sesija == null)
                throw new Exception("Sesija nije pronađena");

            sesija.BeleskeTerapeuta = beleske;
            await _context.SaveChangesAsync();
        }

        public async Task DodajFeedbackKlijenta(int sesijaId, string feedback)
        {
            var sesija = await _context.Sesije.FindAsync(sesijaId);
            if (sesija == null)
                throw new Exception("Sesija nije pronađena");

            if (sesija.Status != "Završena")
                throw new Exception("Feedback možete ostaviti samo za završene sesije");

            sesija.FeedbackKlijenta = feedback;
            await _context.SaveChangesAsync();
        }
    }
}