using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using VarejoAPI.DTO;

namespace VarejoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var resultado = new List<UsuarioOutputDTO>();

            foreach (var usuario in usuarios)
            {
                resultado.Add(new UsuarioOutputDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    nomeUsuario = usuario.nomeUsuario,
                    Senha = usuario.Senha,
                    Ativo = usuario.Ativo,
                    PessoaId = usuario.PessoaId,
                    TipoUsuarioId = usuario.TipoUsuarioId
                });
            }

            return Ok(resultado);

        }




        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            var resultado = new List<UsuarioOutputDTO>();

                resultado.Add(new UsuarioOutputDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    nomeUsuario = usuario.nomeUsuario,
                    Senha = usuario.Senha,
                    Ativo = usuario.Ativo,
                    PessoaId = usuario.PessoaId,
                    TipoUsuarioId = usuario.TipoUsuarioId
                });
            

            return Ok(resultado);

        }





    }
}
