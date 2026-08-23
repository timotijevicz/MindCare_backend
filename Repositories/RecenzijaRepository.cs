using Microsoft.EntityFrameworkCore;
using MentalHealth.Data;
using MentalHealth.Data.Interfaces;
using MentalHealth.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentalHealth.Data.Repositories
{
    public class RecenzijaRepository : IRecenzijaRepository
    {
        private readonly AppDbContext _context;

        public RecenzijaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recenzija>> GetAllAsync(bool samoOdobrene = true)
        {
            var query = _context.Recenzije
                .Include(r => r.Korisnik)
                .AsQueryable();

            if (samoOdobrene)
                query = query.Where(r => r.Status == "Odobreno");

            return await query
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<Recenzija> GetByIdAsync(int id)
        {
            return await _context.Recenzije
                .Include(r => r.Korisnik)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recenzija> AddAsync(Recenzija recenzija)
        {
            recenzija.DatumKreiranja = DateTime.UtcNow;
            recenzija.Status = "NaCekanju";
            await _context.Recenzije.AddAsync(recenzija);
            await _context.SaveChangesAsync();
            return recenzija;
        }

        public async Task UpdateAsync(Recenzija recenzija)
        {
            _context.Recenzije.Update(recenzija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Recenzije.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
