using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IFormaPagamentoRepository
    {
        Task<List<FormaPagamento>> GetAllAsync();

        Task<FormaPagamento> GetByIdAsync(int id);

        Task AddAsync(FormaPagamento forma);

        Task UpdateAsync(FormaPagamento forma);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(string descricao);
    }
}