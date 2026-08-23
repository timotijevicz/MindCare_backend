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
    public class CiljController : ControllerBase
    {
        private readonly ICiljRepository _ciljRepo;
        private readonly IMapper _mapper;

        public CiljController(ICiljRepository ciljRepo, IMapper mapper)
        {
            _ciljRepo = ciljRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajCilj([FromBody] KreirajCiljDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cilj = _mapper.Map<Cilj>(dto);
                cilj.KorisnikId = korisnikId;

                var rezultat = await _ciljRepo.KreirajCilj(cilj);
                var prikaz = _mapper.Map<PrikazCiljaDTO>(rezultat);

                return Ok(new { poruka = "Cilj uspešno kreiran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMojiCiljevi()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var ciljevi = await _ciljRepo.GetCiljeviKorisnika(korisnikId);
                var prikaz = _mapper.Map<List<PrikazCiljaDTO>>(ciljevi);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("aktivni")]
        public async Task<IActionResult> GetAktivniCiljevi()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var ciljevi = await _ciljRepo.GetAktivniCiljevi(korisnikId);
                var prikaz = _mapper.Map<List<PrikazCiljaDTO>>(ciljevi);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{ciljId}")]
        public async Task<IActionResult> GetCilj(int ciljId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cilj = await _ciljRepo.GetCilj(ciljId);

                if (cilj == null)
                    return NotFound(new { poruka = "Cilj nije pronađen" });

                if (cilj.KorisnikId != korisnikId)
                    return Forbid();

                var prikaz = _mapper.Map<PrikazCiljaDTO>(cilj);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{ciljId}")]
        public async Task<IActionResult> AzurirajCilj(int ciljId, [FromBody] KreirajCiljDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cilj = await _ciljRepo.GetCilj(ciljId);

                if (cilj == null)
                    return NotFound(new { poruka = "Cilj nije pronađen" });

                if (cilj.KorisnikId != korisnikId)
                    return Forbid();

                var azuriraniCilj = _mapper.Map<Cilj>(dto);
                azuriraniCilj.CiljId = ciljId;

                var rezultat = await _ciljRepo.AzurirajCilj(azuriraniCilj);
                var prikaz = _mapper.Map<PrikazCiljaDTO>(rezultat);

                return Ok(new { poruka = "Cilj uspešno ažuriran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{ciljId}/koraci")]
        public async Task<IActionResult> DodajKorak(int ciljId, [FromBody] KreirajKorakCiljaDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cilj = await _ciljRepo.GetCilj(ciljId);

                if (cilj == null)
                    return NotFound(new { poruka = "Cilj nije pronađen" });

                if (cilj.KorisnikId != korisnikId)
                    return Forbid();

                var korak = _mapper.Map<KorakCilja>(dto);
                korak.CiljId = ciljId;

                var rezultat = await _ciljRepo.DodajKorak(korak);
                var prikaz = _mapper.Map<PrikazKorakaCiljaDTO>(rezultat);

                return Ok(new { poruka = "Korak uspešno dodat", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("koraci/{korakId}/zavrsi")]
        public async Task<IActionResult> ZavrsiKorak(int korakId)
        {
            try
            {
                var rezultat = await _ciljRepo.ZavrsiKorak(korakId);
                var prikaz = _mapper.Map<PrikazKorakaCiljaDTO>(rezultat);

                return Ok(new { poruka = "Korak uspešno završen", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{ciljId}/koraci")]
        public async Task<IActionResult> GetKoraciCilja(int ciljId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cilj = await _ciljRepo.GetCilj(ciljId);

                if (cilj == null)
                    return NotFound(new { poruka = "Cilj nije pronađen" });

                if (cilj.KorisnikId != korisnikId)
                    return Forbid();

                var koraci = await _ciljRepo.GetKoraciCilja(ciljId);
                var prikaz = _mapper.Map<List<PrikazKorakaCiljaDTO>>(koraci);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{ciljId}")]
        public async Task<IActionResult> ObrisiCilj(int ciljId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cilj = await _ciljRepo.GetCilj(ciljId);

                if (cilj == null)
                    return NotFound(new { poruka = "Cilj nije pronađen" });

                if (cilj.KorisnikId != korisnikId)
                    return Forbid();

                await _ciljRepo.ObrisiCilj(ciljId);
                return Ok(new { poruka = "Cilj uspešno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}