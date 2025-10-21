using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.ViewModels;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCategoria()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            var DTOlst = categorias.Select(c => new CategoriaOutputDTO
            {
                IdCategoria = c.IdCategoria,
                DescricaoCategoria = c.DescricaoCategoria,
            }).ToList();

            return Ok(DTOlst);
        }    


    }
}
