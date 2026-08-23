using AutoMapper;
using MentalHealth.Data.Models;
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
    public class SesijaController : ControllerBase
    {
        private readonly ISesijaRepository _sesijaRepo;
        private readonly IMapper _mapper;

        public SesijaController(ISesijaRepository sesijaRepo, IMapper mapper)
        {
            _sesijaRepo = sesijaRepo;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> KreirajSesiju([FromBody] KreirajSesijuDTO dto)
        {
            try
            {
                var terapeutId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesija = _mapper.Map<Sesija>(dto);
                sesija.TerapeutId = terapeutId;

                var rezultat = await _sesijaRepo.KreirajSesiju(sesija);
                var prikaz = _mapper.Map<PrikazSesijeDTO>(rezultat);

                return Ok(new { poruka = "Sesija uspešno kreirana", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{sesijaId}")]
        public async Task<IActionResult> GetSesija(int sesijaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesija = await _sesijaRepo.GetSesija(sesijaId);

                if (sesija == null)
                    return NotFound(new { poruka = "Sesija nije pronađena" });

                // Provera da li je korisnik učesnik sesije
                if (sesija.KlijentId != korisnikId && sesija.TerapeutId != korisnikId)
                    return Forbid();

                var prikaz = _mapper.Map<PrikazSesijeDTO>(sesija);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("moje-kao-klijent")]
        public async Task<IActionResult> GetMojeSesijeKaoKlijent()
        {
            try
            {
                var klijentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesije = await _sesijaRepo.GetSesijeKlijenta(klijentId);
                var prikaz = _mapper.Map<List<PrikazSesijeDTO>>(sesije);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("moje-kao-terapeut")]
        [Authorize(Roles = "Terapeut")]
        public async Task<IActionResult> GetMojeSesijeKaoTerapeut()
        {
            try
            {
                var terapeutId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesije = await _sesijaRepo.GetSesijeTerapeuta(terapeutId);
                var prikaz = _mapper.Map<List<PrikazSesijeDTO>>(sesije);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("aktivne")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> GetAktivneSesije()
        {
            try
            {
                var sesije = await _sesijaRepo.GetAktivneSesije();
                var prikaz = _mapper.Map<List<PrikazSesijeDTO>>(sesije);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("danas")]
        public async Task<IActionResult> GetZakazaneSesijeZaDanas()
        {
            try
            {
                var sesije = await _sesijaRepo.GetZakazaneSesijeZaDanas();
                var prikaz = _mapper.Map<List<PrikazSesijeDTO>>(sesije);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{sesijaId}/status")]
        public async Task<IActionResult> AzurirajStatusSesije(int sesijaId, [FromBody] string status)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesija = await _sesijaRepo.GetSesija(sesijaId);

                if (sesija == null)
                    return NotFound(new { poruka = "Sesija nije pronađena" });

                // Samo terapeut može da menja status
                if (sesija.TerapeutId != korisnikId)
                    return Forbid();

                var rezultat = await _sesijaRepo.AzurirajStatusSesije(sesijaId, status);
                var prikaz = _mapper.Map<PrikazSesijeDTO>(rezultat);

                return Ok(new { poruka = "Status sesije uspešno ažuriran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{sesijaId}")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> AzurirajSesiju(int sesijaId, [FromBody] AzurirajSesijuDTO dto)
        {
            try
            {
                var sesija = _mapper.Map<Sesija>(dto);
                sesija.SesijaId = sesijaId;

                var rezultat = await _sesijaRepo.AzurirajSesiju(sesija);
                var prikaz = _mapper.Map<PrikazSesijeDTO>(rezultat);

                return Ok(new { poruka = "Sesija uspešno ažurirana", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{sesijaId}/beleske")]
        [Authorize(Roles = "Terapeut")]
        public async Task<IActionResult> DodajBeleskeTerapeuta(int sesijaId, [FromBody] string beleske)
        {
            try
            {
                var terapeutId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesija = await _sesijaRepo.GetSesija(sesijaId);

                if (sesija == null)
                    return NotFound(new { poruka = "Sesija nije pronađena" });

                if (sesija.TerapeutId != terapeutId)
                    return Forbid();

                await _sesijaRepo.DodajBeliskeTerapeuta(sesijaId, beleske);
                return Ok(new { poruka = "Beleške terapeuta uspešno sačuvane" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{sesijaId}/feedback")]
        public async Task<IActionResult> DodajFeedbackKlijenta(int sesijaId, [FromBody] string feedback)
        {
            try
            {
                var klijentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sesija = await _sesijaRepo.GetSesija(sesijaId);

                if (sesija == null)
                    return NotFound(new { poruka = "Sesija nije pronađena" });

                if (sesija.KlijentId != klijentId)
                    return Forbid();

                await _sesijaRepo.DodajFeedbackKlijenta(sesijaId, feedback);
                return Ok(new { poruka = "Feedback uspešno sačuvan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{sesijaId}")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> ObrisiSesiju(int sesijaId)
        {
            try
            {
                await _sesijaRepo.ObrisiSesiju(sesijaId);
                return Ok(new { poruka = "Sesija uspešno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}