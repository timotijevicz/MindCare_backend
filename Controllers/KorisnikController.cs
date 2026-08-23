using AutoMapper;
using MentalHealth.DTOs;
using MentalHealth.Interfejsi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MentalHealth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KorisnikController : ControllerBase
    {
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly IMapper _mapper;

        public KorisnikController(IKorisnikRepository korisnikRepo, IMapper mapper)
        {
            _korisnikRepo = korisnikRepo;
            _mapper = mapper;
        }

        [HttpGet("profil")]
        public async Task<IActionResult> GetProfil()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var profil = await _korisnikRepo.GetProfil(korisnikId);

                if (profil == null)
                    return NotFound(new { poruka = "Profil nije pronađen" });

                var profilDTO = _mapper.Map<AzurirajProfilDTO>(profil);
                return Ok(profilDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("profil")]
        public async Task<IActionResult> AzurirajProfil([FromBody] AzurirajProfilDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var profil = _mapper.Map<Data.Models.KorisnickiProfil>(dto);

                var azuriranProfil = await _korisnikRepo.AzurirajProfil(korisnikId, profil);
                var profilDTO = _mapper.Map<AzurirajProfilDTO>(azuriranProfil);

                return Ok(new { poruka = "Profil uspešno ažuriran", podaci = profilDTO });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("terapeuti")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTerapeuti()
        {
            try
            {
                var terapeuti = await _korisnikRepo.GetDostupniTerapeuti();
                var terapeutiDTO = _mapper.Map<List<TerapeutPrikazDTO>>(terapeuti);
                return Ok(terapeutiDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("terapeuti/moji")]
        [Authorize(Roles = "Klijent")]
        public async Task<IActionResult> GetTerapeutiZaKlijenta()
        {
            try
            {
                var klijentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var terapeuti = await _korisnikRepo.GetTerapeutiZaKlijenta(klijentId);
                var terapeutiDTO = _mapper.Map<List<TerapeutPrikazDTO>>(terapeuti);
                return Ok(terapeutiDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("terapeuti/svi")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetSviTerapeuti()
        {
            try
            {
                var terapeuti = await _korisnikRepo.GetSviTerapeuti();
                var terapeutiDTO = _mapper.Map<List<TerapeutPrikazDTO>>(terapeuti);
                return Ok(terapeutiDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("terapeuti/{terapeutId}")]
        public async Task<IActionResult> GetTerapeutDetalji(int terapeutId)
        {
            try
            {
                var terapeut = await _korisnikRepo.GetTerapeut(terapeutId);
                if (terapeut == null)
                    return NotFound(new { poruka = "Terapeut nije pronađen" });

                var terapeutDTO = _mapper.Map<TerapeutPrikazDTO>(terapeut);
                return Ok(terapeutDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("terapeuti/moj-profil")]
        [Authorize(Roles = "Terapeut")]
        public async Task<IActionResult> GetMojTerapeutProfil()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var terapeut = await _korisnikRepo.GetTerapeutPoKorisnikId(korisnikId);

                if (terapeut == null)
                    return NotFound(new { poruka = "Terapeut profil nije pronađen" });

                var terapeutDTO = _mapper.Map<TerapeutPrikazDTO>(terapeut);
                return Ok(terapeutDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("terapeuti/moj-profil")]
        [Authorize(Roles = "Terapeut")]
        public async Task<IActionResult> AzurirajTerapeutProfil([FromBody] AzurirajTerapeutaDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var terapeut = await _korisnikRepo.GetTerapeutPoKorisnikId(korisnikId);

                if (terapeut == null)
                    return NotFound(new { poruka = "Terapeut profil nije pronađen" });

                var azuriraniTerapeut = _mapper.Map<Data.Models.Terapeut>(dto);
                var rezultat = await _korisnikRepo.AzurirajTerapeuta(terapeut.TerapeutId, azuriraniTerapeut);
                var terapeutDTO = _mapper.Map<TerapeutPrikazDTO>(rezultat);

                return Ok(new { poruka = "Profil terapeuta uspešno ažuriran", podaci = terapeutDTO });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("klijenti")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> GetKlijenti()
        {
            try
            {
                List<Data.Models.Korisnik> klijenti;

                if (User.IsInRole("Administrator"))
                {
                    klijenti = await _korisnikRepo.GetSviKlijenti();
                }
                else
                {
                    var terapeutKorisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    klijenti = await _korisnikRepo.GetKlijentiZaTerapeuta(terapeutKorisnikId);
                }

                var klijentiDTO = _mapper.Map<List<KorisnikPrikazDTO>>(klijenti);
                return Ok(klijentiDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("klijenti/{klijentId}")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> GetKlijentDetalji(string klijentId)
        {
            try
            {
                var klijent = await _korisnikRepo.GetKlijentDetalji(klijentId);
                if (klijent == null)
                    return NotFound(new { poruka = "Klijent nije pronađen" });

                var klijentDTO = _mapper.Map<KorisnikPrikazDTO>(klijent);
                return Ok(klijentDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{korisnikId}/aktivnost")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PostaviAktivnostNaloga(string korisnikId, [FromBody] AktivnostNalogaDTO dto)
        {
            try
            {
                var korisnik = await _korisnikRepo.PostaviAktivnostNaloga(korisnikId, dto.Aktivan);
                return Ok(new { poruka = dto.Aktivan ? "Nalog je aktiviran." : "Nalog je deaktiviran.", aktivan = korisnik.AktivnaNalog });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}