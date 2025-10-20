using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly VarejoDbContext _context;

        public ProdutoRepository(VarejoDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<List<Produto>> GetByFamilia(int id)
        {
            return await _context.Produtos
             .Include(p => p.Familia)
             .Where(p => p.FamiliaId == id)
             .ToListAsync();

        }

        public async Task<Produto?> GetByIdAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.Familia)
                .Include(p => p.ProdutosEmbalagem) // <- adiciona isso
                .ThenInclude(e => e.TipoEmbalagem) // opcional, se quiser já carregar o tipo
                .FirstOrDefaultAsync(p => p.IdProduto == id);
        }


        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
        
    }
}
