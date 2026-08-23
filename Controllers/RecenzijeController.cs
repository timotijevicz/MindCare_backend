using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MentalHealth.Data.DTOs;
using MentalHealth.Data.Interfaces;
using MentalHealth.Data.Models;

namespace MentalHealth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecenzijeController : ControllerBase
    {
        private readonly IRecenzijaRepository _repository;
        private readonly UserManager<Korisnik> _userManager;

        public RecenzijeController(IRecenzijaRepository repository, UserManager<Korisnik> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // GET: api/recenzije – samo odobrene (javno)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var recenzije = await _repository.GetAllAsync(samoOdobrene: true);
            var response = recenzije.Select(r => new RecenzijaResponseDto
            {
                Id = r.Id,
                Tekst = r.Tekst,
                Ocena = r.Ocena,
                DatumKreiranja = r.DatumKreiranja,
                KorisnikIme = $"{r.Korisnik?.Ime} {r.Korisnik?.Prezime}".Trim(),
                KorisnikId = r.KorisnikId,
                Status = r.Status
            });
            return Ok(response);
        }

        // GET: api/recenzije/all – sve recenzije (admin/terapeut)
        [HttpGet("all")]
        [Authorize(Roles = "Administrator,Terapeut")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var recenzije = await _repository.GetAllAsync(samoOdobrene: false);
            var response = recenzije.Select(r => new RecenzijaResponseDto
            {
                Id = r.Id,
                Tekst = r.Tekst,
                Ocena = r.Ocena,
                DatumKreiranja = r.DatumKreiranja,
                KorisnikIme = $"{r.Korisnik?.Ime} {r.Korisnik?.Prezime}".Trim(),
                KorisnikId = r.KorisnikId,
                Status = r.Status
            });
            return Ok(response);
        }

        // POST: api/recenzije
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecenzijaCreateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Niste prijavljeni.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized("Korisnik nije pronađen.");

            var recenzija = new Recenzija
            {
                KorisnikId = userId,
                Tekst = dto.Tekst,
                Ocena = dto.Ocena,
                DatumKreiranja = DateTime.UtcNow,
                Status = "NaCekanju"
            };

            var created = await _repository.AddAsync(recenzija);
            var response = new RecenzijaResponseDto
            {
                Id = created.Id,
                Tekst = created.Tekst,
                Ocena = created.Ocena,
                DatumKreiranja = created.DatumKreiranja,
                KorisnikIme = $"{user.Ime} {user.Prezime}".Trim(),
                KorisnikId = user.Id,
                Status = created.Status
            };

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        // GET: api/recenzije/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var recenzija = await _repository.GetByIdAsync(id);
            if (recenzija == null)
                return NotFound();

            if (recenzija.Status != "Odobreno")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Administrator") || User.IsInRole("Terapeut");
                if (userId != recenzija.KorisnikId && !isAdmin)
                    return Forbid();
            }

            var response = new RecenzijaResponseDto
            {
                Id = recenzija.Id,
                Tekst = recenzija.Tekst,
                Ocena = recenzija.Ocena,
                DatumKreiranja = recenzija.DatumKreiranja,
                KorisnikIme = $"{recenzija.Korisnik?.Ime} {recenzija.Korisnik?.Prezime}".Trim(),
                KorisnikId = recenzija.KorisnikId,
                Status = recenzija.Status
            };
            return Ok(response);
        }

        // PUT: api/recenzije/{id}/status (samo admin/terapeut)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Terapeut")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] RecenzijaUpdateStatusDto dto)
        {
            var recenzija = await _repository.GetByIdAsync(id);
            if (recenzija == null)
                return NotFound();

            if (dto.Status != "Odobreno" && dto.Status != "Odbijeno")
                return BadRequest("Status mora biti 'Odobreno' ili 'Odbijeno'.");

            recenzija.Status = dto.Status;
            await _repository.UpdateAsync(recenzija);

            return NoContent();
        }

        // DELETE: api/recenzije/{id} (samo administrator)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var recenzija = await _repository.GetByIdAsync(id);
            if (recenzija == null)
                return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
