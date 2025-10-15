using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.Repositories;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {

        private readonly IProdutoRepository _produtoRepository;
        private readonly IFamiliaRepository _familiaRepository;
        private readonly IProdutoEmbalagemRepository _produtoEmbalagemRepository;

        public ProdutoController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
            _produtoRepository = produtoRepository;
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

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);

            if (produto == null)
                return NotFound();

            var resultado = new List<ProdutoOutputDTO>();

            resultado.Add(new ProdutoOutputDTO
            {
                IdProduto = produto.IdProduto,
                Complemento = produto.Complemento,
                NomeProduto = produto.NomeProduto,
                EstoqueInicial = produto.EstoqueInicial,
                Ativo = produto.Ativo,
                UrlImagem = produto.UrlImagem,
                CustoMedio = produto.CustoMedio,
                FamiliaId = produto.FamiliaId
            });

            return Ok(resultado);
        }



        [HttpGet("{idProduto}/Categoria")]
        public async Task<ActionResult> GetId(int idProduto)
        {
            // Buscar produto(s) relacionados à categoria do produto
            var produto = await _produtoRepository.GetByIdAsync(idProduto);

            if (produto == null)
                return NotFound();

            var resultado = new List<ProdutoOutputDTO>();

            // Preenche apenas os campos necessários do banco
            resultado.Add(new ProdutoOutputDTO
            {
                IdProduto = produto.IdProduto,
                Complemento = produto.Complemento,
                NomeProduto = produto.NomeProduto,
                EstoqueInicial = produto.EstoqueInicial,
                Ativo = produto.Ativo,
                UrlImagem = produto.UrlImagem,
                CustoMedio = produto.CustoMedio,
                FamiliaId = produto.FamiliaId
            });

            return Ok(resultado);
        }

    }
}
