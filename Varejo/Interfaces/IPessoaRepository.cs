using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IPessoaRepository
    {
        Task<List<Pessoa>> GetAllAsync();
        //stand by
        Task<Pessoa> GetByIdAsync(int id);
        Task AddAsync(Pessoa pessoa);
        Task UpdateAsync(Pessoa pessoa);

        Task<List<Pessoa>> SearchByNameAsync(string nome);

        Task<List<Pessoa>> GetClientesAtivosAsync();

        Task<List<Pessoa>> GetFornecedoresAtivosAsync();

        Task<List<Pessoa>> SearchClientesAsync(string termo);
        Task<List<Pessoa>> SearchFornecedoresAsync(string termo);
    }
}
