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
    public class DnevnikMisliController : ControllerBase
    {
        private readonly IDnevnikMisliRepository _dnevnikRepo;
        private readonly IMapper _mapper;

        public DnevnikMisliController(IDnevnikMisliRepository dnevnikRepo, IMapper mapper)
        {
            _dnevnikRepo = dnevnikRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajBelisku([FromBody] KreirajDnevnikDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleska = _mapper.Map<DnevnikMisli>(dto);
                beleska.KorisnikId = korisnikId;

                var rezultat = await _dnevnikRepo.KreirajBelisku(beleska);
                var prikaz = _mapper.Map<PrikazDnevnikaDTO>(rezultat);

                return Ok(new { poruka = "Beleška uspešno kreirana", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMojeBeleske()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleske = await _dnevnikRepo.GetBeleskeKorisnika(korisnikId);
                var prikaz = _mapper.Map<List<PrikazDnevnikaDTO>>(beleske);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{beleskaId}")]
        public async Task<IActionResult> GetBeleska(int beleskaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleska = await _dnevnikRepo.GetBeleska(beleskaId);

                if (beleska == null)
                    return NotFound(new { poruka = "Beleška nije pronađena" });

                // Samo vlasnik ili terapeut kome je podeljena može da vidi
                if (beleska.KorisnikId != korisnikId &&
                    (!beleska.Deljena || beleska.DeljenjeTerapeutId != korisnikId))
                    return Forbid();

                var prikaz = _mapper.Map<PrikazDnevnikaDTO>(beleska);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("kategorija/{kategorija}")]
        public async Task<IActionResult> GetBeleskePoKategoriji(string kategorija)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleske = await _dnevnikRepo.GetBeleskePoKategoriji(korisnikId, kategorija);
                var prikaz = _mapper.Map<List<PrikazDnevnikaDTO>>(beleske);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("pretraga")]
        public async Task<IActionResult> PretraziBeleske([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Ok(new List<PrikazDnevnikaDTO>());

                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleske = await _dnevnikRepo.PretraziBeleske(korisnikId, q);
                var prikaz = _mapper.Map<List<PrikazDnevnikaDTO>>(beleske);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{beleskaId}")]
        public async Task<IActionResult> AzurirajBelisku(int beleskaId, [FromBody] AzurirajDnevnikDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleska = await _dnevnikRepo.GetBeleska(beleskaId);

                if (beleska == null)
                    return NotFound(new { poruka = "Beleška nije pronađena" });

                if (beleska.KorisnikId != korisnikId)
                    return Forbid();

                var azuriranaBeleska = _mapper.Map<DnevnikMisli>(dto);
                azuriranaBeleska.BeleskaId = beleskaId;

                var rezultat = await _dnevnikRepo.AzurirajBelisku(azuriranaBeleska);
                var prikaz = _mapper.Map<PrikazDnevnikaDTO>(rezultat);

                return Ok(new { poruka = "Beleška uspešno ažurirana", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{beleskaId}/podeli/{terapeutId}")]
        public async Task<IActionResult> PodeliBelisku(int beleskaId, string terapeutId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleska = await _dnevnikRepo.GetBeleska(beleskaId);

                if (beleska == null)
                    return NotFound(new { poruka = "Beleška nije pronađena" });

                if (beleska.KorisnikId != korisnikId)
                    return Forbid();

                await _dnevnikRepo.PodeliBeliskuSaTerapeutom(beleskaId, terapeutId);
                return Ok(new { poruka = "Beleška uspešno podeljena sa terapeutom" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{beleskaId}/ukini-deljenje")]
        public async Task<IActionResult> UkiniDeljenje(int beleskaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleska = await _dnevnikRepo.GetBeleska(beleskaId);

                if (beleska == null)
                    return NotFound(new { poruka = "Beleška nije pronađena" });

                if (beleska.KorisnikId != korisnikId)
                    return Forbid();

                await _dnevnikRepo.UkiniDeljenjeBeliske(beleskaId);
                return Ok(new { poruka = "Deljenje beleške je ukinuto" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("deljene-sa-mnom")]
        [Authorize(Roles = "Terapeut")]
        public async Task<IActionResult> GetDeljeneBeleske()
        {
            try
            {
                var terapeutId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleske = await _dnevnikRepo.GetDeljeneBeleske(terapeutId);
                var prikaz = _mapper.Map<List<PrikazDnevnikaDTO>>(beleske);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{beleskaId}")]
        public async Task<IActionResult> ObrisiBelisku(int beleskaId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var beleska = await _dnevnikRepo.GetBeleska(beleskaId);

                if (beleska == null)
                    return NotFound(new { poruka = "Beleška nije pronađena" });

                if (beleska.KorisnikId != korisnikId)
                    return Forbid();

                await _dnevnikRepo.ObrisiBelisku(beleskaId);
                return Ok(new { poruka = "Beleška uspešno obrisana" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}