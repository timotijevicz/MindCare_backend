using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MentalHealth.Data.Models;
using MentalHealth.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MentalHealth.Data;

namespace MentalHealthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
        }

        // POST: api/Auth/registracija
        [HttpPost("registracija")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join("; ", errors) });
            }

            try
            {
                // 1. Proveri da li korisnik već postoji
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    return BadRequest(new { message = "Korisnik sa ovim email-om već postoji." });

                // 2. Kreiraj korisnika
                var korisnik = new Korisnik
                {
                    Email = registerDto.Email,
                    UserName = registerDto.Email,
                    Ime = registerDto.Ime,
                    Prezime = registerDto.Prezime,
                    AnonimnaRegistracija = registerDto.AnonimnaRegistracija,
                    DatumKreiranja = DateTime.UtcNow,
                    ZadnjaAktivnost = DateTime.UtcNow,
                    AktivnaNalog = true
                };

                var createResult = await _userManager.CreateAsync(korisnik, registerDto.Lozinka);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Greška pri kreiranju naloga: {errors}" });
                }

                // 3. Dodeli ulogu
                string roleName = "Klijent";
                if (!string.IsNullOrEmpty(registerDto.TipKorisnika) &&
                    registerDto.TipKorisnika.Equals("Terapeut", StringComparison.OrdinalIgnoreCase))
                {
                    roleName = "Terapeut";
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                    await _roleManager.CreateAsync(new IdentityRole(roleName));

                await _userManager.AddToRoleAsync(korisnik, roleName);

                // 4. Ako je Terapeut, kreiraj dodatni profil
                if (roleName == "Terapeut")
                {
                    if (string.IsNullOrWhiteSpace(registerDto.Zvanje) ||
                        string.IsNullOrWhiteSpace(registerDto.Licenca) ||
                        !registerDto.SatnicaKonzultacije.HasValue)
                    {
                        return BadRequest(new { message = "Za terapeuta su obavezni: Zvanje, Licenca i Satnica" });
                    }

                    var terapeut = new Terapeut
                    {
                        KorisnikId = korisnik.Id,
                        Zvanje = registerDto.Zvanje,
                        Licenca = registerDto.Licenca,
                        OpisPrakse = registerDto.OpisPrakse ?? "",
                        SatnicaKonzultacije = registerDto.SatnicaKonzultacije.Value,
                        Dostupan = true,
                        DatumRegistracije = DateTime.UtcNow
                    };
                    _context.Terapeuti.Add(terapeut);
                    await _context.SaveChangesAsync();
                }

                // 5. Generiši odgovor
                var authResponse = new AuthResponseDTO
                {
                    KorisnikId = korisnik.Id,
                    Email = korisnik.Email,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Rola = roleName,
                    JeTerapeut = roleName == "Terapeut",
                    Token = await GenerateJwtToken(korisnik, roleName),
                    IstekTokena = DateTime.UtcNow.AddHours(8)
                };

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška na serveru: {ex.Message}" });
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join("; ", errors) });
            }

            try
            {
                var korisnik = await _userManager.FindByEmailAsync(loginDto.Email);
                if (korisnik == null)
                    return Unauthorized(new { message = "Pogrešan email ili lozinka." });

                var signInResult = await _signInManager.CheckPasswordSignInAsync(korisnik, loginDto.Lozinka, lockoutOnFailure: false);
                if (!signInResult.Succeeded)
                    return Unauthorized(new { message = "Pogrešan email ili lozinka." });

                if (!korisnik.AktivnaNalog)
                    return Unauthorized(new { message = "Ovaj nalog je deaktiviran. Obratite se administratoru." });

                // Ažuriraj poslednju aktivnost
                korisnik.ZadnjaAktivnost = DateTime.UtcNow;
                await _userManager.UpdateAsync(korisnik);

                // Dohvati ulogu
                var roles = await _userManager.GetRolesAsync(korisnik);
                var role = roles.FirstOrDefault() ?? "Klijent";

                // Generiši odgovor
                var authResponse = new AuthResponseDTO
                {
                    KorisnikId = korisnik.Id,
                    Email = korisnik.Email,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Rola = role,
                    JeTerapeut = role == "Terapeut",
                    Token = await GenerateJwtToken(korisnik, role),
                    IstekTokena = DateTime.UtcNow.AddHours(8)
                };

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška na serveru: {ex.Message}" });
            }
        }

        // POST: api/Auth/change-password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PromenaLozinkeDTO promenaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join("; ", errors) });
            }

            try
            {
                var korisnik = await _userManager.GetUserAsync(User);
                if (korisnik == null)
                    return Unauthorized(new { message = "Korisnik nije pronađen." });

                var result = await _userManager.ChangePasswordAsync(korisnik, promenaDto.TrenutnaLozinka, promenaDto.NovaLozinka);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Greška pri promeni lozinke: {errors}" });
                }

                return Ok(new { message = "Lozinka uspešno promenjena." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška na serveru: {ex.Message}" });
            }
        }

        // POST: api/Auth/reset-password-init
        [HttpPost("reset-password-init")]
        public async Task<IActionResult> InitiatePasswordReset([FromBody] ResetLozinkeDTO resetDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join("; ", errors) });
            }

            try
            {
                var korisnik = await _userManager.FindByEmailAsync(resetDto.Email);
                if (korisnik == null)
                    return Ok(new { message = "Ukoliko korisnik postoji, poslat ćemo link za reset." });

                var token = await _userManager.GeneratePasswordResetTokenAsync(korisnik);

                // Napomena: nema email servisa u ovom projektu, pa se token vraća direktno
                // u odgovoru umesto slanjem na email. U produkciji bi ovo išlo isključivo
                // na korisnikov email, nikad u API odgovoru.
                return Ok(new { message = "Token za reset lozinke je generisan.", token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška na serveru: {ex.Message}" });
            }
        }

        // POST: api/Auth/reset-password-confirm
        [HttpPost("reset-password-confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] PotvrdiResetLozinkeDTO potvrdaDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join("; ", errors) });
            }

            try
            {
                var korisnik = await _userManager.FindByEmailAsync(potvrdaDto.Email);
                if (korisnik == null)
                    return BadRequest(new { message = "Nevažeći zahtev za reset lozinke." });

                var result = await _userManager.ResetPasswordAsync(korisnik, potvrdaDto.Token, potvrdaDto.NovaLozinka);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Greška pri resetovanju lozinke: {errors}" });
                }

                return Ok(new { message = "Lozinka uspešno resetovana. Prijavite se novom lozinkom." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška na serveru: {ex.Message}" });
            }
        }

        // POST: api/Auth/obrisi-moj-nalog
        // GDPR pravo na brisanje: uklanja sav lični sadržaj korisnika (raspoloženja, dnevnik,
        // ciljeve, navike, SOS kontakte, podsetnike, profil) i anonimizuje identitet na nalogu.
        // Poruke/sesije sa terapeutom se ne brišu fizički (tuđi podaci), ali više ne nose
        // lične podatke jer se ime/email vezanog naloga anonimizuju.
        [Authorize]
        [HttpPost("obrisi-moj-nalog")]
        public async Task<IActionResult> ObrisiMojNalog()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var korisnik = await _userManager.FindByIdAsync(korisnikId);
                if (korisnik == null)
                    return NotFound(new { message = "Korisnik nije pronađen." });

                _context.DnevnaRaspolozenja.RemoveRange(
                    await _context.DnevnaRaspolozenja.Where(x => x.KorisnikId == korisnikId).ToListAsync());
                _context.DnevnikMisli.RemoveRange(
                    await _context.DnevnikMisli.Where(x => x.KorisnikId == korisnikId).ToListAsync());
                _context.SOSKontakti.RemoveRange(
                    await _context.SOSKontakti.Where(x => x.KorisnikId == korisnikId).ToListAsync());
                _context.Podsetnici.RemoveRange(
                    await _context.Podsetnici.Where(x => x.KorisnikId == korisnikId).ToListAsync());
                _context.Ciljevi.RemoveRange(
                    await _context.Ciljevi.Where(x => x.KorisnikId == korisnikId).ToListAsync());
                _context.Navike.RemoveRange(
                    await _context.Navike.Where(x => x.KorisnikId == korisnikId).ToListAsync());

                var profil = await _context.KorisnickiProfili.FirstOrDefaultAsync(p => p.KorisnikId == korisnikId);
                if (profil != null)
                    _context.KorisnickiProfili.Remove(profil);

                var terapeut = await _context.Terapeuti.FirstOrDefaultAsync(t => t.KorisnikId == korisnikId);
                if (terapeut != null)
                    _context.Terapeuti.Remove(terapeut);

                await _context.SaveChangesAsync();

                korisnik.Ime = "Obrisan";
                korisnik.Prezime = "korisnik";
                korisnik.Email = $"obrisan-{korisnik.Id}@obrisan.local";
                korisnik.UserName = korisnik.Email;
                korisnik.PhoneNumber = null;
                korisnik.AktivnaNalog = false;
                await _userManager.UpdateNormalizedEmailAsync(korisnik);
                await _userManager.UpdateNormalizedUserNameAsync(korisnik);
                await _userManager.UpdateAsync(korisnik);

                // Onemogući dalju prijavu postavljanjem lozinke koju korisnik ne zna
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(korisnik);
                await _userManager.ResetPasswordAsync(korisnik, resetToken, Guid.NewGuid().ToString("N") + "Aa1!");

                return Ok(new { message = "Nalog i lični podaci su trajno obrisani." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Greška na serveru: {ex.Message}" });
            }
        }

        // POMOĆNA METODA ZA GENERISANJE JWT TOKENA
        private async Task<string> GenerateJwtToken(Korisnik korisnik, string role)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT ključ nije konfigurisan.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, korisnik.Id),
                new Claim(JwtRegisteredClaimNames.Email, korisnik.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, korisnik.Ime ?? ""),
                new Claim(JwtRegisteredClaimNames.FamilyName, korisnik.Prezime ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
