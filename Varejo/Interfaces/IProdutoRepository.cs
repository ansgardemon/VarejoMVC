using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(int id);
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);
    }
}
