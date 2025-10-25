using Microsoft.AspNetCore.Mvc;
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

        public async Task<List<Produto>> GetByCategory(int id)
        {
            return await _context.Produtos
           .Include(p => p.Familia)
               .ThenInclude(f => f.Categoria)
           .Include(p => p.ProdutosEmbalagem)
           .Where(p => p.Familia.Categoria.IdCategoria == id)
           .ToListAsync();

        }

        public async Task<List<Produto>> GetAllDetailedAsync()
        {
            return await _context.Produtos
                .Include(p => p.Familia)
                .ThenInclude(f => f.Categoria)
                .Include(p => p.Familia)
                .ThenInclude(f => f.Marca)
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
                .Include(p => p.ProdutosEmbalagem) 
                .ThenInclude(e => e.TipoEmbalagem) 
                .FirstOrDefaultAsync(p => p.IdProduto == id);
        }


        public async Task<Produto?> GetByIdDetailedAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.Familia)
                .ThenInclude(f => f.Categoria)
                .Include(p => p.Familia)
                .ThenInclude(f => f.Marca)
                .FirstOrDefaultAsync(p => p.IdProduto == id);
        }


        public async Task<List<ProdutoViewModel>> GetByNameAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<ProdutoViewModel>();

            return await _context.Produtos
                .AsNoTracking()
                .Where(p => p.NomeProduto.Contains(query) && p.Ativo)
                .Include(p => p.Familia)
                .Include(p => p.ProdutosEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem) // << ERA SÓ FAZER ISSO CHAT MALDITOOOOOOOOOOOO CARALHOOOOOOOOOOOO
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
                            TipoEmbalagemDescricao = e.TipoEmbalagem != null ? e.TipoEmbalagem.DescricaoTipoEmbalagem : null
                        })
                        .ToList()
                })
                .Take(20)
                .ToListAsync();
        }


        public async Task<bool> ProdutoEmbalagemPossuiMovimentoAsync(int idProdutoEmbalagem)
        {
            // Supondo que você tenha uma tabela de movimentos chamada "Movimentos"
            // que tem uma FK chamada ProdutoEmbalagemId
            return await _context.ProdutosMovimento
                                 .AnyAsync(m => m.ProdutoEmbalagemId == idProdutoEmbalagem);
        }
        public async Task<List<Produto>> GetProdutosDestaqueAsync(int take = 8)
        {
            // retorna produtos ativos, com embalagens carregadas (e tipo de embalagem se precisar do multiplicador)
            return await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Ativo)
                .Include(p => p.ProdutosEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .OrderByDescending(p => EF.Property<DateTime>(p, "DataCriacao")) // ou outra regra de destaque
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Produto>> GetProdutosCatalogoAsync(int? idCategoria, int? idMarca)
        {
            // Base query com todos os includes necessários
            var query = _context.Produtos
                .AsNoTracking()
                .Where(p => p.Ativo)
                .Include(p => p.ProdutosEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .Include(p => p.Familia)
                    .ThenInclude(f => f.Categoria)
                .Include(p => p.Familia)
                    .ThenInclude(f => f.Marca)
                .AsQueryable();

            // Filtra por categoria se tiver valor
            if (idCategoria.HasValue && idCategoria.Value > 0)
            {
                query = query.Where(p => p.Familia.CategoriaId == idCategoria.Value);
            }

            // Filtra por marca se tiver valor
            if (idMarca.HasValue && idMarca.Value > 0)
            {
                query = query.Where(p => p.Familia.MarcaId == idMarca.Value);
            }

            // Ordena alfabeticamente
            var produtos = await query
                .OrderBy(p => p.NomeProduto)
                .ToListAsync();

            // Seleciona o menor preço disponível de cada produto
            foreach (var produto in produtos)
            {
                produto.ProdutosEmbalagem = produto.ProdutosEmbalagem
                    .OrderBy(pe => pe.Preco)
                    .Take(1)
                    .ToList();
            }

            return produtos;
        }





        public async Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

       

       
    }
}
