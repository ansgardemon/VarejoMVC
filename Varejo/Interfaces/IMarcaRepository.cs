using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IMarcaRepository
    {
        Task<List<Marca>> GetAllAsync();
        Task<Marca> GetByIdAsync(int id);
        Task AddAsync(Marca marca);
        Task UpdateAsync(Marca marca);
        Task DeleteAsync(int id);
    }
}
