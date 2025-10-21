using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

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
            return await _context.Produtos
                .Include(p => p.ProdutosEmbalagem)
                .ToListAsync();
        }

        public async Task<List<Produto>> GetByFamilia(int id)
        {
            return await _context.Produtos
             .Include(p => p.Familia)
             .Include(p => p.ProdutosEmbalagem)
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

        public async Task<List<ProdutoViewModel>> GetByNameAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<ProdutoViewModel>();

            return await _context.Produtos
                .Where(p => p.NomeProduto.Contains(query) && p.Ativo)
                .Select(p => new ProdutoViewModel
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto,
                    UrlImagem = p.UrlImagem,
                    Complemento = p.Complemento,
                    CustoMedio = p.CustoMedio,
                    EstoqueInicial = p.EstoqueInicial,
                    Ativo = p.Ativo,
                    FamiliaId = p.FamiliaId,
                    Familia = p.Familia,
                    Embalagens = p.ProdutosEmbalagem
                        .Select(e => new ProdutoEmbalagemViewModel
                        {
                            IdProdutoEmbalagem = e.IdProdutoEmbalagem,
                            Preco = e.Preco,
                            ProdutoId = e.ProdutoId,
                            TipoEmbalagemId = e.TipoEmbalagemId,
                            // TiposEmbalagem não precisa popular aqui, é só para forms
                        })
                        .ToList()
                })
                .Take(20) // limite para performance
                .ToListAsync();
        }

        public async Task<bool> ProdutoEmbalagemPossuiMovimentoAsync(int idProdutoEmbalagem)
        {
            // Supondo que você tenha uma tabela de movimentos chamada "Movimentos"
            // que tem uma FK chamada ProdutoEmbalagemId
            return await _context.ProdutosMovimento
                                 .AnyAsync(m => m.ProdutoEmbalagemId == idProdutoEmbalagem);
        }



        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

       

       
    }
}
