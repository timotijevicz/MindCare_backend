using MentalHealth.Data;
using MentalHealth.Data.Models;
using MentalHealth.Interfejsi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MentalHealth.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly AppDbContext _context;

        public AuthRepository(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<Korisnik> Registracija(Korisnik korisnik, string lozinka)
        {
            // Provera da li email već postoji
            var postojeciKorisnik = await _userManager.FindByEmailAsync(korisnik.Email);
            if (postojeciKorisnik != null)
            {
                throw new Exception("Korisnik sa ovim emailom već postoji");
            }

            // Kreiranje korisnika
            var rezultat = await _userManager.CreateAsync(korisnik, lozinka);
            if (!rezultat.Succeeded)
            {
                var greske = string.Join(", ", rezultat.Errors.Select(e => e.Description));
                throw new Exception($"Greška pri registraciji: {greske}");
            }

            // Kreiranje profila za svakog korisnika
            var profil = new KorisnickiProfil
            {
                KorisnikId = korisnik.Id,
                PrimiMotivacionuPoruku = true,
                PrimiPodsetnik = true,
                DatumAzuriranja = DateTime.UtcNow
            };
            _context.KorisnickiProfili.Add(profil);
            await _context.SaveChangesAsync();

            return korisnik;
        }

        public async Task<Korisnik> Prijava(string email, string lozinka)
        {
            var korisnik = await _userManager.FindByEmailAsync(email);
            if (korisnik == null)
            {
                throw new Exception("Pogrešan email ili lozinka");
            }

            if (!korisnik.AktivnaNalog)
            {
                throw new Exception("Nalog je deaktiviran. Kontaktirajte administratora.");
            }

            var rezultat = await _signInManager.CheckPasswordSignInAsync(korisnik, lozinka, false);
            if (!rezultat.Succeeded)
            {
                throw new Exception("Pogrešan email ili lozinka");
            }

            // Ažuriranje poslednje aktivnosti
            korisnik.ZadnjaAktivnost = DateTime.UtcNow;
            await _userManager.UpdateAsync(korisnik);

            return korisnik;
        }

        public async Task<bool> ProveriLozinku(Korisnik korisnik, string lozinka)
        {
            return await _userManager.CheckPasswordAsync(korisnik, lozinka);
        }

        public async Task<IList<string>> GetUloge(Korisnik korisnik)
        {
            return await _userManager.GetRolesAsync(korisnik);
        }

        public async Task DodajUlogu(Korisnik korisnik, string uloga)
        {
            if (!await _userManager.IsInRoleAsync(korisnik, uloga))
            {
                await _userManager.AddToRoleAsync(korisnik, uloga);
            }
        }

        public async Task<Korisnik> GetKorisnikPoEmailu(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Korisnik> GetKorisnikPoId(string korisnikId)
        {
            return await _userManager.FindByIdAsync(korisnikId);
        }

        public async Task AzurirajKorisnika(Korisnik korisnik)
        {
            await _userManager.UpdateAsync(korisnik);
        }

        public async Task<bool> DeaktivirajNalog(string korisnikId)
        {
            var korisnik = await _userManager.FindByIdAsync(korisnikId);
            if (korisnik == null)
                return false;

            korisnik.AktivnaNalog = false;
            var rezultat = await _userManager.UpdateAsync(korisnik);
            return rezultat.Succeeded;
        }

        public async Task PromeniLozinku(Korisnik korisnik, string trenutnaLozinka, string novaLozinka)
        {
            var rezultat = await _userManager.ChangePasswordAsync(korisnik, trenutnaLozinka, novaLozinka);
            if (!rezultat.Succeeded)
            {
                var greske = string.Join(", ", rezultat.Errors.Select(e => e.Description));
                throw new Exception($"Greška pri promeni lozinke: {greske}");
            }
        }
    }
}