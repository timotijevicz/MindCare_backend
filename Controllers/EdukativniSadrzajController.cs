using AutoMapper;
using MentalHealth.Data.Models;
using MentalHealth.DTOs;
using MentalHealth.Interfejsi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentalHealth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EdukativniSadrzajController : ControllerBase
    {
        private readonly IEdukativniSadrzajRepository _edukativniSadrzajRepo;
        private readonly IMapper _mapper;

        public EdukativniSadrzajController(IEdukativniSadrzajRepository edukativniSadrzajRepo, IMapper mapper)
        {
            _edukativniSadrzajRepo = edukativniSadrzajRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSviSadrzaji()
        {
            try
            {
                var sadrzaji = await _edukativniSadrzajRepo.GetSviSadrzaji();
                var prikaz = _mapper.Map<List<PrikazEdukativnogSadrzajaDTO>>(sadrzaji);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("{sadrzajId}")]
        public async Task<IActionResult> GetSadrzaj(int sadrzajId)
        {
            try
            {
                var sadrzaj = await _edukativniSadrzajRepo.GetSadrzaj(sadrzajId);

                if (sadrzaj == null)
                    return NotFound(new { poruka = "Sadržaj nije pronađen" });

                var prikaz = _mapper.Map<PrikazEdukativnogSadrzajaDTO>(sadrzaj);
                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("kategorija/{kategorija}")]
        public async Task<IActionResult> GetSadrzajiPoKategoriji(string kategorija)
        {
            try
            {
                var sadrzaji = await _edukativniSadrzajRepo.GetSadrzajiPoKategoriji(kategorija);
                var prikaz = _mapper.Map<List<PrikazEdukativnogSadrzajaDTO>>(sadrzaji);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("pretraga")]
        public async Task<IActionResult> PretraziSadrzaje([FromQuery] string termin)
        {
            try
            {
                if (string.IsNullOrEmpty(termin))
                    return BadRequest(new { poruka = "Termin za pretragu je obavezan" });

                var sadrzaji = await _edukativniSadrzajRepo.PretraziSadrzaje(termin);
                var prikaz = _mapper.Map<List<PrikazEdukativnogSadrzajaDTO>>(sadrzaji);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpGet("najnoviji")]
        public async Task<IActionResult> GetNajnovijiSadrzaji([FromQuery] int broj = 5)
        {
            try
            {
                var sadrzaji = await _edukativniSadrzajRepo.GetNajnovijiSadrzaji(broj);
                var prikaz = _mapper.Map<List<PrikazEdukativnogSadrzajaDTO>>(sadrzaji);

                return Ok(prikaz);
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> KreirajSadrzaj([FromBody] KreirajEdukativniSadrzajDTO dto)
        {
            try
            {
                var sadrzaj = _mapper.Map<EdukativniSadrzaj>(dto);
                var rezultat = await _edukativniSadrzajRepo.KreirajSadrzaj(sadrzaj);
                var prikaz = _mapper.Map<PrikazEdukativnogSadrzajaDTO>(rezultat);

                return Ok(new { poruka = "Sadržaj uspešno kreiran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpPut("{sadrzajId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AzurirajSadrzaj(int sadrzajId, [FromBody] KreirajEdukativniSadrzajDTO dto)
        {
            try
            {
                var sadrzaj = _mapper.Map<EdukativniSadrzaj>(dto);
                sadrzaj.SadrzajId = sadrzajId;

                var rezultat = await _edukativniSadrzajRepo.AzurirajSadrzaj(sadrzaj);
                var prikaz = _mapper.Map<PrikazEdukativnogSadrzajaDTO>(rezultat);

                return Ok(new { poruka = "Sadržaj uspešno ažuriran", podaci = prikaz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        [HttpDelete("{sadrzajId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ObrisiSadrzaj(int sadrzajId)
        {
            try
            {
                await _edukativniSadrzajRepo.ObrisiSadrzaj(sadrzajId);
                return Ok(new { poruka = "Sadržaj uspešno obrisan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }
    }
}