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
    public class ZakazivanjeController : ControllerBase
    {
        private readonly IZakazivanjeRepository _zakazivanjeRepo;
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly IMapper _mapper;

        public ZakazivanjeController(IZakazivanjeRepository zakazivanjeRepo, IKorisnikRepository korisnikRepo, IMapper mapper)
        {
            _zakazivanjeRepo = zakazivanjeRepo;
            _korisnikRepo = korisnikRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> KreirajZakazivanje([FromBody] KreirajZakazivanjeDTO dto)
        {
            try
            {
                var klijentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var zakazivanje = _mapper.Map<ZakazivanjeSesije>(dto);
                zakazivanje.KlijentId = klijentId;

                var rezultat = await _zakazivanjeRepo.KreirajZakazivanje(zakazivanje);
                var prikaz = _mapper.Map<PrikazZakazivanjaDTO>(rezultat);

                return Ok(new { poruka = "Zakazivanje uspešno kreirano", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("moja")]
        public async Task<IActionResult> GetMojaZakazivanja()
        {
            try
            {
                var klijentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var zakazivanja = await _zakazivanjeRepo.GetZakazivanjaKlijenta(klijentId);
                var prikaz = _mapper.Map<List<PrikazZakazivanjaDTO>>(zakazivanja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("terapeut/{terapeutId}")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> GetZakazivanjaTerapeuta(int terapeutId)
        {
            try
            {
                var zakazivanja = await _zakazivanjeRepo.GetZakazivanjaTerapeuta(terapeutId);
                var prikaz = _mapper.Map<List<PrikazZakazivanjaDTO>>(zakazivanja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("datum/{datum}")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> GetZakazivanjaZaDatum(DateTime datum)
        {
            try
            {
                var zakazivanja = await _zakazivanjeRepo.GetZakazivanjaZaDatum(datum);
                var prikaz = _mapper.Map<List<PrikazZakazivanjaDTO>>(zakazivanja);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("proveri-dostupnost/{terapeutId}")]
        public async Task<IActionResult> ProveriDostupnost(int terapeutId, [FromQuery] DateTime datum)
        {
            try
            {
                var dostupnost = await _zakazivanjeRepo.ProveriDostupnostTermina(terapeutId, datum);
                return Ok(new { dostupno = dostupnost });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{zakazivanjeId}/status")]
        [Authorize(Roles = "Terapeut,Administrator")]
        public async Task<IActionResult> AzurirajStatus(int zakazivanjeId, [FromBody] string status)
        {
            try
            {
                var zakazivanje = await _zakazivanjeRepo.GetZakazivanje(zakazivanjeId);
                if (zakazivanje == null)
                    return NotFound(new { poruka = "Zakazivanje nije pronađeno" });

                if (!User.IsInRole("Administrator"))
                {
                    var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var terapeut = await _korisnikRepo.GetTerapeutPoKorisnikId(korisnikId);
                    if (terapeut == null || zakazivanje.TerapeutId != terapeut.TerapeutId)
                        return Forbid();
                }

                var rezultat = await _zakazivanjeRepo.AzurirajStatusZakazivanja(zakazivanjeId, status);
                var prikaz = _mapper.Map<PrikazZakazivanjaDTO>(rezultat);

                return Ok(new { poruka = "Status zakazivanja uspešno ažuriran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost("{zakazivanjeId}/otkazi")]
        public async Task<IActionResult> OtkaziZakazivanje(int zakazivanjeId)
        {
            try
            {
                var zakazivanje = await _zakazivanjeRepo.GetZakazivanje(zakazivanjeId);
                if (zakazivanje == null)
                    return NotFound(new { poruka = "Zakazivanje nije pronađeno" });

                var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (zakazivanje.KlijentId != korisnikId && !User.IsInRole("Administrator"))
                    return Forbid();

                await _zakazivanjeRepo.OtkaziZakazivanje(zakazivanjeId);
                return Ok(new { poruka = "Zakazivanje uspešno otkazano" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}