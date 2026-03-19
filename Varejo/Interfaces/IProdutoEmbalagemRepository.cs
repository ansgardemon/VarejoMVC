using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IProdutoEmbalagemRepository
    {
        Task<List<ProdutoEmbalagem>> GetAllAsync();
        Task<ProdutoEmbalagem?> GetByIdAsync(int id);
        Task AddAsync(ProdutoEmbalagem produtoEmbalagem);
        Task UpdateAsync(ProdutoEmbalagem produtoEmbalagem);
        Task DeleteAsync(int id);

        Task<bool> EanExisteAsync(string ean, int? ignorarId = null);

        Task<IEnumerable<ProdutoEmbalagem>> GetByProdutoIdAsync(int produtoId);
    }
}
