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
        Task DeleteAsync(int id);
    }
}
