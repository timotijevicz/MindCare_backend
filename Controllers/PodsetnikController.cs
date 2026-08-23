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
    public class PodsetnikController : ControllerBase
    {
        private readonly IPodsetnikRepository _podsetnikRepo;
        private readonly IMapper _mapper;

        public PodsetnikController(IPodsetnikRepository podsetnikRepo, IMapper mapper)
        {
            _podsetnikRepo = podsetnikRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajPodsetnik([FromBody] KreirajPodsetnikDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnik = _mapper.Map<Podsetnik>(dto);
                podsetnik.KorisnikId = korisnikId;

                var rezultat = await _podsetnikRepo.KreirajPodsetnik(podsetnik);
                var prikaz = _mapper.Map<PrikazPodsetnikaDTO>(rezultat);

                return Ok(new { poruka = "Podsetnik uspešno kreiran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMojiPodsetnici()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnici = await _podsetnikRepo.GetPodsetniciKorisnika(korisnikId);
                var prikaz = _mapper.Map<List<PrikazPodsetnikaDTO>>(podsetnici);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("aktivni")]
        public async Task<IActionResult> GetAktivniPodsetnici()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnici = await _podsetnikRepo.GetAktivniPodsetnici(korisnikId);
                var prikaz = _mapper.Map<List<PrikazPodsetnikaDTO>>(podsetnici);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("tip/{tip}")]
        public async Task<IActionResult> GetPodsetniciPoTipu(string tip)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnici = await _podsetnikRepo.GetPodsetniciPoTipu(korisnikId, tip);
                var prikaz = _mapper.Map<List<PrikazPodsetnikaDTO>>(podsetnici);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("motivaciona-poruka")]
        public async Task<IActionResult> GetMotivacionaPoruka()
        {
            try
            {
                var poruka = await _podsetnikRepo.GenerisiMotivacionuPoruku();
                return Ok(new { poruka });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{podsetnikId}")]
        public async Task<IActionResult> AzurirajPodsetnik(int podsetnikId, [FromBody] AzurirajPodsetnikDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnik = await _podsetnikRepo.GetPodsetnik(podsetnikId);

                if (podsetnik == null)
                    return NotFound(new { poruka = "Podsetnik nije pronađen" });

                if (podsetnik.KorisnikId != korisnikId)
                    return Forbid();

                var azuriraniPodsetnik = _mapper.Map<Podsetnik>(dto);
                azuriraniPodsetnik.PodsetnikId = podsetnikId;

                var rezultat = await _podsetnikRepo.AzurirajPodsetnik(azuriraniPodsetnik);
                var prikaz = _mapper.Map<PrikazPodsetnikaDTO>(rezultat);

                return Ok(new { poruka = "Podsetnik uspešno ažuriran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{podsetnikId}/aktiviraj")]
        public async Task<IActionResult> AktivirajPodsetnik(int podsetnikId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnik = await _podsetnikRepo.GetPodsetnik(podsetnikId);

                if (podsetnik == null)
                    return NotFound(new { poruka = "Podsetnik nije pronađen" });

                if (podsetnik.KorisnikId != korisnikId)
                    return Forbid();

                await _podsetnikRepo.AktivirajPodsetnik(podsetnikId);
                return Ok(new { poruka = "Podsetnik uspešno aktiviran" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{podsetnikId}/deaktiviraj")]
        public async Task<IActionResult> DeaktivirajPodsetnik(int podsetnikId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnik = await _podsetnikRepo.GetPodsetnik(podsetnikId);

                if (podsetnik == null)
                    return NotFound(new { poruka = "Podsetnik nije pronađen" });

                if (podsetnik.KorisnikId != korisnikId)
                    return Forbid();

                await _podsetnikRepo.DeaktivirajPodsetnik(podsetnikId);
                return Ok(new { poruka = "Podsetnik uspešno deaktiviran" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{podsetnikId}")]
        public async Task<IActionResult> ObrisiPodsetnik(int podsetnikId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var podsetnik = await _podsetnikRepo.GetPodsetnik(podsetnikId);

                if (podsetnik == null)
                    return NotFound(new { poruka = "Podsetnik nije pronađen" });

                if (podsetnik.KorisnikId != korisnikId)
                    return Forbid();

                await _podsetnikRepo.ObrisiPodsetnik(podsetnikId);
                return Ok(new { poruka = "Podsetnik uspešno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}