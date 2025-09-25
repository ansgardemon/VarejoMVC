using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class ProdutoEmbalagemRepository : IProdutoEmbalagemRepository
    {
        private readonly VarejoDbContext _context;

        public ProdutoEmbalagemRepository(VarejoDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ProdutoEmbalagem produtoEmbalagem)
        {
            await _context.AddAsync(produtoEmbalagem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var produtoEmbalagem = _context.ProdutosEmbalagem.Find(id);
            if (produtoEmbalagem != null)
            {
                _context.ProdutosEmbalagem.Remove(produtoEmbalagem);
                _context.SaveChanges();
            }
        }

        public async Task<List<ProdutoEmbalagem>> GetAllAsync()
        {
            return await _context.ProdutosEmbalagem.ToListAsync();
        }

        public async Task<ProdutoEmbalagem?> GetByIdAsync(int id)
        {
            return await _context.ProdutosEmbalagem
                                 .FirstOrDefaultAsync(e => e.IdProdutoEmbalagem == id);
        }

        public async Task UpdateAsync(ProdutoEmbalagem produtoEmbalagem)
        {
            _context.ProdutosEmbalagem.Update(produtoEmbalagem);
            await _context.SaveChangesAsync();
        }
    }
}
