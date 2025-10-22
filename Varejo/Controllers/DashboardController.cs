using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Varejo.Controllers
{
    [Authorize(Roles = "Administrador,Gerente,Usuário")]
    public class DashboardController : Controller
    {
        private readonly VarejoDbContext _context;

        public DashboardController(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            // =====================
            // CARDS
            // =====================
            vm.TotalProdutos = await _context.Produtos.CountAsync();
            vm.TotalCategorias = await _context.Categorias.CountAsync();
            vm.TotalFamilias = await _context.Familias.CountAsync();
            vm.TotalUsuarios = await _context.Usuarios.CountAsync();
            vm.TotalMarcas = await _context.Marcas.CountAsync();

            // CARDS DE MOVIMENTOS
            vm.TotalMovimentos = await _context.Movimentos.CountAsync();
            vm.TotalProdutosMovimentados = await _context.ProdutosMovimento.CountAsync();

            // =====================
            // ÚLTIMOS 5 PRODUTOS ADICIONADOS
            // =====================
            vm.UltimosProdutos = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Familia)
                    .ThenInclude(f => f.Categoria)
                .OrderByDescending(p => EF.Property<DateTime>(p, "DataCriacao"))
                .Select(p => new ProdutoItem
                {
                    IdProduto = p.IdProduto,
                    Nome = p.NomeProduto,
                    Familia = p.Familia != null ? p.Familia.NomeFamilia : "",
                    Categoria = p.Familia != null && p.Familia.Categoria != null ? p.Familia.Categoria.DescricaoCategoria : "",
                    UrlImagem = p.UrlImagem ?? ""
                })
                .Take(5)
                .ToListAsync();

            // =====================
            // PRODUTOS POR CATEGORIA
            // =====================
            vm.ProdutosPorCategoria = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Familia)
                    .ThenInclude(f => f.Categoria)
                .GroupBy(p => new { p.Familia.Categoria.IdCategoria, p.Familia.Categoria.DescricaoCategoria })
                .Select(g => new CategoriaCount
                {
                    IdCategoria = g.Key.IdCategoria,
                    Descricao = g.Key.DescricaoCategoria,
                    QtdeProdutos = g.Count()
                })
                .OrderByDescending(x => x.QtdeProdutos)
                .ToListAsync();

            // =====================
            // PRODUTOS POR FAMILIA
            // =====================
            vm.ProdutosPorFamilia = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Familia)
                .GroupBy(p => new { p.Familia.IdFamilia, p.Familia.NomeFamilia })
                .Select(g => new FamiliaCount
                {
                    IdFamilia = g.Key.IdFamilia,
                    Descricao = g.Key.NomeFamilia,
                    QtdeProdutos = g.Count()
                })
                .OrderByDescending(x => x.QtdeProdutos)
                .ToListAsync();

            // =====================
            // USUÁRIOS RECENTES
            // =====================
            vm.UltimosUsuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderByDescending(u => EF.Property<DateTime>(u, "DataCriacao"))
                .Select(u => new UsuarioItem
                {
                    IdUsuario = u.IdUsuario,
                    NomeUsuario = u.nomeUsuario,
                    TipoUsuario = u.TipoUsuario != null ? u.TipoUsuario.DescricaoTipoUsuario : ""
                })
                .Take(5)
                .ToListAsync();

            // =====================
            // ÚLTIMOS 5 MOVIMENTOS
            // =====================
            vm.UltimosMovimentos = await _context.Movimentos
                .AsNoTracking()
                .Include(m => m.Pessoa)
                .Include(m => m.TipoMovimento)
                .Include(m => m.ProdutosMovimento)
                .OrderByDescending(m => EF.Property<DateTime>(m, "DataCriacao"))
                .Take(5)
                .Select(m => new MovimentoItem
                {
                    IdMovimento = m.IdMovimento,
                    Pessoa = m.Pessoa != null ? m.Pessoa.NomeRazao : "",
                    TipoMovimento = m.TipoMovimento != null ? m.TipoMovimento.DescricaoTipoMovimento : "",
                    Data = EF.Property<DateTime>(m, "DataCriacao"),
                    QtdeProdutos = m.ProdutosMovimento.Count
                })
                .ToListAsync();

            // =====================
            // MOVIMENTOS POR TIPO
            // =====================
            vm.MovimentosPorTipo = await _context.Movimentos
                .AsNoTracking()
                .Include(m => m.TipoMovimento)
                .GroupBy(m => new { m.TipoMovimento.IdTipoMovimento, m.TipoMovimento.DescricaoTipoMovimento })
                .Select(g => new MovimentosPorTipo
                {
                    IdTipoMovimento = g.Key.IdTipoMovimento,
                    TipoMovimento = g.Key.DescricaoTipoMovimento,
                    QtdeMovimentos = g.Count()
                })
                .OrderByDescending(x => x.QtdeMovimentos)
                .ToListAsync();

            return View(vm);
        }
    }
}
