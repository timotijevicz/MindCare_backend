using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class NavikaRepository : INavikaRepository
    {
        private readonly AppDbContext _context;

        public NavikaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Navika> KreirajNaviku(Navika navika)
        {
            // Validacija kategorije
            var dozvoljeneKategorije = new[] { "Meditacija", "Vežbanje", "Čitanje", "Dnevnik" };
            if (!dozvoljeneKategorije.Contains(navika.Kategorija))
                throw new Exception("Nedozvoljena kategorija navike");

            // Validacija učestalosti
            var dozvoljeneUcestalosti = new[] { "Dnevno", "Nedeljno", "Mesečno" };
            if (!dozvoljeneUcestalosti.Contains(navika.Ucestalost))
                throw new Exception("Nedozvoljena učestalost");

            navika.Aktivna = true;
            navika.DatumPocetka = DateTime.UtcNow;

            _context.Navike.Add(navika);
            await _context.SaveChangesAsync();

            return navika;
        }

        public async Task<Navika> GetNavika(int navikaId)
        {
            return await _context.Navike
                .Include(n => n.Korisnik)
                .Include(n => n.Pracenja.OrderByDescending(p => p.Datum))
                .FirstOrDefaultAsync(n => n.NavikaId == navikaId);
        }

        public async Task<List<Navika>> GetNavikeKorisnika(string korisnikId)
        {
            return await _context.Navike
                .Include(n => n.Pracenja)
                .Where(n => n.KorisnikId == korisnikId)
                .OrderByDescending(n => n.DatumPocetka)
                .ToListAsync();
        }

        public async Task<List<Navika>> GetAktivneNavike(string korisnikId)
        {
            return await _context.Navike
                .Include(n => n.Pracenja)
                .Where(n => n.KorisnikId == korisnikId && n.Aktivna)
                .OrderByDescending(n => n.DatumPocetka)
                .ToListAsync();
        }

        public async Task<Navika> AzurirajNaviku(Navika navika)
        {
            var postojeca = await _context.Navike.FindAsync(navika.NavikaId);
            if (postojeca == null)
                throw new Exception("Navika nije pronađena");

            postojeca.NazivNavike = navika.NazivNavike ?? postojeca.NazivNavike;
            postojeca.Opis = navika.Opis ?? postojeca.Opis;
            postojeca.Kategorija = navika.Kategorija ?? postojeca.Kategorija;
            postojeca.Ucestalost = navika.Ucestalost ?? postojeca.Ucestalost;

            await _context.SaveChangesAsync();
            return postojeca;
        }

        public async Task ObrisiNaviku(int navikaId)
        {
            var navika = await _context.Navike.FindAsync(navikaId);
            if (navika != null)
            {
                // Briši i sva praćenja navike
                var pracenja = await _context.PracenjaNavika
                    .Where(p => p.NavikaId == navikaId)
                    .ToListAsync();

                _context.PracenjaNavika.RemoveRange(pracenja);
                _context.Navike.Remove(navika);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AktivirajNaviku(int navikaId)
        {
            var navika = await _context.Navike.FindAsync(navikaId);
            if (navika != null)
            {
                navika.Aktivna = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeaktivirajNaviku(int navikaId)
        {
            var navika = await _context.Navike.FindAsync(navikaId);
            if (navika != null)
            {
                navika.Aktivna = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PracenjeNavike> ZabeleziPracenje(PracenjeNavike pracenje)
        {
            var navika = await _context.Navike.FindAsync(pracenje.NavikaId);
            if (navika == null)
                throw new Exception("Navika nije pronađena");

            if (!navika.Aktivna)
                throw new Exception("Ne možete pratiti neaktivnu naviku");

            // Provera da li već postoji praćenje za ovaj datum
            var postojecPracenje = await _context.PracenjaNavika
                .FirstOrDefaultAsync(p => p.NavikaId == pracenje.NavikaId &&
                                         p.Datum.Date == pracenje.Datum.Date);

            if (postojecPracenje != null)
            {
                postojecPracenje.Zavrseno = pracenje.Zavrseno;
                postojecPracenje.Komentar = pracenje.Komentar ?? postojecPracenje.Komentar;
                await _context.SaveChangesAsync();
                return postojecPracenje;
            }

            _context.PracenjaNavika.Add(pracenje);
            await _context.SaveChangesAsync();

            return pracenje;
        }

        public async Task<List<PracenjeNavike>> GetPracenjaNavike(int navikaId)
        {
            return await _context.PracenjaNavika
                .Where(p => p.NavikaId == navikaId)
                .OrderByDescending(p => p.Datum)
                .ToListAsync();
        }

        public async Task<List<PracenjeNavike>> GetPracenjaZaPeriod(int navikaId, DateTime od, DateTime dok)
        {
            return await _context.PracenjaNavika
                .Where(p => p.NavikaId == navikaId &&
                           p.Datum >= od &&
                           p.Datum <= dok)
                .OrderBy(p => p.Datum)
                .ToListAsync();
        }

        public async Task<double> GetProcenatIspunjavanja(int navikaId, int brojDana)
        {
            var odDatuma = DateTime.UtcNow.AddDays(-brojDana);
            var pracenja = await _context.PracenjaNavika
                .Where(p => p.NavikaId == navikaId &&
                           p.Datum >= odDatuma &&
                           p.Zavrseno)
                .ToListAsync();

            var navika = await _context.Navike.FindAsync(navikaId);
            if (navika == null)
                return 0;

            // Računanje očekivanog broja praćenja na osnovu učestalosti
            var ocekivaniBroj = navika.Ucestalost switch
            {
                "Dnevno" => brojDana,
                "Nedeljno" => brojDana / 7,
                "Mesečno" => brojDana / 30,
                _ => brojDana
            };

            if (ocekivaniBroj == 0)
                return 0;

            return Math.Round((double)pracenja.Count / ocekivaniBroj * 100, 1);
        }
    }
}