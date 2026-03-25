using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {

        private readonly VarejoDbContext _context;

        public MovimentoController(VarejoDbContext context)
        {
            _context = context;
        }

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

        [HttpPost]
        public async Task<ActionResult> Post(MovimentoInputDTO dto)
        {
            if (dto.Produtos == null || !dto.Produtos.Any())
                return BadRequest("O movimento deve ter pelo menos um produto.");

            var movimento = new Movimento
            {
               
                Documento = dto.Documento,
                Observacao = dto.Observacao,
                DataMovimento = dto.DataMovimento ?? DateTime.Now,
                PessoaId = dto.PessoaId,
                TipoMovimentoId = dto.TipoMovimentoId,

                ProdutosMovimento = dto.Produtos.Select(p => new ProdutoMovimento
                {
                    ProdutoId = p.ProdutoId,
                    ProdutoEmbalagemId = p.ProdutoEmbalagemId,
                    Quantidade = p.Quantidade
                }).ToList()
            };

            _context.Movimentos.Add(movimento);
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

