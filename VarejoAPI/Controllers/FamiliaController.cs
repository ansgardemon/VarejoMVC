using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliaController : ControllerBase
    {

        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IFamiliaRepository _familiaRepository;

        public FamiliaController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IFamiliaRepository familiaRepository)
        {
            _produtoRepository = produtoRepository;
            _categoriaRepository = categoriaRepository;
            _familiaRepository = familiaRepository;


        }


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var familias = await _familiaRepository.GetAllAsync();

            var resultado = new List<FamiliaOutputDTO>();

            foreach (var familia in familias)
            {
                resultado.Add(new FamiliaOutputDTO
                {
                    IdFamilia = familia.IdFamilia,
                    NomeFamilia = familia.NomeFamilia, 
                    Ativo = familia.Ativo, 
                    CategoriaId = familia.CategoriaId.ToString(), 
                    MarcaId = familia.MarcaId?.ToString(), 
                   
                });
            }

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);

            if (familia == null)
                return NotFound();

            var resultado = new List<FamiliaOutputDTO>();


            resultado.Add(new FamiliaOutputDTO
            {
                IdFamilia = familia.IdFamilia,
                NomeFamilia = familia.NomeFamilia,
                Ativo = familia.Ativo,
                CategoriaId = familia.CategoriaId.ToString(),
                MarcaId = familia.MarcaId?.ToString(),
            });

            return Ok(resultado);
        }



        [HttpGet("familia/{idCategoria}")]
        public async Task<ActionResult> GetFamilia(int idCategoria)
        {
            var familias = await _familiaRepository.GetByCategory(idCategoria);
            //var familias = await _familiaRepository.GetByCategory(idCategoria);

            var resultado = new List<FamiliaOutputDTO>();

            foreach (var familia in familias) // aqui usar "familia" do loop
            {
                resultado.Add(new FamiliaOutputDTO
                {
                    IdFamilia = familia.IdFamilia,
                    NomeFamilia = familia.NomeFamilia,
                    Ativo = familia.Ativo,
                    CategoriaId = familia.CategoriaId.ToString(),
                    MarcaId = familia.MarcaId?.ToString(),
                    
                });
            }

            return Ok(resultado);
        }







    }
}
