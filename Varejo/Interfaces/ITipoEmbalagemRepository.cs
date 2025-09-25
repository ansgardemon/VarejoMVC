using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface ITipoEmbalagemRepository
    {
        Task<List<TipoEmbalagem>> GetAllAsync();
        Task<TipoEmbalagem?> GetByIdAsync(int id);
        Task AddAsync(TipoEmbalagem tipoEmbalagem);
        Task UpdateAsync(TipoEmbalagem tipoEmbalagem);
        Task DeleteAsync(int id);
    }
}
