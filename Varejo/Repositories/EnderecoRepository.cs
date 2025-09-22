using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;


namespace Varejo.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly VarejoDbContext _context;

        public EnderecoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Endereco endereco)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Endereco>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Endereco> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Endereco endereco)
        {
            throw new NotImplementedException();
        }
    }
}
