using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<List<Movimento>> GetAllAsync();
        Task<Movimento> GetByIdAsync(int id);
        Task AddAsync(Movimento movimento);

        Task UpdateAsync(Movimento movimento);

        Task DeleteAsync(int id);


    }
}
