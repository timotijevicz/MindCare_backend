using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly AppDbContext _context;

        public KorisnikRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Korisnik> GetKorisnikSaProfilom(string korisnikId)
        {
            return await _context.Users
                .Include(k => k.Profil)
                .Include(k => k.Terapeut)
                .FirstOrDefaultAsync(k => k.Id == korisnikId);
        }

        public async Task<KorisnickiProfil> GetProfil(string korisnikId)
        {
            return await _context.KorisnickiProfili
                .FirstOrDefaultAsync(p => p.KorisnikId == korisnikId);
        }

        public async Task<KorisnickiProfil> AzurirajProfil(string korisnikId, KorisnickiProfil profil)
        {
            var postojeciProfil = await _context.KorisnickiProfili
                .FirstOrDefaultAsync(p => p.KorisnikId == korisnikId);

            if (postojeciProfil == null)
            {
                profil.KorisnikId = korisnikId;
                profil.DatumAzuriranja = DateTime.UtcNow;
                _context.KorisnickiProfili.Add(profil);
            }
            else
            {
                postojeciProfil.OpisProfila = profil.OpisProfila ?? postojeciProfil.OpisProfila;
                postojeciProfil.ProfilnaSlika = profil.ProfilnaSlika ?? postojeciProfil.ProfilnaSlika;
                postojeciProfil.PreferiraniSat = profil.PreferiraniSat ?? postojeciProfil.PreferiraniSat;
                postojeciProfil.PrimiMotivacionuPoruku = profil.PrimiMotivacionuPoruku;
                postojeciProfil.PrimiPodsetnik = profil.PrimiPodsetnik;
                postojeciProfil.Cilj = profil.Cilj ?? postojeciProfil.Cilj;
                postojeciProfil.DatumAzuriranja = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return postojeciProfil ?? profil;
        }

        public async Task<List<Terapeut>> GetSviTerapeuti()
        {
            return await _context.Terapeuti
                .Include(t => t.Korisnik)
                    .ThenInclude(k => k.Profil)
                .Where(t => t.Korisnik.AktivnaNalog)
                .ToListAsync();
        }

        public async Task<List<Terapeut>> GetDostupniTerapeuti()
        {
            return await _context.Terapeuti
                .Include(t => t.Korisnik)
                    .ThenInclude(k => k.Profil)
                .Where(t => t.Dostupan && t.Korisnik.AktivnaNalog)
                .ToListAsync();
        }

        public async Task<List<Terapeut>> GetTerapeutiZaKlijenta(string klijentId)
        {
            // Terapeut koji je postavio da nije dostupan za nove klijente i dalje treba
            // da bude vidljiv klijentima sa kojima je već imao neki oblik kontakta —
            // zakazivanje, poruku ili podeljenu belešku iz dnevnika misli.
            var poznatiIzZakazivanja = await _context.ZakazivanjaSesija
                .Where(z => z.KlijentId == klijentId)
                .Select(z => z.TerapeutId)
                .Distinct()
                .ToListAsync();

            var poznatiKorisniciIzPoruka = await _context.Poruke
                .Where(p => p.PosiljaocId == klijentId || p.PrimalacId == klijentId)
                .Select(p => p.PosiljaocId == klijentId ? p.PrimalacId : p.PosiljaocId)
                .Distinct()
                .ToListAsync();

            var poznatiKorisniciIzDeljenihBeleski = await _context.DnevnikMisli
                .Where(d => d.KorisnikId == klijentId && d.DeljenjeTerapeutId != null && d.DeljenjeTerapeutId != "")
                .Select(d => d.DeljenjeTerapeutId)
                .Distinct()
                .ToListAsync();

            return await _context.Terapeuti
                .Include(t => t.Korisnik)
                    .ThenInclude(k => k.Profil)
                .Where(t => t.Korisnik.AktivnaNalog && (
                    t.Dostupan ||
                    poznatiIzZakazivanja.Contains(t.TerapeutId) ||
                    poznatiKorisniciIzPoruka.Contains(t.KorisnikId) ||
                    poznatiKorisniciIzDeljenihBeleski.Contains(t.KorisnikId)
                ))
                .ToListAsync();
        }

        public async Task<Terapeut> GetTerapeut(int terapeutId)
        {
            return await _context.Terapeuti
                .Include(t => t.Korisnik)
                    .ThenInclude(k => k.Profil)
                .FirstOrDefaultAsync(t => t.TerapeutId == terapeutId);
        }

        public async Task<Terapeut> GetTerapeutPoKorisnikId(string korisnikId)
        {
            return await _context.Terapeuti
                .Include(t => t.Korisnik)
                .FirstOrDefaultAsync(t => t.KorisnikId == korisnikId);
        }

        public async Task<Terapeut> AzurirajTerapeuta(int terapeutId, Terapeut terapeut)
        {
            var postojeciTerapeut = await _context.Terapeuti.FindAsync(terapeutId);
            if (postojeciTerapeut == null)
                throw new Exception("Terapeut nije pronađen");

            postojeciTerapeut.Zvanje = terapeut.Zvanje ?? postojeciTerapeut.Zvanje;
            postojeciTerapeut.Licenca = terapeut.Licenca ?? postojeciTerapeut.Licenca;
            postojeciTerapeut.OpisPrakse = terapeut.OpisPrakse ?? postojeciTerapeut.OpisPrakse;
            postojeciTerapeut.Dostupan = terapeut.Dostupan;
            postojeciTerapeut.SatnicaKonzultacije = terapeut.SatnicaKonzultacije;

            await _context.SaveChangesAsync();
            return postojeciTerapeut;
        }

        public async Task<List<Korisnik>> GetSviKlijenti()
        {
            var klijentRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Klijent");

            if (klijentRole == null)
                return new List<Korisnik>();

            var klijentIds = await _context.UserRoles
                .Where(ur => ur.RoleId == klijentRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Nalog može istovremeno imati i ulogu Administrator (npr. ako je prvo registrovan
            // kao klijent pa mu je uloga naknadno dodata) — takav nalog se ne prikazuje u listi
            // klijenata da admin ne bi slučajno deaktivirao ili obrisao sopstveni nalog odatle.
            var adminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Administrator");

            var adminIds = adminRole == null
                ? new List<string>()
                : await _context.UserRoles
                    .Where(ur => ur.RoleId == adminRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

            // Namerno bez filtera po AktivnaNalog — admin panel mora da vidi i deaktivirane
            // naloge da bi mogao ponovo da ih aktivira.
            return await _context.Users
                .Include(k => k.Profil)
                .Where(k => klijentIds.Contains(k.Id) && !adminIds.Contains(k.Id))
                .ToListAsync();
        }

        public async Task<List<Korisnik>> GetKlijentiZaTerapeuta(string terapeutKorisnikId)
        {
            var terapeut = await _context.Terapeuti
                .FirstOrDefaultAsync(t => t.KorisnikId == terapeutKorisnikId);

            var izPoruka = await _context.Poruke
                .Where(p => p.PosiljaocId == terapeutKorisnikId || p.PrimalacId == terapeutKorisnikId)
                .Select(p => p.PosiljaocId == terapeutKorisnikId ? p.PrimalacId : p.PosiljaocId)
                .Distinct()
                .ToListAsync();

            var izBeleski = await _context.DnevnikMisli
                .Where(d => d.Deljena && d.DeljenjeTerapeutId == terapeutKorisnikId)
                .Select(d => d.KorisnikId)
                .Distinct()
                .ToListAsync();

            var izSesija = await _context.Sesije
                .Where(s => s.TerapeutId == terapeutKorisnikId)
                .Select(s => s.KlijentId)
                .Distinct()
                .ToListAsync();

            var izZakazivanja = terapeut == null
                ? new List<string>()
                : await _context.ZakazivanjaSesija
                    .Where(z => z.TerapeutId == terapeut.TerapeutId)
                    .Select(z => z.KlijentId)
                    .Distinct()
                    .ToListAsync();

            var klijentIds = izPoruka
                .Union(izBeleski)
                .Union(izSesija)
                .Union(izZakazivanja)
                .Distinct()
                .ToList();

            return await _context.Users
                .Include(k => k.Profil)
                .Where(k => klijentIds.Contains(k.Id) && k.AktivnaNalog)
                .ToListAsync();
        }

        public async Task<Korisnik> GetKlijentDetalji(string klijentId)
        {
            return await _context.Users
                .Include(k => k.Profil)
                .Include(k => k.Raspolozenja.OrderByDescending(r => r.DatumUnosa).Take(10))
                .Include(k => k.DnevnikMisli.OrderByDescending(d => d.DatumKreiranja).Take(5))
                .Include(k => k.SesijeKaoKlijent.OrderByDescending(s => s.DatumSesije).Take(5))
                .FirstOrDefaultAsync(k => k.Id == klijentId);
        }

        public async Task<Korisnik> PostaviAktivnostNaloga(string korisnikId, bool aktivan)
        {
            var korisnik = await _context.Users.FirstOrDefaultAsync(k => k.Id == korisnikId);
            if (korisnik == null)
                throw new Exception("Korisnik nije pronađen");

            korisnik.AktivnaNalog = aktivan;
            await _context.SaveChangesAsync();
            return korisnik;
        }
    }
}