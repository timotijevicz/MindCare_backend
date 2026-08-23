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
    public class NavikaController : ControllerBase
    {
        private readonly INavikaRepository _navikaRepo;
        private readonly IMapper _mapper;

        public NavikaController(INavikaRepository navikaRepo, IMapper mapper)
        {
            _navikaRepo = navikaRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajNaviku([FromBody] KreirajNavikuDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = _mapper.Map<Navika>(dto);
                navika.KorisnikId = korisnikId;

                var rezultat = await _navikaRepo.KreirajNaviku(navika);
                var prikaz = _mapper.Map<PrikazNavikeDTO>(rezultat);

                return Ok(new { poruka = "Navika uspešno kreirana", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMojeNavike()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navike = await _navikaRepo.GetNavikeKorisnika(korisnikId);
                var prikaz = _mapper.Map<List<PrikazNavikeDTO>>(navike);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("aktivne")]
        public async Task<IActionResult> GetAktivneNavike()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navike = await _navikaRepo.GetAktivneNavike(korisnikId);
                var prikaz = _mapper.Map<List<PrikazNavikeDTO>>(navike);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{navikaId}")]
        public async Task<IActionResult> GetNavika(int navikaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                var prikaz = _mapper.Map<PrikazNavikeDTO>(navika);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{navikaId}")]
        public async Task<IActionResult> AzurirajNaviku(int navikaId, [FromBody] KreirajNavikuDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                var azuriranaNavika = _mapper.Map<Navika>(dto);
                azuriranaNavika.NavikaId = navikaId;

                var rezultat = await _navikaRepo.AzurirajNaviku(azuriranaNavika);
                var prikaz = _mapper.Map<PrikazNavikeDTO>(rezultat);

                return Ok(new { poruka = "Navika uspešno ažurirana", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{navikaId}/aktiviraj")]
        public async Task<IActionResult> AktivirajNaviku(int navikaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                await _navikaRepo.AktivirajNaviku(navikaId);
                return Ok(new { poruka = "Navika uspešno aktivirana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{navikaId}/deaktiviraj")]
        public async Task<IActionResult> DeaktivirajNaviku(int navikaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                await _navikaRepo.DeaktivirajNaviku(navikaId);
                return Ok(new { poruka = "Navika uspešno deaktivirana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{navikaId}/pracenje")]
        public async Task<IActionResult> ZabeleziPracenje(int navikaId, [FromBody] KreirajPracenjeNavikeDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                var pracenje = _mapper.Map<PracenjeNavike>(dto);
                pracenje.NavikaId = navikaId;

                var rezultat = await _navikaRepo.ZabeleziPracenje(pracenje);
                var prikaz = _mapper.Map<PrikazPracenjaNavikeDTO>(rezultat);

                return Ok(new { poruka = "Praćenje uspešno zabeleženo", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{navikaId}/pracenja")]
        public async Task<IActionResult> GetPracenjaNavike(int navikaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                var pracenja = await _navikaRepo.GetPracenjaNavike(navikaId);
                var prikaz = _mapper.Map<List<PrikazPracenjaNavikeDTO>>(pracenja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{navikaId}/pracenja/period")]
        public async Task<IActionResult> GetPracenjaZaPeriod(int navikaId, [FromQuery] DateTime od, [FromQuery] DateTime dok)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                var pracenja = await _navikaRepo.GetPracenjaZaPeriod(navikaId, od, dok);
                var prikaz = _mapper.Map<List<PrikazPracenjaNavikeDTO>>(pracenja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{navikaId}/statistika")]
        public async Task<IActionResult> GetStatistikaNavike(int navikaId, [FromQuery] int brojDana = 30)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                var procenat = await _navikaRepo.GetProcenatIspunjavanja(navikaId, brojDana);

                return Ok(new { navikaId, procenatIspunjavanja = procenat, periodDana = brojDana });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{navikaId}")]
        public async Task<IActionResult> ObrisiNaviku(int navikaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var navika = await _navikaRepo.GetNavika(navikaId);

                if (navika == null)
                    return NotFound(new { poruka = "Navika nije pronađena" });

                if (navika.KorisnikId != korisnikId)
                    return Forbid();

                await _navikaRepo.ObrisiNaviku(navikaId);
                return Ok(new { poruka = "Navika uspešno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}