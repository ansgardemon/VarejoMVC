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
            return await _context.ProdutosEmbalagem
                .Include(t => t.TipoEmbalagem) // se quiser carregar o relacionamento
                .ToListAsync();
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

        public async Task<bool> EanExisteAsync(string ean)
        {
            return await _context.ProdutosEmbalagem
                .AnyAsync(e => e.Ean == ean);
        }

        public async Task<bool> EanExisteAsync(string ean, int? ignorarId = null)
        {
            return await _context.ProdutosEmbalagem
                .AnyAsync(e => e.Ean == ean &&
                              (!ignorarId.HasValue || e.IdProdutoEmbalagem != ignorarId));
        }

        public async Task<IEnumerable<ProdutoEmbalagem>> GetByProdutoIdAsync(int produtoId)
        {
            return await _context.ProdutosEmbalagem
                .Where(e => e.ProdutoId == produtoId)
                .Include(e => e.TipoEmbalagem)
                .ToListAsync();
        }
    }
}
