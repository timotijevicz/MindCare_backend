using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class ZakazivanjeRepository : IZakazivanjeRepository
    {
        private readonly AppDbContext _context;

        public ZakazivanjeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ZakazivanjeSesije> KreirajZakazivanje(ZakazivanjeSesije zakazivanje)
        {
            // Provera da li terapeut postoji i da li je dostupan
            var terapeut = await _context.Terapeuti
                .Include(t => t.Korisnik)
                .FirstOrDefaultAsync(t => t.TerapeutId == zakazivanje.TerapeutId);

            if (terapeut == null)
                throw new Exception("Terapeut nije pronađen");

            if (!terapeut.Dostupan)
            {
                // Terapeut ne prima nove klijente, ali klijenti sa kojima je već imao
                // zakazivanje i dalje mogu da zakažu novu sesiju.
                var imaoRanijiKontakt = await _context.ZakazivanjaSesija
                    .AnyAsync(z => z.TerapeutId == zakazivanje.TerapeutId && z.KlijentId == zakazivanje.KlijentId);

                if (!imaoRanijiKontakt)
                    throw new Exception("Terapeut trenutno nije dostupan");
            }

            // Provera da li klijent postoji
            var klijent = await _context.Users.FindAsync(zakazivanje.KlijentId);
            if (klijent == null)
                throw new Exception("Klijent nije pronađen");

            // Provera dostupnosti termina
            var dostupnost = await ProveriDostupnostTermina(zakazivanje.TerapeutId, zakazivanje.DatumZakazane);
            if (!dostupnost)
                throw new Exception("Termin je već zauzet");

            // Zakazivanje čeka odluku terapeuta — ne postaje aktivno dok terapeut ne prihvati.
            zakazivanje.Status = "Na čekanju";
            zakazivanje.DatumKreiranja = DateTime.UtcNow;

            _context.ZakazivanjaSesija.Add(zakazivanje);
            await _context.SaveChangesAsync();

            return zakazivanje;
        }

        public async Task<ZakazivanjeSesije> GetZakazivanje(int zakazivanjeId)
        {
            return await _context.ZakazivanjaSesija
                .Include(z => z.Klijent)
                    .ThenInclude(k => k.Profil)
                .Include(z => z.Terapeut)
                    .ThenInclude(t => t.Korisnik)
                        .ThenInclude(k => k.Profil)
                .FirstOrDefaultAsync(z => z.ZakazivanjeSesijeId == zakazivanjeId);
        }

        public async Task<List<ZakazivanjeSesije>> GetZakazivanjaKlijenta(string klijentId)
        {
            return await _context.ZakazivanjaSesija
                .Include(z => z.Terapeut)
                    .ThenInclude(t => t.Korisnik)
                .Where(z => z.KlijentId == klijentId)
                .OrderByDescending(z => z.DatumZakazane)
                .ToListAsync();
        }

        public async Task<List<ZakazivanjeSesije>> GetZakazivanjaTerapeuta(int terapeutId)
        {
            return await _context.ZakazivanjaSesija
                .Include(z => z.Klijent)
                    .ThenInclude(k => k.Profil)
                .Where(z => z.TerapeutId == terapeutId)
                .OrderByDescending(z => z.DatumZakazane)
                .ToListAsync();
        }

        public async Task<List<ZakazivanjeSesije>> GetZakazivanjaZaDatum(DateTime datum)
        {
            return await _context.ZakazivanjaSesija
                .Include(z => z.Klijent)
                .Include(z => z.Terapeut)
                    .ThenInclude(t => t.Korisnik)
                .Where(z => z.DatumZakazane.Date == datum.Date && z.Status == "Aktivno")
                .OrderBy(z => z.DatumZakazane)
                .ToListAsync();
        }

        public async Task<ZakazivanjeSesije> AzurirajStatusZakazivanja(int zakazivanjeId, string status)
        {
            var zakazivanje = await _context.ZakazivanjaSesija
                .Include(z => z.Terapeut)
                    .ThenInclude(t => t.Korisnik)
                .FirstOrDefaultAsync(z => z.ZakazivanjeSesijeId == zakazivanjeId);
            if (zakazivanje == null)
                throw new Exception("Zakazivanje nije pronađeno");

            var dozvoljeniStatusi = new[] { "Na čekanju", "Aktivno", "Odbijeno", "Otkazano", "Završeno" };
            if (!dozvoljeniStatusi.Contains(status))
                throw new Exception("Nedozvoljen status");

            var zavrsniStatusi = new[] { "Završeno", "Odbijeno", "Otkazano" };
            if (zavrsniStatusi.Contains(zakazivanje.Status))
                throw new Exception($"Ne možete menjati zakazivanje sa statusom '{zakazivanje.Status}'");

            var prethodniStatus = zakazivanje.Status;
            zakazivanje.Status = status;
            await _context.SaveChangesAsync();

            // Podsetnik za sesiju se pravi tek kad terapeut prihvati zahtev.
            if (prethodniStatus == "Na čekanju" && status == "Aktivno")
            {
                var podsetnik = new Podsetnik
                {
                    KorisnikId = zakazivanje.KlijentId,
                    Tip = "Sesija",
                    Tekst = $"Podsetnik: Sesija sa terapeutom {zakazivanje.Terapeut.Korisnik.Ime} {zakazivanje.Terapeut.Korisnik.Prezime}",
                    Aktivan = true,
                    VremePodsetnika = zakazivanje.DatumZakazane.AddHours(-1).ToString("HH:mm"),
                    DatumKreiranja = DateTime.UtcNow
                };
                _context.Podsetnici.Add(podsetnik);
                await _context.SaveChangesAsync();
            }

            return zakazivanje;
        }

        public async Task<bool> ProveriDostupnostTermina(int terapeutId, DateTime datum)
        {
            // Provera da li terapeut već ima zakazivanje u tom terminu
            var pocetakTermina = datum;
            var krajTermina = datum.AddHours(1); // Pretpostavljamo da sesije traju 1 sat

            var preklapanje = await _context.ZakazivanjaSesija
                .AnyAsync(z => z.TerapeutId == terapeutId &&
                              (z.Status == "Aktivno" || z.Status == "Na čekanju") &&
                              z.DatumZakazane.Date == datum.Date &&
                              ((z.DatumZakazane <= pocetakTermina &&
                                z.DatumZakazane.AddHours(1) > pocetakTermina) ||
                               (pocetakTermina <= z.DatumZakazane &&
                                krajTermina > z.DatumZakazane)));

            return !preklapanje;
        }

        public async Task OtkaziZakazivanje(int zakazivanjeId)
        {
            var zakazivanje = await _context.ZakazivanjaSesija.FindAsync(zakazivanjeId);
            if (zakazivanje == null)
                throw new Exception("Zakazivanje nije pronađeno");

            var zavrsniStatusi = new[] { "Završeno", "Odbijeno", "Otkazano" };
            if (zavrsniStatusi.Contains(zakazivanje.Status))
                throw new Exception($"Ne možete otkazati zakazivanje sa statusom '{zakazivanje.Status}'");

            // Zahtev koji terapeut još nije prihvatio može da se povuče bilo kada;
            // već potvrđenu sesiju treba otkazati najkasnije 24h unapred.
            if (zakazivanje.Status == "Aktivno" && zakazivanje.DatumZakazane < DateTime.UtcNow.AddHours(24))
                throw new Exception("Zakazivanje možete otkazati najkasnije 24h pre termina");

            zakazivanje.Status = "Otkazano";
            zakazivanje.Napomena += " | Otkazano " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm");

            await _context.SaveChangesAsync();
        }
    }
}