using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class RaspolozenjeRepository : IRaspolozenjeRepository
    {
        private readonly AppDbContext _context;

        public RaspolozenjeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DnevnoRaspolozenje> KreirajRaspolozenje(DnevnoRaspolozenje raspolozenje)
        {
            // Provera da li već postoji unos za danas
            var danas = DateTime.UtcNow.Date;
            var postojeci = await _context.DnevnaRaspolozenja
                .FirstOrDefaultAsync(r => r.KorisnikId == raspolozenje.KorisnikId &&
                                         r.DatumUnosa.Date == danas);

            if (postojeci != null)
            {
                // Ažuriraj postojeći unos
                postojeci.OcenaRaspolozenja = raspolozenje.OcenaRaspolozenja;
                postojeci.Emotikon = raspolozenje.Emotikon;
                postojeci.Opis = raspolozenje.Opis;
                postojeci.UzrociStresa = raspolozenje.UzrociStresa;
                postojeci.Afirmacija = raspolozenje.Afirmacija;
                postojeci.KvalitetSna = raspolozenje.KvalitetSna;
                postojeci.KvalitetIshrane = raspolozenje.KvalitetIshrane;
                postojeci.NivoFizickeAktivnosti = raspolozenje.NivoFizickeAktivnosti;

                await _context.SaveChangesAsync();
                return postojeci;
            }

            raspolozenje.DatumUnosa = DateTime.UtcNow;
            _context.DnevnaRaspolozenja.Add(raspolozenje);
            await _context.SaveChangesAsync();
            return raspolozenje;
        }

        public async Task<DnevnoRaspolozenje> GetRaspolozenje(int raspolozenjeId)
        {
            return await _context.DnevnaRaspolozenja
                .Include(r => r.Korisnik)
                .FirstOrDefaultAsync(r => r.RaspolozenjeId == raspolozenjeId);
        }

        public async Task<List<DnevnoRaspolozenje>> GetRaspolozenjaKorisnika(string korisnikId)
        {
            return await _context.DnevnaRaspolozenja
                .Where(r => r.KorisnikId == korisnikId)
                .OrderByDescending(r => r.DatumUnosa)
                .ToListAsync();
        }

        public async Task<List<DnevnoRaspolozenje>> GetRaspolozenjaPeriod(string korisnikId, DateTime od, DateTime dok)
        {
            return await _context.DnevnaRaspolozenja
                .Where(r => r.KorisnikId == korisnikId &&
                           r.DatumUnosa >= od &&
                           r.DatumUnosa <= dok)
                .OrderBy(r => r.DatumUnosa)
                .ToListAsync();
        }

        public async Task<DnevnoRaspolozenje> AzurirajRaspolozenje(DnevnoRaspolozenje raspolozenje)
        {
            _context.DnevnaRaspolozenja.Update(raspolozenje);
            await _context.SaveChangesAsync();
            return raspolozenje;
        }

        public async Task ObrisiRaspolozenje(int raspolozenjeId)
        {
            var raspolozenje = await _context.DnevnaRaspolozenja.FindAsync(raspolozenjeId);
            if (raspolozenje != null)
            {
                _context.DnevnaRaspolozenja.Remove(raspolozenje);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double> GetProsecnaOcena(string korisnikId)
        {
            var ocene = await _context.DnevnaRaspolozenja
                .Where(r => r.KorisnikId == korisnikId)
                .Select(r => r.OcenaRaspolozenja)
                .ToListAsync();

            return ocene.Any() ? Math.Round(ocene.Average(), 2) : 0;
        }

        public async Task<int> GetBrojUnosa(string korisnikId)
        {
            return await _context.DnevnaRaspolozenja
                .CountAsync(r => r.KorisnikId == korisnikId);
        }

        public async Task<DnevnoRaspolozenje> GetDanasnjeRaspolozenje(string korisnikId)
        {
            var danas = DateTime.UtcNow.Date;
            return await _context.DnevnaRaspolozenja
                .FirstOrDefaultAsync(r => r.KorisnikId == korisnikId &&
                                         r.DatumUnosa.Date == danas);
        }

        public async Task<Dictionary<string, double>> GetStatistikaPoDanima(string korisnikId, int brojDana)
        {
            var odDatum = DateTime.UtcNow.AddDays(-brojDana);
            var raspolozenja = await _context.DnevnaRaspolozenja
                .Where(r => r.KorisnikId == korisnikId && r.DatumUnosa >= odDatum)
                .OrderBy(r => r.DatumUnosa)
                .ToListAsync();

            var statistika = new Dictionary<string, double>();
            for (var datum = odDatum.Date; datum <= DateTime.UtcNow.Date; datum = datum.AddDays(1))
            {
                var dnevnaRaspolozenja = raspolozenja
                    .Where(r => r.DatumUnosa.Date == datum)
                    .ToList();

                var prosek = dnevnaRaspolozenja.Any()
                    ? dnevnaRaspolozenja.Average(r => r.OcenaRaspolozenja)
                    : 0;

                statistika[datum.ToString("dd.MM.yyyy")] = Math.Round(prosek, 1);
            }

            return statistika;
        }

        public async Task<Dictionary<DayOfWeek, double>> GetProsekPoDanuUNedelji(string korisnikId, int brojDana)
        {
            var odDatum = DateTime.UtcNow.AddDays(-brojDana);
            var raspolozenja = await _context.DnevnaRaspolozenja
                .Where(r => r.KorisnikId == korisnikId && r.DatumUnosa >= odDatum)
                .ToListAsync();

            var prosekPoDanu = new Dictionary<DayOfWeek, double>();
            foreach (DayOfWeek dan in Enum.GetValues(typeof(DayOfWeek)))
            {
                var unosiZaDan = raspolozenja.Where(r => r.DatumUnosa.DayOfWeek == dan).ToList();
                prosekPoDanu[dan] = unosiZaDan.Any()
                    ? Math.Round(unosiZaDan.Average(r => r.OcenaRaspolozenja), 1)
                    : 0;
            }

            return prosekPoDanu;
        }

        public async Task<List<DnevnoRaspolozenje>> GetRaspolozenjaKlijentaZaTerapeuta(string terapeutId, string klijentId)
        {
            // Provera da li terapeut ima pristup ovom klijentu
            var imaPristup = await _context.Sesije
                .AnyAsync(s => s.TerapeutId == terapeutId && s.KlijentId == klijentId);

            if (!imaPristup)
                throw new Exception("Nemate pristup podacima ovog klijenta");

            return await _context.DnevnaRaspolozenja
                .Where(r => r.KorisnikId == klijentId)
                .OrderByDescending(r => r.DatumUnosa)
                .ToListAsync();
        }
    }
}