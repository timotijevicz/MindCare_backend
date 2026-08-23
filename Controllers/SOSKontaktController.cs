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
    public class SOSKontaktController : ControllerBase
    {
        private readonly ISOSKontaktRepository _sosKontaktRepo;
        private readonly IMapper _mapper;

        public SOSKontaktController(ISOSKontaktRepository sosKontaktRepo, IMapper mapper)
        {
            _sosKontaktRepo = sosKontaktRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajSOSKontakt([FromBody] KreirajSOSKontaktDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakt = _mapper.Map<SOSKontakt>(dto);
                kontakt.KorisnikId = korisnikId;

                var rezultat = await _sosKontaktRepo.KreirajSOSKontakt(kontakt);
                var prikaz = _mapper.Map<PrikazSOSKontaktaDTO>(rezultat);

                return Ok(new { poruka = "SOS kontakt uspešno kreiran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMojiSOSKontakti()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakti = await _sosKontaktRepo.GetSOSKontaktiKorisnika(korisnikId);
                var prikaz = _mapper.Map<List<PrikazSOSKontaktaDTO>>(kontakti);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("aktivni")]
        public async Task<IActionResult> GetAktivniSOSKontakti()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakti = await _sosKontaktRepo.GetAktivniSOSKontakti(korisnikId);
                var prikaz = _mapper.Map<List<PrikazSOSKontaktaDTO>>(kontakti);

                return Ok(new { poruka = "Lista aktivnih SOS kontakata", kontakti = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{kontaktId}")]
        public async Task<IActionResult> GetSOSKontakt(int kontaktId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakt = await _sosKontaktRepo.GetSOSKontakt(kontaktId);

                if (kontakt == null)
                    return NotFound(new { poruka = "SOS kontakt nije pronađen" });

                if (kontakt.KorisnikId != korisnikId)
                    return Forbid();

                var prikaz = _mapper.Map<PrikazSOSKontaktaDTO>(kontakt);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{kontaktId}")]
        public async Task<IActionResult> AzurirajSOSKontakt(int kontaktId, [FromBody] AzurirajSOSKontaktDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakt = await _sosKontaktRepo.GetSOSKontakt(kontaktId);

                if (kontakt == null)
                    return NotFound(new { poruka = "SOS kontakt nije pronađen" });

                if (kontakt.KorisnikId != korisnikId)
                    return Forbid();

                var azuriraniKontakt = _mapper.Map<SOSKontakt>(dto);
                azuriraniKontakt.SOSKontaktId = kontaktId;

                var rezultat = await _sosKontaktRepo.AzurirajSOSKontakt(azuriraniKontakt);
                var prikaz = _mapper.Map<PrikazSOSKontaktaDTO>(rezultat);

                return Ok(new { poruka = "SOS kontakt uspešno ažuriran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{kontaktId}/aktiviraj")]
        public async Task<IActionResult> AktivirajSOSKontakt(int kontaktId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakt = await _sosKontaktRepo.GetSOSKontakt(kontaktId);

                if (kontakt == null)
                    return NotFound(new { poruka = "SOS kontakt nije pronađen" });

                if (kontakt.KorisnikId != korisnikId)
                    return Forbid();

                await _sosKontaktRepo.AktivirajSOSKontakt(kontaktId);
                return Ok(new { poruka = "SOS kontakt uspešno aktiviran" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{kontaktId}/deaktiviraj")]
        public async Task<IActionResult> DeaktivirajSOSKontakt(int kontaktId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakt = await _sosKontaktRepo.GetSOSKontakt(kontaktId);

                if (kontakt == null)
                    return NotFound(new { poruka = "SOS kontakt nije pronađen" });

                if (kontakt.KorisnikId != korisnikId)
                    return Forbid();

                await _sosKontaktRepo.DeaktivirajSOSKontakt(kontaktId);
                return Ok(new { poruka = "SOS kontakt uspešno deaktiviran" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{kontaktId}")]
        public async Task<IActionResult> ObrisiSOSKontakt(int kontaktId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var kontakt = await _sosKontaktRepo.GetSOSKontakt(kontaktId);

                if (kontakt == null)
                    return NotFound(new { poruka = "SOS kontakt nije pronađen" });

                if (kontakt.KorisnikId != korisnikId)
                    return Forbid();

                await _sosKontaktRepo.ObrisiSOSKontakt(kontaktId);
                return Ok(new { poruka = "SOS kontakt uspešno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}