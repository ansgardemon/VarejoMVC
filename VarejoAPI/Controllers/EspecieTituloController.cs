using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspecieTituloController : ControllerBase
    {

            private readonly IEspecieTituloRepository _iEspecieTituloRepository;

            public EspecieTituloController(IEspecieTituloRepository iEspecieTituloRepository)
            {
                _iEspecieTituloRepository = iEspecieTituloRepository;
            }

            [HttpGet]
            public async Task<ActionResult> GetAll()
            {
                var lista = await _iEspecieTituloRepository.GetAllAsync();

                var result = lista.Select(e => new EspecieTitulo
                {
                    IdEspecieTitulo = e.IdEspecieTitulo,
                    Descricao = e.Descricao
                });

                return Ok(result);
            }

         
            [HttpGet("{id}")]
            public async Task<ActionResult> GetById(int id)
            {
                var model = await _iEspecieTituloRepository.GetByIdAsync(id);

                if (model == null)
                    return NotFound();

                var result = new EspecieTitulo
                {
                    IdEspecieTitulo = model.IdEspecieTitulo,
                    Descricao = model.Descricao
                };

                return Ok(result);
            }

        


        }
    }

