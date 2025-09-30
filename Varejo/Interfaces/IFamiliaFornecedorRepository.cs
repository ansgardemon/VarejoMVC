using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IFamiliaFornecedorRepository
    {
        Task<List<FornecedorFamilia>> GetAllAsync();
        Task<FornecedorFamilia?> GetByIdAsync(int id);
        Task AddAsync(FornecedorFamilia fornecedorFamilia);
        Task UpdateAsync(FornecedorFamilia fornecedorFamilia);
        Task DeleteAsync(int id);

        // OPCIONAL: métodos extras se precisar buscar por relacionamentos
        Task<List<FornecedorFamilia>> GetByPessoaIdAsync(int pessoaId);
        Task<List<FornecedorFamilia>> GetByFamiliaIdAsync(int familiaId);
    }
}
