using System;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class ProdutoMovimentoRepository : IProdutoMovimentoRepository
    {
        private readonly VarejoDbContext _context;

        public ProdutoMovimentoRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto> GetByIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task AddAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
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

        public async Task AtualizarEstoqueAsync(ProdutoMovimento produtoMovimento)
        {
            var produto = await _context.Produtos.FindAsync(produtoMovimento.ProdutoId);
            if (produto == null) return;

            var movimento = await _context.Movimentos
                .Include(m => m.TipoMovimento)
                .ThenInclude(t => t.EspecieMovimento)
                .FirstOrDefaultAsync(m => m.IdMovimento == produtoMovimento.MovimentoId);

            if (movimento == null) return;

            var especieId = movimento.TipoMovimento.EspecieMovimento.IdEspecieMovimento;

            // 1 = Entrada | 2 = Saída
            switch (especieId)
            {
                case 1:
                    produto.EstoqueAtual += produtoMovimento.Quantidade;
                    break;

                case 2:
                    produto.EstoqueAtual -= produtoMovimento.Quantidade;
                    break;
            }

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
    }

}
