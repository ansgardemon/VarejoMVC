using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        public Task AddAsync(Produto produto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Produto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Produto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Produto produto)
        {
            throw new NotImplementedException();
        }
    }
}
