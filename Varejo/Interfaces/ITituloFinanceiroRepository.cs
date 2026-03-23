using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface ITituloFinanceiroRepository
    {
        Task<List<TituloFinanceiro>> GetAllAsync();

        Task<TituloFinanceiro> GetByIdAsync(int id);

        Task<List<TituloFinanceiro>> GetByDocumentoAsync(int documento);

        Task AddAsync(TituloFinanceiro titulo);

        Task UpdateAsync(TituloFinanceiro titulo);

        Task DeleteAsync(int id);

        Task BaixarTituloAsync(int id, decimal valorPago, DateTime dataPagamento);

        Task GerarTitulosAsync(
            int documento,
            decimal valorTotal,
            int prazoPagamentoId,
            int especieTituloId,
            int? formaPagamentoId,
            int? pessoaId,
            DateTime dataEmissao
        );
    }
}