using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;
using static VarejoAPI.DTO.ProdutoOutputDTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {

        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IFamiliaRepository _familiaRepository;

        public ProdutoController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IFamiliaRepository familiaRepository)
        {
            _produtoRepository = produtoRepository;
            _categoriaRepository = categoriaRepository;
            _familiaRepository = familiaRepository;


        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var produtos = await _produtoRepository.GetAllAsync();


            var resultado = new List<ProdutoOutputDTO>();

            foreach (var produto in produtos)
            {


                resultado.Add(new ProdutoOutputDTO
                {
                    IdProduto = produto.IdProduto,
                    NomeProduto = produto.NomeProduto,
                    UrlImagem = produto.UrlImagem,
                    Preco = produto.ProdutosEmbalagem.Min(pe => pe.Preco),
                });
            }

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoCategoriaDTO>> GetByIdAsync(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                return NotFound();

            var resultado = new ProdutoOutputDTO
            {
                IdProduto = produto.IdProduto,
                NomeProduto = produto.NomeProduto,
                Preco = produto.ProdutosEmbalagem.Min(pe => pe.Preco),
                UrlImagem = produto.UrlImagem,
               

            };

            return Ok(resultado);
        }

        [HttpGet("categoria/{id}")]
        public async Task<ActionResult<List<ProdutoCategoriaDTO>>> GetByCategory(int id)
        {
            var produtos = await _produtoRepository.GetByCategory(id);

            if (produtos == null || !produtos.Any())
                return NotFound();

            var resultado = produtos.Select(produto => new ProdutoCategoriaDTO
            {
                IdProduto = produto.IdProduto,
                DescricaoCategoria = produto.Familia.Categoria.DescricaoCategoria ,
                IdCategoria = produto.Familia.Categoria.IdCategoria,
                NomeProduto = produto.NomeProduto,
                Preco = produto.ProdutosEmbalagem.Min(pe => pe.Preco),
                UrlImagem = produto.UrlImagem,
            });

            return Ok(resultado);
        }


        [HttpGet("familia/{idFamilia}")]
        public async Task<ActionResult> GetByFamilia(int idFamilia)
        {
            var familia = await _produtoRepository.GetByFamilia(idFamilia);

            var resultado = new List<ProdutoFamilia>();

            foreach (var produto in familia)
            {
                resultado.Add(new ProdutoFamilia
                {
                    IdProduto = produto.IdProduto,
                    NomeProduto = produto.NomeProduto,
                    Preco = produto.ProdutosEmbalagem.Min(pe => pe.Preco),
                    UrlImagem = produto.UrlImagem,
                    IdFamilia = produto.Familia.IdFamilia,
                    NomeFamilia = produto.Familia.NomeFamilia
                });
            }

            return Ok(resultado);

        }



    }
}
