using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IProdutoMovimentoRepository
    {
        Task<List<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(int id);
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);

        Task AtualizarEstoqueAsync(ProdutoMovimento produtoMovimento);


    }
}
