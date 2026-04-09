using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IProdutoCustoRepository
    {
        // Busca o custo ativo (EhCustoAtual = true) para precificação
        Task<ProdutoCusto> GetCustoAtualAsync(int produtoId);

        // Busca todo o histórico de oscilação de preço de um item
        Task<IEnumerable<ProdutoCusto>> GetHistoricoCustosAsync(int produtoId);

        // Adiciona o novo custo e seta o anterior como EhCustoAtual = false
        Task<bool> AtualizarCustoAsync(int produtoId, int recebimentoId, decimal novoValor);
    }
}
