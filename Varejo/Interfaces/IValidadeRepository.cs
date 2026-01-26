using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IValidadeRepository
    {
        Task<List<Validade>> GetAllAsync();
        Task<Validade> GetByIdAsync(int id);

        Task<IEnumerable<Validade>> FiltrarAsync(
DateTime? dataInicial,
DateTime? dataFinal,
string? produtoNome,
bool? emEstoque);
        Task AddAsync(Validade validade);

        Task UpdateAsync(Validade validade);

        Task DeleteAsync(int id);

        Task<IEnumerable<Validade>> GetByEstoqueAsync(bool emEstoque);

    }
}
