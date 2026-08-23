using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class SOSKontaktRepository : ISOSKontaktRepository
    {
        private readonly AppDbContext _context;

        public SOSKontaktRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SOSKontakt> KreirajSOSKontakt(SOSKontakt kontakt)
        {
            // Provera da li korisnik postoji
            var korisnik = await _context.Users.FindAsync(kontakt.KorisnikId);
            if (korisnik == null)
                throw new Exception("Korisnik nije pronađen");

            // Provera broja SOS kontakata (maksimalno 5)
            var brojKontakata = await _context.SOSKontakti
                .CountAsync(k => k.KorisnikId == kontakt.KorisnikId);

            if (brojKontakata >= 5)
                throw new Exception("Možete imati najviše 5 SOS kontakata");

            kontakt.Aktivan = true;
            kontakt.DatumKreiranja = DateTime.UtcNow;

            _context.SOSKontakti.Add(kontakt);
            await _context.SaveChangesAsync();

            return kontakt;
        }

        public async Task<SOSKontakt> GetSOSKontakt(int kontaktId)
        {
            return await _context.SOSKontakti
                .Include(k => k.Korisnik)
                .FirstOrDefaultAsync(k => k.SOSKontaktId == kontaktId);
        }

        public async Task<List<SOSKontakt>> GetSOSKontaktiKorisnika(string korisnikId)
        {
            return await _context.SOSKontakti
                .Where(k => k.KorisnikId == korisnikId)
                .OrderBy(k => k.ImeKontakta)
                .ToListAsync();
        }

        public async Task<List<SOSKontakt>> GetAktivniSOSKontakti(string korisnikId)
        {
            return await _context.SOSKontakti
                .Where(k => k.KorisnikId == korisnikId && k.Aktivan)
                .OrderBy(k => k.ImeKontakta)
                .ToListAsync();
        }

        public async Task<SOSKontakt> AzurirajSOSKontakt(SOSKontakt kontakt)
        {
            var postojeci = await _context.SOSKontakti.FindAsync(kontakt.SOSKontaktId);
            if (postojeci == null)
                throw new Exception("SOS kontakt nije pronađen");

            postojeci.ImeKontakta = kontakt.ImeKontakta ?? postojeci.ImeKontakta;
            postojeci.Telefon = kontakt.Telefon ?? postojeci.Telefon;
            postojeci.Email = kontakt.Email ?? postojeci.Email;
            postojeci.Napomena = kontakt.Napomena ?? postojeci.Napomena;
            postojeci.Aktivan = kontakt.Aktivan;

            await _context.SaveChangesAsync();
            return postojeci;
        }

        public async Task ObrisiSOSKontakt(int kontaktId)
        {
            var kontakt = await _context.SOSKontakti.FindAsync(kontaktId);
            if (kontakt != null)
            {
                _context.SOSKontakti.Remove(kontakt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AktivirajSOSKontakt(int kontaktId)
        {
            var kontakt = await _context.SOSKontakti.FindAsync(kontaktId);
            if (kontakt != null)
            {
                kontakt.Aktivan = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeaktivirajSOSKontakt(int kontaktId)
        {
            var kontakt = await _context.SOSKontakti.FindAsync(kontaktId);
            if (kontakt != null)
            {
                kontakt.Aktivan = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
