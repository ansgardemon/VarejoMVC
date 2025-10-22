using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface ITipoMovimentoRepository
    {
        Task<List<TipoMovimento>> GetAllAsync();
        Task<TipoMovimento?> GetByIdAsync(int id);
        Task AddAsync(TipoMovimento tipoMovimento);
        Task UpdateAsync(TipoMovimento tipoMovimento);
        Task DeleteAsync(int id);

        Task<List<EspecieMovimento>> GetAllEspeciesAsync();

    }
}
