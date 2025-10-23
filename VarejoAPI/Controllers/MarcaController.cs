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
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaRepository _marcaRepository;
        public MarcaController(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Get() 
        {

            var marcas = await _marcaRepository.GetAllAsync();


            var resultado = new List<MarcaOutputDOT>();

            foreach (var marca in marcas)
            {

                resultado.Add(new MarcaOutputDOT
                {
                    IdMarca = marca.IdMarca,
                    NomeMarca = marca.NomeMarca
                });
            }
            return Ok(resultado);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var marca = await _marcaRepository.GetByIdAsync(id);

            if (marca == null)
                return NotFound();

            var resultado = new List<MarcaOutputDOT>();


            resultado.Add(new MarcaOutputDOT
            {
                IdMarca = marca.IdMarca,
                NomeMarca = marca.NomeMarca
            });

            return Ok(resultado);
        }



    }
}
        
   