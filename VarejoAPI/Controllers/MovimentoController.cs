using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;
using VarejoSHARED.DTO;
using Varejo.Interfaces;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {
        private readonly VarejoDbContext _context;
        private readonly IEstoqueRepository _estoqueRepo;

        public MovimentoController(VarejoDbContext context, IEstoqueRepository estoqueRepo)
        {
            _context = context;
            _estoqueRepo = estoqueRepo;
        }

        // 🔹 GET: api/movimento
        [HttpGet]
        public async Task<ActionResult<List<MovimentoOutputDTO>>> GetAll()
        {
            var movimentos = await _context.Movimentos
                .Select(m => new MovimentoOutputDTO
                {
                    IdMovimento = m.IdMovimento,
                    Documento = m.Documento,
                    Observacao = m.Observacao,
                    DataMovimento = m.DataMovimento,
                    Pessoa = m.Pessoa.NomeRazao,
                    TipoMovimento = m.TipoMovimento.DescricaoTipoMovimento,
                    Produtos = m.ProdutosMovimento.Select(pm => new ProdutoMovimentoOutputDTO
                    {
                        IdProdutoMovimento = pm.IdProdutoMovimento,
                        Quantidade = pm.Quantidade,
                        Produto = pm.Produto.NomeProduto,
                        Embalagem = pm.ProdutoEmbalagem.TipoEmbalagem.DescricaoTipoEmbalagem
                    }).ToList()
                })
                .ToListAsync();

            return Ok(movimentos);
        }

        // 🔹 GET: api/movimento/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimentoOutputDTO>> GetById(int id)
        {
            var movimento = await _context.Movimentos
                .Where(m => m.IdMovimento == id)
                .Select(m => new MovimentoOutputDTO
                {
                    IdMovimento = m.IdMovimento,
                    Documento = m.Documento,
                    Observacao = m.Observacao,
                    DataMovimento = m.DataMovimento,
                    Pessoa = m.Pessoa.NomeRazao,
                    TipoMovimento = m.TipoMovimento.DescricaoTipoMovimento,
                    Produtos = m.ProdutosMovimento.Select(pmo => new ProdutoMovimentoOutputDTO
                    {
                        IdProdutoMovimento = pmo.IdProdutoMovimento,
                        Quantidade = pmo.Quantidade,
                        Produto = pmo.Produto.NomeProduto,
                        Embalagem = pmo.ProdutoEmbalagem.TipoEmbalagem.DescricaoTipoEmbalagem
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (movimento == null)
                return NotFound();

            return Ok(movimento);
        }

        // 🔹 POST: api/movimento
        [HttpPost]
        public async Task<ActionResult> Post(MovimentoInputDTO dto)
        {
            // Verificações básicas
            if (dto == null)
                return BadRequest("Dados inválidos.");

            if (dto.Produtos == null || !dto.Produtos.Any())
                return BadRequest("O movimento deve ter pelo menos um produto.");

            // Verifica se Pessoa existe
            var pessoaExiste = await _context.Pessoas.AnyAsync(p => p.IdPessoa == dto.PessoaId);
            if (!pessoaExiste)
                return BadRequest("Pessoa informada não existe.");

            // Verifica se TipoMovimento existe
            var tipoExiste = await _context.TiposMovimento.AnyAsync(t => t.IdTipoMovimento == dto.TipoMovimentoId);
            if (!tipoExiste)
                return BadRequest("Tipo de movimento inválido.");

            var movimento = new Movimento
            {
                Documento = dto.Documento,
                Observacao = dto.Observacao ?? "Movimentação via API",
                DataMovimento = dto.DataMovimento ?? DateTime.Now,
                PessoaId = dto.PessoaId,
                TipoMovimentoId = dto.TipoMovimentoId,
                ProdutosMovimento = new List<ProdutoMovimento>()
            };

            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync(); // gera IdMovimento

            foreach (var p in dto.Produtos)
            {
                // Verifica se Produto existe
                var produtoExiste = await _context.Produtos.AnyAsync(prod => prod.IdProduto == p.ProdutoId);
                if (!produtoExiste)
                    return BadRequest($"Produto {p.ProdutoId} não encontrado.");

                // Verifica se Embalagem existe
                var embalagemExiste = await _context.ProdutosEmbalagem.AnyAsync(pe => pe.IdProdutoEmbalagem == p.ProdutoEmbalagemId);
                if (!embalagemExiste)
                    return BadRequest($"Embalagem {p.ProdutoEmbalagemId} não encontrada.");

                var produtoMovimento = new ProdutoMovimento
                {
                    MovimentoId = movimento.IdMovimento,
                    ProdutoId = p.ProdutoId,
                    ProdutoEmbalagemId = p.ProdutoEmbalagemId,
                    Quantidade = p.Quantidade
                };

                movimento.ProdutosMovimento.Add(produtoMovimento);
                _context.ProdutosMovimento.Add(produtoMovimento);

                // 🔹 Registrar histórico e atualizar saldo
                await _estoqueRepo.RegistrarMovimentacaoAsync(
                    p.ProdutoId,
                    movimento.IdMovimento,
                    movimento.TipoMovimentoId,
                    p.ProdutoEmbalagemId,
                    p.Quantidade,
                    movimento.Observacao
                );
            }

            await _context.SaveChangesAsync();

            var result = await _context.Movimentos
                .Where(m => m.IdMovimento == movimento.IdMovimento)
                .Select(m => new MovimentoOutputDTO
                {
                    IdMovimento = m.IdMovimento,
                    Documento = m.Documento,
                    Observacao = m.Observacao,
                    DataMovimento = m.DataMovimento,
                    Pessoa = m.Pessoa.NomeRazao,
                    TipoMovimento = m.TipoMovimento.DescricaoTipoMovimento,
                    Produtos = m.ProdutosMovimento.Select(pmo => new ProdutoMovimentoOutputDTO
                    {
                        IdProdutoMovimento = pmo.IdProdutoMovimento,
                        Quantidade = pmo.Quantidade,
                        Produto = pmo.Produto.NomeProduto,
                        Embalagem = pmo.ProdutoEmbalagem.TipoEmbalagem.DescricaoTipoEmbalagem
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
