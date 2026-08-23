using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class EdukativniSadrzajRepository : IEdukativniSadrzajRepository
    {
        private readonly AppDbContext _context;

        public EdukativniSadrzajRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EdukativniSadrzaj> KreirajSadrzaj(EdukativniSadrzaj sadrzaj)
        {
            // Validacija kategorije
            var dozvoljeneKategorije = new[] { "Članak", "Video", "Meditacija", "Vežba" };
            if (!dozvoljeneKategorije.Contains(sadrzaj.Kategorija))
                throw new Exception("Nedozvoljena kategorija. Dozvoljene su: Članak, Video, Meditacija, Vežba");

            // Validacija URL-a
            if (!Uri.TryCreate(sadrzaj.Url, UriKind.Absolute, out _))
                throw new Exception("URL nije validan");

            sadrzaj.DatumObjave = DateTime.UtcNow;

            _context.EdukativniSadrzaj.Add(sadrzaj);
            await _context.SaveChangesAsync();

            return sadrzaj;
        }

        public async Task<EdukativniSadrzaj> GetSadrzaj(int sadrzajId)
        {
            return await _context.EdukativniSadrzaj.FindAsync(sadrzajId);
        }

        public async Task<List<EdukativniSadrzaj>> GetSviSadrzaji()
        {
            return await _context.EdukativniSadrzaj
                .OrderByDescending(s => s.DatumObjave)
                .ToListAsync();
        }

        public async Task<List<EdukativniSadrzaj>> GetSadrzajiPoKategoriji(string kategorija)
        {
            return await _context.EdukativniSadrzaj
                .Where(s => s.Kategorija == kategorija)
                .OrderByDescending(s => s.DatumObjave)
                .ToListAsync();
        }

        public async Task<List<EdukativniSadrzaj>> PretraziSadrzaje(string termin)
        {
            return await _context.EdukativniSadrzaj
                .Where(s => s.Naslov.Contains(termin) ||
                           s.Opis.Contains(termin) ||
                           s.Autor.Contains(termin))
                .OrderByDescending(s => s.DatumObjave)
                .ToListAsync();
        }

        public async Task<EdukativniSadrzaj> AzurirajSadrzaj(EdukativniSadrzaj sadrzaj)
        {
            var postojeci = await _context.EdukativniSadrzaj.FindAsync(sadrzaj.SadrzajId);
            if (postojeci == null)
                throw new Exception("Sadržaj nije pronađen");

            postojeci.Naslov = sadrzaj.Naslov ?? postojeci.Naslov;
            postojeci.Opis = sadrzaj.Opis ?? postojeci.Opis;
            postojeci.Kategorija = sadrzaj.Kategorija ?? postojeci.Kategorija;
            postojeci.Url = sadrzaj.Url ?? postojeci.Url;
            postojeci.Autor = sadrzaj.Autor ?? postojeci.Autor;

            await _context.SaveChangesAsync();
            return postojeci;
        }

        public async Task ObrisiSadrzaj(int sadrzajId)
        {
            var sadrzaj = await _context.EdukativniSadrzaj.FindAsync(sadrzajId);
            if (sadrzaj != null)
            {
                _context.EdukativniSadrzaj.Remove(sadrzaj);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EdukativniSadrzaj>> GetNajnovijiSadrzaji(int broj)
        {
            return await _context.EdukativniSadrzaj
                .OrderByDescending(s => s.DatumObjave)
                .Take(broj)
                .ToListAsync();
        }
    }
}