using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IHistoricoProdutoRepository
    {
        Task AddAsync(HistoricoProduto historico);

        Task<List<HistoricoProduto>> GetByProdutoIdAsync(int produtoId);

        Task<List<HistoricoProduto>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
    }
}
