using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{

    public class ValidadeRepository : IValidadeRepository
    {

            private readonly VarejoDbContext _context;

    public ValidadeRepository(VarejoDbContext context)
    {
        _context = context;
    }



        public async Task AddAsync(Validade validade)
        {
            await _context.Validades.AddAsync(validade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var validade = _context.Validades.Find(id);
            if (validade != null)
            {
                _context.Validades.Remove(validade);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Validade>> GetAllAsync()
        {
            return await _context.Validades
                                 .Include(v => v.Produto)
                                 .OrderBy(v => v.DataValidade)
                                   .ToListAsync();
        }

        public async Task<Validade> GetByIdAsync(int id)
        {
            return await _context.Validades
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(v => v.IdValidade == id);
        }

        public async Task<IEnumerable<Validade>> FiltrarAsync(
    DateTime? dataInicial,
    DateTime? dataFinal,
    string? produtoNome,
    bool? emEstoque)
        {
            var query = _context.Validades
                .Include(v => v.Produto)
                .AsQueryable();

            if (dataInicial.HasValue)
                query = query.Where(v => v.DataValidade >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(v => v.DataValidade <= dataFinal.Value);

            if (!string.IsNullOrEmpty(produtoNome))
                query = query.Where(v => v.Produto.NomeProduto.Contains(produtoNome));

            if (emEstoque.HasValue)
                query = query.Where(v => v.EmEstoque == emEstoque.Value);

            return await query.OrderBy(v => v.DataValidade).ToListAsync();
        }

        public async Task UpdateAsync(Validade validade)
        {

            _context.Validades.Update(validade);
            await _context.SaveChangesAsync();
        }
    }
}
