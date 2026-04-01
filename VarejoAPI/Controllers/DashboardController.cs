using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Varejo.Data; // Ajuste para o namespace do seu DbContext
using VarejoSHARED.DTO; // Onde moram as nossas novas DTOs

namespace VarejoSHARED.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Descomente se a sua API exigir token JWT
    public class DashboardController : ControllerBase
    {
        private readonly VarejoDbContext _context;

        public DashboardController(VarejoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDTO>> GetDashboard()
        {
            var dto = new DashboardDTO();
            var hoje = DateTime.Today;
            var dataLimiteValidade = hoje.AddDays(30);

            // =====================================
            // 1. KPIs GERAIS
            // =====================================
            dto.TotalProdutos = await _context.Produtos.CountAsync();
            dto.TotalCategorias = await _context.Categorias.CountAsync();
            dto.TotalFamilias = await _context.Familias.CountAsync();
            dto.TotalUsuarios = await _context.Usuarios.CountAsync();
            dto.TotalMarcas = await _context.Marcas.CountAsync();
            dto.TotalMovimentos = await _context.Movimentos.CountAsync();
            dto.TotalProdutosMovimentados = await _context.ProdutosMovimento.CountAsync();

            // =====================================
            // 2. KPIs ESTRATÉGICOS (Alertas)
            // =====================================
            dto.TotalClientes = await _context.Pessoas
                .CountAsync(p => p.EhCliente && p.Ativo);

            dto.ProdutosSemEstoque = await _context.Produtos
                .CountAsync(p => p.Ativo && p.EstoqueAtual <= 0);

            dto.ProdutosVencendo = await _context.Validades
                .CountAsync(v => v.EmEstoque && v.DataValidade >= hoje && v.DataValidade <= dataLimiteValidade);

            // =====================================
            // 3. LISTAS PARA TABELAS E GRÁFICOS
            // =====================================

            // A. Últimos 5 Produtos Adicionados (Mapeado para ProdutoOutputDTO)
            dto.UltimosProdutos = await _context.Produtos
                .AsNoTracking()
                // Se não tiver DataCriacao, ordene pelo IdProduto decrescente
                .OrderByDescending(p => p.IdProduto)
                .Take(5)
                .Select(p => new ProdutoOutputDTO
                {
                    IdProduto = p.IdProduto,
                    NomeProduto = p.NomeProduto,
                    UrlImagem = p.UrlImagem ?? "",
                    Preco = p.CustoMedio // Mapeando CustoMedio para Preco caso não tenha Preco direto
                })
                .ToListAsync();

            // B. Produtos por Categoria (Mapeado para CategoriaCountDTO)
            dto.ProdutosPorCategoria = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Familia)
                    .ThenInclude(f => f.Categoria)
                .Where(p => p.Familia != null && p.Familia.Categoria != null)
                .GroupBy(p => new { p.Familia.Categoria.IdCategoria, p.Familia.Categoria.DescricaoCategoria })
                .Select(g => new CategoriaCountDTO
                {
                    IdCategoria = g.Key.IdCategoria,
                    Descricao = g.Key.DescricaoCategoria,
                    QtdeProdutos = g.Count()
                })
                .OrderByDescending(x => x.QtdeProdutos)
                .ToListAsync();

            // C. Últimos 5 Movimentos (Mapeado para MovimentoItemDTO)
            dto.UltimosMovimentos = await _context.Movimentos
                .AsNoTracking()
                .Include(m => m.Pessoa)
                .Include(m => m.TipoMovimento)
                .Include(m => m.ProdutosMovimento) // <- Usando a navegação correta que arrumamos antes!
                .OrderByDescending(m => m.DataMovimento) // ou DataCriacao, dependendo do seu banco
                .Take(5)
                .Select(m => new MovimentoItemDTO
                {
                    IdMovimento = m.IdMovimento,
                    Pessoa = m.Pessoa != null ? m.Pessoa.NomeRazao : "Não Informado",
                    TipoMovimento = m.TipoMovimento != null ? m.TipoMovimento.DescricaoTipoMovimento : "Geral",
                    Data = m.DataMovimento,
                    QtdeProdutos = m.ProdutosMovimento.Count()
                })
                .ToListAsync();

            // D. Validades Próximas (Mapeado para ValidadeOutputDTO)
            // Primeiro trazemos os dados do banco para a memória...
            var validadesCruas = await _context.Validades
                .AsNoTracking()
                .Include(v => v.Produto)
                .Where(v => v.EmEstoque && v.DataValidade >= hoje && v.DataValidade <= dataLimiteValidade)
                .OrderBy(v => v.DataValidade)
                .Take(6)
                .ToListAsync();

            // ...depois formatamos a data (o banco não entende .ToString("dd/MM/yyyy"))
            dto.ValidadesProximas = validadesCruas.Select(v => new ValidadeOutputDTO
            {
                IdValidade = v.IdValidade,
                DataValidade = v.DataValidade.ToString("dd/MM/yyyy"),
                EmEstoque = v.EmEstoque,
                ProdutoId = v.ProdutoId,
                ProdutoNome = v.Produto != null ? v.Produto.NomeProduto : "Desconhecido"
            }).ToList();

            return Ok(dto);
        }
    }
}