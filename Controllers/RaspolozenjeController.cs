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
    public class RaspolozenjeController : ControllerBase
    {
        private readonly IRaspolozenjeRepository _raspolozenjeRepo;
        private readonly IMapper _mapper;

        public RaspolozenjeController(IRaspolozenjeRepository raspolozenjeRepo, IMapper mapper)
        {
            _raspolozenjeRepo = raspolozenjeRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajRaspolozenje([FromBody] KreirajRaspolozenjeDTO dto)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var raspolozenje = _mapper.Map<DnevnoRaspolozenje>(dto);
                raspolozenje.KorisnikId = korisnikId;

                var rezultat = await _raspolozenjeRepo.KreirajRaspolozenje(raspolozenje);
                var prikaz = _mapper.Map<PrikazRaspolozenjaDTO>(rezultat);

                return Ok(new { poruka = "Raspoloženje uspešno zabeleženo", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMojaRaspolozenja()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var raspolozenja = await _raspolozenjeRepo.GetRaspolozenjaKorisnika(korisnikId);
                var prikaz = _mapper.Map<List<PrikazRaspolozenjaDTO>>(raspolozenja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("danas")]
        public async Task<IActionResult> GetDanasnjeRaspolozenje()
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var raspolozenje = await _raspolozenjeRepo.GetDanasnjeRaspolozenje(korisnikId);

                if (raspolozenje == null)
                    return Ok(new { poruka = "Još uvek niste uneli današnje raspoloženje", podaci = (object)null });

                var prikaz = _mapper.Map<PrikazRaspolozenjaDTO>(raspolozenje);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("period")]
        public async Task<IActionResult> GetRaspolozenjaPeriod([FromQuery] DateTime od, [FromQuery] DateTime dok)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var raspolozenja = await _raspolozenjeRepo.GetRaspolozenjaPeriod(korisnikId, od, dok);
                var prikaz = _mapper.Map<List<PrikazRaspolozenjaDTO>>(raspolozenja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("statistika")]
        public async Task<IActionResult> GetStatistika([FromQuery] int brojDana = 30)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var prosecnaOcena = await _raspolozenjeRepo.GetProsecnaOcena(korisnikId);
                var brojUnosa = await _raspolozenjeRepo.GetBrojUnosa(korisnikId);
                var statistikaPoDanima = await _raspolozenjeRepo.GetStatistikaPoDanima(korisnikId, brojDana);
                var prosekPoDanuUNedelji = await _raspolozenjeRepo.GetProsekPoDanuUNedelji(korisnikId, brojDana);

                var naziviDana = new[] { "Ponedeljak", "Utorak", "Sreda", "Četvrtak", "Petak", "Subota", "Nedelja" };
                var redosledDana = new[]
                {
                    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                };
                var prosekPoDanuNazivi = redosledDana
                    .Select((dan, i) => new { naziv = naziviDana[i], prosek = prosekPoDanuUNedelji[dan] })
                    .ToDictionary(x => x.naziv, x => x.prosek);

                return Ok(new
                {
                    prosecnaOcena,
                    brojUnosa,
                    statistikaPoDanima,
                    prosekPoDanuUNedelji = prosekPoDanuNazivi
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("klijent/{klijentId}")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> GetRaspolozenjaKlijenta(string klijentId)
        {
            try
            {
                var terapeutId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var raspolozenja = await _raspolozenjeRepo.GetRaspolozenjaKlijentaZaTerapeuta(terapeutId, klijentId);
                var prikaz = _mapper.Map<List<PrikazRaspolozenjaDTO>>(raspolozenja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{raspolozenjeId}")]
        public async Task<IActionResult> ObrisiRaspolozenje(int raspolozenjeId)
        {
            try
            {
                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var raspolozenje = await _raspolozenjeRepo.GetRaspolozenje(raspolozenjeId);

                if (raspolozenje == null)
                    return NotFound(new { poruka = "Raspoloženje nije pronađeno" });

                if (raspolozenje.KorisnikId != korisnikId)
                    return Forbid();

                await _raspolozenjeRepo.ObrisiRaspolozenje(raspolozenjeId);
                return Ok(new { poruka = "Raspoloženje uspešno obrisano" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}