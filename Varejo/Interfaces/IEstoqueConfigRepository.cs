using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<EstoqueConfig?> GetByProdutoIdAsync(int produtoId);
        Task<List<EstoqueConfig>> GetAllAsync();

        Task AddAsync(EstoqueConfig config);
        Task UpdateAsync(EstoqueConfig config);

        // O "Coração" do sistema: Registra qualquer entrada/saída e gera o histórico
        Task<bool> RegistrarMovimentacaoAsync(int produtoId, int movimentoId, int tipoId, int produtoEmbalagemId, decimal quantidadeInformada, string? observacao);

        // Para o Inventário: Força o estoque a um valor específico e calcula a diferença para o histórico
        Task<bool> AjustarEstoqueInventarioAsync(int produtoId, int movimentoId, int inventarioId, decimal novaQuantidade, string? observacao);

        // Snapshot Diário
        Task<int> GerarSnapshotDiarioAsync();

        // Consultas para a sua ViewModel de Filtros
        Task<EstoqueViewModel> ObterEstoqueGeralAsync(EstoqueFiltroViewModel filtro);
    }
}
