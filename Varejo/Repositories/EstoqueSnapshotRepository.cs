using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class EstoqueSnapshotRepository : IEstoqueSnapshotRepository
    {
        private readonly VarejoDbContext _context;

        public EstoqueSnapshotRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task GerarSnapshotAsync()
        {
            var produtos = await _context.Produtos.ToListAsync();

            var snapshot = produtos.Select(p => new EstoqueSnapshot
            {
                ProdutoId = p.IdProduto,
                Estoque = p.EstoqueAtual,
                Data = DateTime.Now
            }).ToList();

            _context.EstoquesSnapshot.AddRange(snapshot);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EstoqueSnapshot>> GetUltimoSnapshotAsync()
        {
            var ultimaData = await _context.EstoquesSnapshot
                .MaxAsync(e => (DateTime?)e.Data);

            if (ultimaData == null)
                return new List<EstoqueSnapshot>();

            return await _context.EstoquesSnapshot
                .Where(e => e.Data == ultimaData)
                .ToListAsync();
        }
    }
}
