using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
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

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioOutputDTO>>> Get()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            var resultado = usuarios.Select(u => new UsuarioOutputDTO
            {
                IdUsuario = u.IdUsuario,
                nomeUsuario = u.nomeUsuario,
                Ativo = u.Ativo,
                PessoaId = u.PessoaId,
                TipoUsuarioId = u.TipoUsuarioId
            });

            return Ok(resultado);
        }

        // GET: api/Usuario/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioOutputDTO>> Get(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var resultado = new UsuarioOutputDTO
            {
                IdUsuario = usuario.IdUsuario,
                nomeUsuario = usuario.nomeUsuario,
                Ativo = usuario.Ativo,
                PessoaId = usuario.PessoaId,
                TipoUsuarioId = usuario.TipoUsuarioId
            };

            return Ok(resultado);
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioOutputDTO>> Post([FromBody] UsuarioInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = new Usuario
            {
                nomeUsuario = dto.nomeUsuario,
                Senha = dto.Senha,
                Ativo = dto.Ativo,
                PessoaId = dto.PessoaId,
                TipoUsuarioId = dto.TipoUsuarioId
            };

            try
            {
                await _usuarioRepository.AddAsync(usuario);

                return CreatedAtAction(nameof(Get), new { id = usuario.IdUsuario }, new UsuarioOutputDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    nomeUsuario = usuario.nomeUsuario,
                    Ativo = usuario.Ativo,
                    PessoaId = usuario.PessoaId,
                    TipoUsuarioId = usuario.TipoUsuarioId
                });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT: api/Usuario/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.nomeUsuario = dto.nomeUsuario;
            usuario.Senha = dto.Senha;
            usuario.Ativo = dto.Ativo;
            usuario.PessoaId = dto.PessoaId;
            usuario.TipoUsuarioId = dto.TipoUsuarioId;

            try
            {
                await _usuarioRepository.UpdateAsync(usuario);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE (INATIVAR): api/Usuario/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Inativar(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound();

            await _usuarioRepository.InativarUsuario(id);
            return NoContent();
        }

        // GET: api/Usuario/Inativos
        [HttpGet("Inativos")]
        public async Task<ActionResult<IEnumerable<UsuarioOutputDTO>>> Inativos()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            var resultado = usuarios
                .Where(u => !u.Ativo)
                .Select(u => new UsuarioOutputDTO
                {
                    IdUsuario = u.IdUsuario,
                    nomeUsuario = u.nomeUsuario,
                    Ativo = u.Ativo,
                    PessoaId = u.PessoaId,
                    TipoUsuarioId = u.TipoUsuarioId
                });

            return Ok(resultado);
        }

        // PUT: api/Usuario/Ativar/5
        [HttpPut("Ativar/{id:int}")]
        public async Task<IActionResult> Ativar(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Ativo = true;
            await _usuarioRepository.UpdateAsync(usuario);

            return NoContent();
        }

        // POST: api/Usuario/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _usuarioRepository.ValidarLoginAsync(dto.nomeUsuario, dto.senha);

            if (usuario == null || !usuario.Ativo)
                return Unauthorized("Usuário ou senha inválidos.");

            return Ok(new
            {
                usuario.IdUsuario,
                usuario.nomeUsuario,
                usuario.PessoaId,
                usuario.TipoUsuarioId
            });
        }
    }
}
