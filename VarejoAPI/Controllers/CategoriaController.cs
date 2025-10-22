using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Repositories;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly ICategoriaRepository _categoriaRepository;


        public CategoriaController (ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
           _categoriaRepository = categoriaRepository;
        }


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var categorias = await _categoriaRepository.GetAllAsync();


            var resultado = new List<CategoriaOutputDTO>();

            foreach (var categoria in categorias)
            {



                resultado.Add(new CategoriaOutputDTO
                {
                    IdCategoria = categoria.IdCategoria,
                    DescricaoCategoria = categoria.DescricaoCategoria

                });
            }

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
             var categoria = await _categoriaRepository.GetByIdAsync(id);

            if (categoria == null)
                return NotFound();

            var resultado = new List<CategoriaOutputDTO>();


            resultado.Add(new CategoriaOutputDTO
            {
                IdCategoria = categoria.IdCategoria,
                DescricaoCategoria = categoria.DescricaoCategoria
            });

            return Ok(resultado);
        }

    }
}
