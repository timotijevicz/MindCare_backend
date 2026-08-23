using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class CiljRepository : ICiljRepository
    {
        private readonly AppDbContext _context;

        public CiljRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cilj> KreirajCilj(Cilj cilj)
        {
            // Validacija kategorije
            var dozvoljeneKategorije = new[] { "MentalnoZdravlje", "FizičkoZdravlje", "Odnosi", "Karijera" };
            if (!dozvoljeneKategorije.Contains(cilj.Kategorija))
                throw new Exception("Nedozvoljena kategorija cilja");

            // Provera datuma završetka
            if (cilj.DatumZavrsetka.HasValue && cilj.DatumZavrsetka.Value < DateTime.UtcNow)
                throw new Exception("Datum završetka ne može biti u prošlosti");

            cilj.Status = "Aktivan";
            cilj.ProcenatNapretka = 0;
            cilj.DatumPocetka = DateTime.UtcNow;

            _context.Ciljevi.Add(cilj);
            await _context.SaveChangesAsync();

            return cilj;
        }

        public async Task<Cilj> GetCilj(int ciljId)
        {
            return await _context.Ciljevi
                .Include(c => c.Korisnik)
                .Include(c => c.Koraci)
                .FirstOrDefaultAsync(c => c.CiljId == ciljId);
        }

        public async Task<List<Cilj>> GetCiljeviKorisnika(string korisnikId)
        {
            return await _context.Ciljevi
                .Include(c => c.Koraci)
                .Where(c => c.KorisnikId == korisnikId)
                .OrderByDescending(c => c.DatumPocetka)
                .ToListAsync();
        }

        public async Task<List<Cilj>> GetAktivniCiljevi(string korisnikId)
        {
            return await _context.Ciljevi
                .Include(c => c.Koraci)
                .Where(c => c.KorisnikId == korisnikId && c.Status == "Aktivan")
                .OrderByDescending(c => c.DatumPocetka)
                .ToListAsync();
        }

        public async Task<Cilj> AzurirajCilj(Cilj cilj)
        {
            var postojeci = await _context.Ciljevi.FindAsync(cilj.CiljId);
            if (postojeci == null)
                throw new Exception("Cilj nije pronađen");

            postojeci.NazivCilja = cilj.NazivCilja ?? postojeci.NazivCilja;
            postojeci.Opis = cilj.Opis ?? postojeci.Opis;
            postojeci.Kategorija = cilj.Kategorija ?? postojeci.Kategorija;
            postojeci.DatumZavrsetka = cilj.DatumZavrsetka ?? postojeci.DatumZavrsetka;

            await _context.SaveChangesAsync();
            return postojeci;
        }

        public async Task ObrisiCilj(int ciljId)
        {
            var cilj = await _context.Ciljevi.FindAsync(ciljId);
            if (cilj != null)
            {
                // Briši i sve korake cilja
                var koraci = await _context.KoraciCiljeva
                    .Where(k => k.CiljId == ciljId)
                    .ToListAsync();

                _context.KoraciCiljeva.RemoveRange(koraci);
                _context.Ciljevi.Remove(cilj);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<KorakCilja> DodajKorak(KorakCilja korak)
        {
            var cilj = await _context.Ciljevi.FindAsync(korak.CiljId);
            if (cilj == null)
                throw new Exception("Cilj nije pronađen");

            if (cilj.Status != "Aktivan")
                throw new Exception("Možete dodavati korake samo aktivnim ciljevima");

            korak.Zavrsen = false;
            _context.KoraciCiljeva.Add(korak);
            await _context.SaveChangesAsync();

            // Ažuriraj procenat napretka
            await AzurirajProcenatNapretka(korak.CiljId);

            return korak;
        }

        public async Task<KorakCilja> ZavrsiKorak(int korakId)
        {
            var korak = await _context.KoraciCiljeva.FindAsync(korakId);
            if (korak == null)
                throw new Exception("Korak nije pronađen");

            korak.Zavrsen = true;
            korak.DatumZavrsetka = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Ažuriraj procenat napretka cilja
            await AzurirajProcenatNapretka(korak.CiljId);

            // Proveri da li su svi koraci završeni
            var sviKoraci = await GetKoraciCilja(korak.CiljId);
            if (sviKoraci.All(k => k.Zavrsen))
            {
                var cilj = await _context.Ciljevi.FindAsync(korak.CiljId);
                if (cilj != null)
                {
                    cilj.Status = "Završen";
                    cilj.ProcenatNapretka = 100;
                    await _context.SaveChangesAsync();
                }
            }

            return korak;
        }

        public async Task<KorakCilja> PonistiKorak(int korakId)
        {
            var korak = await _context.KoraciCiljeva.FindAsync(korakId);
            if (korak == null)
                throw new Exception("Korak nije pronađen");

            korak.Zavrsen = false;
            korak.DatumZavrsetka = null;
            await _context.SaveChangesAsync();

            await AzurirajProcenatNapretka(korak.CiljId);

            // Ako je cilj u međuvremenu bio označen kao završen, vrati ga na aktivan
            var cilj = await _context.Ciljevi.FindAsync(korak.CiljId);
            if (cilj != null && cilj.Status == "Završen")
            {
                cilj.Status = "Aktivan";
                await _context.SaveChangesAsync();
            }

            return korak;
        }

        public async Task<List<KorakCilja>> GetKoraciCilja(int ciljId)
        {
            return await _context.KoraciCiljeva
                .Where(k => k.CiljId == ciljId)
                .OrderBy(k => k.KorakId)
                .ToListAsync();
        }

        public async Task AzurirajProcenatNapretka(int ciljId)
        {
            var koraci = await _context.KoraciCiljeva
                .Where(k => k.CiljId == ciljId)
                .ToListAsync();

            if (koraci.Any())
            {
                var procenat = (double)koraci.Count(k => k.Zavrsen) / koraci.Count * 100;
                var cilj = await _context.Ciljevi.FindAsync(ciljId);
                if (cilj != null)
                {
                    cilj.ProcenatNapretka = (int)Math.Round(procenat);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}