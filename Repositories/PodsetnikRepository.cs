using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class PodsetnikRepository : IPodsetnikRepository
    {
        private readonly AppDbContext _context;

        // Indeks odgovara DayOfWeek enumu (Nedelja=0 ... Subota=6) — ista poruka
        // važi ceo dan i menja se sledećeg dana, umesto da bude nasumična.
        private readonly string[] _motivacionePorukePoDanu = new[]
        {
            "Nedelja je za predah — ne moraš danas ništa da dokazuješ. 🌤️",
            "Nova nedelja, novi početak. Kreni polako, korak po korak. 🌱",
            "Ti si jači/a nego što misliš. Veruj u sebe! 💪",
            "Pola nedelje je iza tebe — to zaslužuje pohvalu. 🏆",
            "Male promene vode do velikih rezultata. Nastavi tako. 🌟",
            "Diši duboko. Uspeo/la si i ovu nedelju. 🧘‍♂️",
            "Danas je dan za tebe. Briga o sebi nije sebičnost. ❤️"
        };

        public PodsetnikRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Podsetnik> KreirajPodsetnik(Podsetnik podsetnik)
        {
            // Provera da li korisnik postoji
            var korisnik = await _context.Users.FindAsync(podsetnik.KorisnikId);
            if (korisnik == null)
                throw new Exception("Korisnik nije pronađen");

            // Validacija vremena
            if (!TimeSpan.TryParse(podsetnik.VremePodsetnika, out _))
                throw new Exception("Format vremena nije validan. Koristite HH:mm format");

            podsetnik.Aktivan = true;
            podsetnik.DatumKreiranja = DateTime.UtcNow;

            _context.Podsetnici.Add(podsetnik);
            await _context.SaveChangesAsync();

            return podsetnik;
        }

        public async Task<Podsetnik> GetPodsetnik(int podsetnikId)
        {
            return await _context.Podsetnici
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(p => p.PodsetnikId == podsetnikId);
        }

        public async Task<List<Podsetnik>> GetPodsetniciKorisnika(string korisnikId)
        {
            return await _context.Podsetnici
                .Where(p => p.KorisnikId == korisnikId)
                .OrderBy(p => p.VremePodsetnika)
                .ToListAsync();
        }

        public async Task<List<Podsetnik>> GetAktivniPodsetnici(string korisnikId)
        {
            return await _context.Podsetnici
                .Where(p => p.KorisnikId == korisnikId && p.Aktivan)
                .OrderBy(p => p.VremePodsetnika)
                .ToListAsync();
        }

        public async Task<List<Podsetnik>> GetPodsetniciPoTipu(string korisnikId, string tip)
        {
            return await _context.Podsetnici
                .Where(p => p.KorisnikId == korisnikId && p.Tip == tip && p.Aktivan)
                .OrderBy(p => p.VremePodsetnika)
                .ToListAsync();
        }

        public async Task<Podsetnik> AzurirajPodsetnik(Podsetnik podsetnik)
        {
            var postojeci = await _context.Podsetnici.FindAsync(podsetnik.PodsetnikId);
            if (postojeci == null)
                throw new Exception("Podsetnik nije pronađen");

            postojeci.Tip = podsetnik.Tip ?? postojeci.Tip;
            postojeci.Tekst = podsetnik.Tekst ?? postojeci.Tekst;
            postojeci.Aktivan = podsetnik.Aktivan;
            postojeci.VremePodsetnika = podsetnik.VremePodsetnika ?? postojeci.VremePodsetnika;

            await _context.SaveChangesAsync();
            return postojeci;
        }

        public async Task AktivirajPodsetnik(int podsetnikId)
        {
            var podsetnik = await _context.Podsetnici.FindAsync(podsetnikId);
            if (podsetnik != null)
            {
                podsetnik.Aktivan = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeaktivirajPodsetnik(int podsetnikId)
        {
            var podsetnik = await _context.Podsetnici.FindAsync(podsetnikId);
            if (podsetnik != null)
            {
                podsetnik.Aktivan = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ObrisiPodsetnik(int podsetnikId)
        {
            var podsetnik = await _context.Podsetnici.FindAsync(podsetnikId);
            if (podsetnik != null)
            {
                _context.Podsetnici.Remove(podsetnik);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Podsetnik>> GetPodsetniciZaSlanje(string trenutnoVreme)
        {
            return await _context.Podsetnici
                .Include(p => p.Korisnik)
                .Where(p => p.Aktivan &&
                           p.VremePodsetnika == trenutnoVreme &&
                           p.Korisnik.AktivnaNalog)
                .ToListAsync();
        }

        public async Task<string> GenerisiMotivacionuPoruku()
        {
            var indeks = (int)DateTime.UtcNow.DayOfWeek;
            return await Task.FromResult(_motivacionePorukePoDanu[indeks]);
        }
    }
}