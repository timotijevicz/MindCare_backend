using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace MentalHealth.Repository
{
    public class PorukaRepository : IPorukaRepository
    {
        private readonly AppDbContext _context;
        private readonly byte[] _encryptionKey;

        public PorukaRepository(AppDbContext context)
        {
            _context = context;
            // U produkciji, ključ bi trebalo da bude u Azure Key Vault ili slično
            _encryptionKey = SHA256.HashData(Encoding.UTF8.GetBytes("MentalHealthApp2024SecretKey123456789!"));
        }

        public async Task<Poruka> PosaljiPoruku(Poruka poruka)
        {
            // Provera da li postoji primalac
            var primalac = await _context.Users.FindAsync(poruka.PrimalacId);
            if (primalac == null)
                throw new Exception("Primalac nije pronađen");

            // Ako je poruka vezana za sesiju, proveri da li sesija postoji
            if (poruka.SesijaId.HasValue)
            {
                var sesija = await _context.Sesije.FindAsync(poruka.SesijaId.Value);
                if (sesija == null)
                    throw new Exception("Sesija nije pronađena");

                // Provera da li su korisnici deo sesije
                if (sesija.KlijentId != poruka.PosiljaocId &&
                    sesija.KlijentId != poruka.PrimalacId &&
                    sesija.TerapeutId != poruka.PosiljaocId &&
                    sesija.TerapeutId != poruka.PrimalacId)
                {
                    throw new Exception("Korisnici nisu deo ove sesije");
                }
            }

            // Šifrovanje poruke
            poruka.SadrzajSifrovane = await SifrujPoruku(poruka.Sadrzaj);
            poruka.DatumSlanja = DateTime.UtcNow;
            poruka.Procitana = false;

            _context.Poruke.Add(poruka);
            await _context.SaveChangesAsync();
            return poruka;
        }

        public async Task<Poruka> GetPoruka(int porukaId)
        {
            var poruka = await _context.Poruke
                .Include(p => p.Posiljaoc)
                .Include(p => p.Primalac)
                .FirstOrDefaultAsync(p => p.PorukaId == porukaId);

            if (poruka != null && !string.IsNullOrEmpty(poruka.SadrzajSifrovane))
            {
                poruka.Sadrzaj = await DesifrujPoruku(poruka.SadrzajSifrovane);
            }

            return poruka;
        }

        public async Task<List<Poruka>> GetPrimljenePoruke(string korisnikId)
        {
            var poruke = await _context.Poruke
                .Include(p => p.Posiljaoc)
                .Where(p => p.PrimalacId == korisnikId)
                .OrderByDescending(p => p.DatumSlanja)
                .ToListAsync();

            // Dešifrovanje poruka
            foreach (var poruka in poruke)
            {
                if (!string.IsNullOrEmpty(poruka.SadrzajSifrovane))
                {
                    poruka.Sadrzaj = await DesifrujPoruku(poruka.SadrzajSifrovane);
                }
            }

            return poruke;
        }

        public async Task<List<Poruka>> GetPoslatePoruke(string korisnikId)
        {
            return await _context.Poruke
                .Include(p => p.Primalac)
                .Where(p => p.PosiljaocId == korisnikId)
                .OrderByDescending(p => p.DatumSlanja)
                .ToListAsync();
        }

        public async Task<List<Poruka>> GetKonverzacija(string korisnik1Id, string korisnik2Id)
        {
            var poruke = await _context.Poruke
                .Include(p => p.Posiljaoc)
                .Include(p => p.Primalac)
                .Where(p => (p.PosiljaocId == korisnik1Id && p.PrimalacId == korisnik2Id) ||
                           (p.PosiljaocId == korisnik2Id && p.PrimalacId == korisnik1Id))
                .OrderBy(p => p.DatumSlanja)
                .ToListAsync();

            // Dešifrovanje poruka
            foreach (var poruka in poruke)
            {
                if (!string.IsNullOrEmpty(poruka.SadrzajSifrovane))
                {
                    poruka.Sadrzaj = await DesifrujPoruku(poruka.SadrzajSifrovane);
                }
            }

            return poruke;
        }

        public async Task<List<Poruka>> GetPorukeSesije(int sesijaId)
        {
            return await _context.Poruke
                .Include(p => p.Posiljaoc)
                .Where(p => p.SesijaId == sesijaId)
                .OrderBy(p => p.DatumSlanja)
                .ToListAsync();
        }

        public async Task<List<Poruka>> GetNeprocitanePoruke(string korisnikId)
        {
            return await _context.Poruke
                .Include(p => p.Posiljaoc)
                .Where(p => p.PrimalacId == korisnikId && !p.Procitana)
                .OrderByDescending(p => p.DatumSlanja)
                .ToListAsync();
        }

        public async Task<int> GetBrojNeprocitanihPoruka(string korisnikId)
        {
            return await _context.Poruke
                .CountAsync(p => p.PrimalacId == korisnikId && !p.Procitana);
        }

        public async Task OznaciKaoProcitanu(int porukaId)
        {
            var poruka = await _context.Poruke.FindAsync(porukaId);
            if (poruka != null && !poruka.Procitana)
            {
                poruka.Procitana = true;
                poruka.DatumCitanja = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task OznaciSveKaoProcitane(string korisnikId)
        {
            var neprocitane = await _context.Poruke
                .Where(p => p.PrimalacId == korisnikId && !p.Procitana)
                .ToListAsync();

            foreach (var poruka in neprocitane)
            {
                poruka.Procitana = true;
                poruka.DatumCitanja = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ObrisiPoruku(int porukaId)
        {
            var poruka = await _context.Poruke.FindAsync(porukaId);
            if (poruka != null)
            {
                _context.Poruke.Remove(poruka);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> SifrujPoruku(string sadrzaj)
        {
            if (string.IsNullOrEmpty(sadrzaj))
                return sadrzaj;

            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write, leaveOpen: true))
            using (var sw = new StreamWriter(cs))
            {
                await sw.WriteAsync(sadrzaj);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public async Task<string> DesifrujPoruku(string sifrovaniSadrzaj)
        {
            if (string.IsNullOrEmpty(sifrovaniSadrzaj))
                return sifrovaniSadrzaj;

            using var aes = Aes.Create();
            aes.Key = _encryptionKey;

            var svePodatke = Convert.FromBase64String(sifrovaniSadrzaj);
            var iv = svePodatke.Take(16).ToArray();
            var sifrovaniBajtovi = svePodatke.Skip(16).ToArray();

            var decryptor = aes.CreateDecryptor(aes.Key, iv);
            using var ms = new MemoryStream(sifrovaniBajtovi);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return await sr.ReadToEndAsync();
        }
    }
}