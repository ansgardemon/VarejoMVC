using Varejo.Models;

namespace Varejo.Interfaces
{
    public interface IEnderecoRepository
    {
        Task<List<Endereco>> GetAllAsync();
        Task<Endereco> GetByIdAsync(int id);
        Task AddAsync(Endereco endereco);
        Task UpdateAsync(Endereco endereco);
        Task DeleteAsync(int id);
    }
}
