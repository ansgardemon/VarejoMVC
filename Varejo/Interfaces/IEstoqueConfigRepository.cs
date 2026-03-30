using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IEstoqueConfigRepository
    {
        Task<EstoqueConfig?> GetByProdutoIdAsync(int produtoId);
        Task<List<EstoqueConfig>> GetAllAsync();

        Task AddAsync(EstoqueConfig config);
        Task UpdateAsync(EstoqueConfig config);
    }
}
