using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class DnevnikMisliRepository : IDnevnikMisliRepository
    {
        private readonly AppDbContext _context;

        public DnevnikMisliRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DnevnikMisli> KreirajBelisku(DnevnikMisli beleska)
        {
            beleska.DatumKreiranja = DateTime.UtcNow;
            _context.DnevnikMisli.Add(beleska);
            await _context.SaveChangesAsync();
            return beleska;
        }

        public async Task<DnevnikMisli> GetBeleska(int beleskaId)
        {
            return await _context.DnevnikMisli
                .Include(b => b.Korisnik)
                .FirstOrDefaultAsync(b => b.BeleskaId == beleskaId);
        }

        public async Task<List<DnevnikMisli>> GetBeleskeKorisnika(string korisnikId)
        {
            return await _context.DnevnikMisli
                .Where(b => b.KorisnikId == korisnikId)
                .OrderByDescending(b => b.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<List<DnevnikMisli>> GetBeleskePoKategoriji(string korisnikId, string kategorija)
        {
            return await _context.DnevnikMisli
                .Where(b => b.KorisnikId == korisnikId && b.Kategorija == kategorija)
                .OrderByDescending(b => b.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<List<DnevnikMisli>> PretraziBeleske(string korisnikId, string pojam)
        {
            return await _context.DnevnikMisli
                .Where(b => b.KorisnikId == korisnikId &&
                    (b.Naslov.Contains(pojam) || b.Sadrzaj.Contains(pojam)))
                .OrderByDescending(b => b.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<List<DnevnikMisli>> GetDeljeneBeleske(string terapeutId)
        {
            return await _context.DnevnikMisli
                .Include(b => b.Korisnik)
                .Where(b => b.Deljena && b.DeljenjeTerapeutId == terapeutId)
                .OrderByDescending(b => b.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<DnevnikMisli> AzurirajBelisku(DnevnikMisli beleska)
        {
            var postojeca = await _context.DnevnikMisli.FindAsync(beleska.BeleskaId);
            if (postojeca == null)
                throw new Exception("Beleška nije pronađena");

            postojeca.Naslov = beleska.Naslov;
            postojeca.Sadrzaj = beleska.Sadrzaj;
            postojeca.Kategorija = beleska.Kategorija;
            postojeca.Deljena = beleska.Deljena;
            postojeca.DeljenjeTerapeutId = beleska.DeljenjeTerapeutId;
            postojeca.DatumAzuriranja = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return postojeca;
        }

        public async Task ObrisiBelisku(int beleskaId)
        {
            var beleska = await _context.DnevnikMisli.FindAsync(beleskaId);
            if (beleska != null)
            {
                _context.DnevnikMisli.Remove(beleska);
                await _context.SaveChangesAsync();
            }
        }

        public async Task PodeliBeliskuSaTerapeutom(int beleskaId, string terapeutId)
        {
            var beleska = await _context.DnevnikMisli.FindAsync(beleskaId);
            if (beleska == null)
                throw new Exception("Beleška nije pronađena");

            // Provera da li terapeut postoji
            var terapeut = await _context.Terapeuti
                .FirstOrDefaultAsync(t => t.KorisnikId == terapeutId);
            if (terapeut == null)
                throw new Exception("Terapeut nije pronađen");

            beleska.Deljena = true;
            beleska.DeljenjeTerapeutId = terapeutId;
            beleska.DatumAzuriranja = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task UkiniDeljenjeBeliske(int beleskaId)
        {
            var beleska = await _context.DnevnikMisli.FindAsync(beleskaId);
            if (beleska == null)
                throw new Exception("Beleška nije pronađena");

            beleska.Deljena = false;
            beleska.DeljenjeTerapeutId = "";
            beleska.DatumAzuriranja = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}