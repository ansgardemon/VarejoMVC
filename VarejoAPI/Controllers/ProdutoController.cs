using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using VarejoAPI.DTO;

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
            var produtos = await _produtoRepository.GetAllDetailedAsync();


            var resultado = new List<ProdutoDTO>();

            foreach (var produto in produtos)
            {



                resultado.Add(new ProdutoDTO
                {
                    IdFamilia = produto.Familia.IdFamilia,
                    NomeFamilia = produto.Familia.NomeFamilia,
                    IdProduto = produto.IdProduto,
                    Complemento = produto.Complemento,
                    NomeProduto = produto.NomeProduto,
                    EstoqueInicial = produto.EstoqueInicial,
                    EstoqueAtual = produto.EstoqueAtual,
                    Ativo = produto.Ativo,
                    DescricaoCategoria = produto.Familia.Categoria.DescricaoCategoria,
                    NomeMarca = produto.Familia.Marca.NomeMarca,
                    UrlImagem = produto.UrlImagem,
                    CustoMedio = produto.CustoMedio,
          

                });
            }

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _produtoRepository.GetByIdDetailedAsync(id);
            if (produto == null)
                return NotFound();

            var resultado = new ProdutoDTO
            {
                  IdFamilia = produto.Familia.IdFamilia,
                    NomeFamilia = produto.Familia.NomeFamilia,
                    IdProduto = produto.IdProduto,
                    Complemento = produto.Complemento,
                    NomeProduto = produto.NomeProduto,
                    EstoqueInicial = produto.EstoqueInicial,
                    EstoqueAtual = produto.EstoqueAtual,
                    Ativo = produto.Ativo,
                    DescricaoCategoria = produto.Familia.Categoria.DescricaoCategoria,
                    NomeMarca = produto.Familia.Marca.NomeMarca,
                    UrlImagem = produto.UrlImagem,
                    CustoMedio = produto.CustoMedio,
            };

            return Ok(resultado);
        }

        [HttpGet("destaques")]
        public async Task<ActionResult<List<ProdutoCardViewModel>>> GetDestaques()
        {
            var produtos = await _produtoRepository.GetProdutosDestaqueAsync(8);

            var resultado = produtos.Select(p => new ProdutoCardViewModel
            {
                IdProduto = p.IdProduto,
                NomeProduto = p.NomeProduto,
                UrlImagem = p.UrlImagem,
                //escolher menor preço entre embalagens
                Preco = p.ProdutosEmbalagem != null && p.ProdutosEmbalagem.Any()
                    ? p.ProdutosEmbalagem.Min(e => e.Preco)
                    : 0m
            }).ToList();

            return Ok(resultado);
        }

        [HttpGet("catalogo")]
        public async Task<ActionResult<IEnumerable<ProdutoCatalogoViewModel>>> GetCatalogo(
        int? IdCategoria = null, int? IdMarca = null)
        {
            try
            {
                var produtos = await _produtoRepository.GetProdutosCatalogoAsync(IdCategoria, IdMarca);

                if (produtos == null || !produtos.Any())
                    return NotFound("Nenhum produto encontrado.");

                var resultado = produtos.Select(p => new ProdutoCatalogoViewModel
                {
                    IdProduto = p.IdProduto,
                    IdCategoria = p.Familia?.Categoria?.IdCategoria ?? 0,
                    IdMarca = p.Familia?.Marca?.IdMarca ?? 0,
                    NomeProduto = p.NomeProduto,
                    UrlImagem = p.UrlImagem,
                    Preco = p.ProdutosEmbalagem
                        .Where(pe => pe.Preco > 0)
                        .Min(pe => pe.Preco)
                });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }



        [HttpGet("categoria/{idCategoria}")]
        public async Task<IActionResult> Details(int idCategoria)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(idCategoria);

            if (categoria == null)
                return NotFound();

            var viewModel = new CategoriaOutputDTO
            {
                IdCategoria = categoria.IdCategoria,
                DescricaoCategoria = categoria.DescricaoCategoria ?? string.Empty,

                Familias = categoria.Familias?
                    .Where(f => f != null)
                    .Select(f =>
                    {
                        var produtos = f.Produtos?
                            .Where(p => p != null)
                            .Select(p => new ProdutoOutputDTO
                            {
                                IdProduto = p.IdProduto,
                                NomeProduto = p.NomeProduto ?? string.Empty,
                                Complemento = p.Complemento ?? string.Empty,
                                EstoqueInicial = p.EstoqueInicial,
                                Ativo = p.Ativo,
                                UrlImagem = p.UrlImagem ?? string.Empty,
                                CustoMedio = p.CustoMedio,
                                FamiliaId = p.FamiliaId
                            })
                            .ToList() ?? new List<ProdutoOutputDTO>();

                        return new FamiliaOutputDTO
                        {
                            IdFamilia = f.IdFamilia,
                            NomeFamilia = f.NomeFamilia ?? string.Empty,
                            Produtos = produtos
                        };
                    })
                    .ToList() ?? new List<FamiliaOutputDTO>()
            };

            return Ok(viewModel);
        }


        [HttpGet("familia/{idFamilia}")]
        public async Task<ActionResult> GetFamilia(int idFamilia)
        {
            var familia = await _produtoRepository.GetByFamilia(idFamilia);

            var resultado = new List<ProdutoOutputDTO>();

            foreach (var produto in familia)
            {
                resultado.Add(new ProdutoOutputDTO
                {
                    IdProduto = produto.IdProduto,
                    Complemento = produto.Complemento,
                    NomeProduto = produto.NomeProduto,
                    EstoqueInicial = produto.EstoqueInicial,
                    Ativo = produto.Ativo,
                    UrlImagem = produto.UrlImagem,
                    CustoMedio = produto.CustoMedio,
                    FamiliaId = produto.FamiliaId,
                });
            }

            return Ok(resultado);

        }



    }
}
