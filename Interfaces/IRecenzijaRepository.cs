using System.Collections.Generic;
using System.Threading.Tasks;
using MentalHealth.Data.Models;

namespace MentalHealth.Data.Interfaces
{
    public interface IRecenzijaRepository
    {
        Task<IEnumerable<Recenzija>> GetAllAsync(bool samoOdobrene = true);
        Task<Recenzija> GetByIdAsync(int id);
        Task<Recenzija> AddAsync(Recenzija recenzija);
        Task UpdateAsync(Recenzija recenzija);
        Task DeleteAsync(int id);
    }
}
