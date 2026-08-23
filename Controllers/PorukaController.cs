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
    public class PorukaController : ControllerBase
    {
        private readonly IPorukaRepository _porukaRepo;
        private readonly IMapper _mapper;

        public PorukaController(IPorukaRepository porukaRepo, IMapper mapper)
        {
            _porukaRepo = porukaRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PosaljiPoruku([FromBody] KreirajPorukuDTO dto)
        {
            try
            {
                var posiljaocId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruka = _mapper.Map<Poruka>(dto);
                poruka.PosiljaocId = posiljaocId;

                var rezultat = await _porukaRepo.PosaljiPoruku(poruka);
                var prikaz = _mapper.Map<PrikazPorukeDTO>(rezultat);

                return Ok(new { poruka = "Poruka uspešno poslata", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("primljene")]
        public async Task<IActionResult> GetPrimljenePoruke()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruke = await _porukaRepo.GetPrimljenePoruke(korisnikId);
                var prikaz = _mapper.Map<List<PrikazPorukeDTO>>(poruke);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("poslate")]
        public async Task<IActionResult> GetPoslatePoruke()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruke = await _porukaRepo.GetPoslatePoruke(korisnikId);
                var prikaz = _mapper.Map<List<PrikazPorukeDTO>>(poruke);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("konverzacija/{korisnikId}")]
        public async Task<IActionResult> GetKonverzacija(string korisnikId)
        {
            try
            {
                var trenutniKorisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruke = await _porukaRepo.GetKonverzacija(trenutniKorisnikId, korisnikId);
                var prikaz = _mapper.Map<List<PrikazPorukeDTO>>(poruke);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("sesija/{sesijaId}")]
        public async Task<IActionResult> GetPorukeSesije(int sesijaId)
        {
            try
            {
                var poruke = await _porukaRepo.GetPorukeSesije(sesijaId);
                var prikaz = _mapper.Map<List<PrikazPorukeDTO>>(poruke);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("neprocitane")]
        public async Task<IActionResult> GetNeprocitanePoruke()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruke = await _porukaRepo.GetNeprocitanePoruke(korisnikId);
                var brojNeprocitanih = await _porukaRepo.GetBrojNeprocitanihPoruka(korisnikId);
                var prikaz = _mapper.Map<List<PrikazPorukeDTO>>(poruke);

                return Ok(new { brojNeprocitanih, poruke = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{porukaId}/procitano")]
        public async Task<IActionResult> OznaciKaoProcitanu(int porukaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruka = await _porukaRepo.GetPoruka(porukaId);

                if (poruka == null)
                    return NotFound(new { poruka = "Poruka nije pronađena" });

                if (poruka.PrimalacId != korisnikId)
                    return Forbid();

                await _porukaRepo.OznaciKaoProcitanu(porukaId);
                return Ok(new { poruka = "Poruka označena kao pročitana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("oznaci-sve-procitano")]
        public async Task<IActionResult> OznaciSveKaoProcitane()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _porukaRepo.OznaciSveKaoProcitane(korisnikId);
                return Ok(new { poruka = "Sve poruke označene kao pročitane" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{porukaId}")]
        public async Task<IActionResult> ObrisiPoruku(int porukaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var poruka = await _porukaRepo.GetPoruka(porukaId);

                if (poruka == null)
                    return NotFound(new { poruka = "Poruka nije pronađena" });

                if (poruka.PosiljaocId != korisnikId && poruka.PrimalacId != korisnikId)
                    return Forbid();

                await _porukaRepo.ObrisiPoruku(porukaId);
                return Ok(new { poruka = "Poruka uspešno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}