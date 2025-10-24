using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class FamiliaRepository : IFamiliaRepository
    {

        private readonly VarejoDbContext _context;

        public FamiliaRepository(VarejoDbContext context)
        {
            _context = context;
        }

        // LISTAR TODAS AS FAMÍLIAS (inclui Marca, Categoria e Produtos)
        public async Task<List<Familia>> GetAllAsync()
        {
            return await _context.Familias
                .Include(f => f.Marca)
                .Include(f => f.Categoria)
                .Include(f => f.Produtos)
                .ToListAsync();
        }

        // OBTER UMA FAMÍLIA PELO ID
        public async Task<Familia?> GetByIdAsync(int id)
        {
            return await _context.Familias
                .Include(f => f.Marca)
                .Include(f => f.Categoria)
                .Include(f => f.Produtos)
                .FirstOrDefaultAsync(f => f.IdFamilia == id);
        }

        // ADICIONAR
        public async Task AddAsync(Familia familia)
        {
            await _context.Familias.AddAsync(familia);
            await _context.SaveChangesAsync();
        }

        // ATUALIZAR
        public async Task UpdateAsync(Familia familia)
        {
            _context.Familias.Update(familia);
            await _context.SaveChangesAsync();
        }

        // DELETAR
        public async Task DeleteAsync(int id)
        {
            var familia = await _context.Familias.FindAsync(id);
            if (familia != null)
            {
                _context.Familias.Remove(familia);
                await _context.SaveChangesAsync();
            }
        }

        //MÉTODOS PARA DROPDOWN
        public List<Marca> GetMarcas()
        {
            return _context.Marcas.ToList();
        }

        public List<Categoria> GetCategorias()
        {
            return _context.Categorias.ToList();
        }

        public async Task<List<Familia>> GetByFamiliaCategory(int id)
        {
       
            return await _context.Familias
             .Include(p => p.Categoria)
             .Where(p => p.CategoriaId == id)
             .ToListAsync();

        }

     
    }
}


