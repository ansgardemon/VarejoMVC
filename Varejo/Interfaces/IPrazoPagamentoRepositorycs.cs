using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IPrazoPagamentoRepository
    {
        Task<List<PrazoPagamento>> GetAllAsync();

        Task<PrazoPagamento> GetByIdAsync(int id);

        Task AddAsync(PrazoPagamento prazo);

        Task UpdateAsync(PrazoPagamento prazo);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string descricao);
    }
}