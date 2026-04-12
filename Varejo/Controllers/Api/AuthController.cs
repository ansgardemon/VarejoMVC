// Arquivo: Controllers/Api/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Services;

namespace Varejo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")] // A URL vai ficar: https://seusite/api/auth
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("login")] // URL completa: https://seusite/api/auth/login
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (string.IsNullOrEmpty(model.NomeUsuario) || string.IsNullOrEmpty(model.Senha))
                return BadRequest(new { sucesso = false, mensagem = "Usuário e senha são obrigatórios." });

            // Vai no repositório e checa o BCrypt
            var usuario = await _usuarioRepository.ValidarLoginAsync(model.NomeUsuario, model.Senha);

            if (usuario == null)
                return Unauthorized(new { sucesso = false, mensagem = "Usuário ou senha inválidos." });

            // Gerou o JWT
            var tokenService = new TokenService();
            var tokenString = tokenService.GenerateToken(usuario);

            // Devolve o JSON com o Token para o Blazor guardar no celular
            return Ok(new
            {
                sucesso = true,
                nome = usuario.nomeUsuario,
                permissao = usuario.TipoUsuario?.DescricaoTipoUsuario,
                token = tokenString
            });
        }
    }

    // DTO que representa o JSON enviado pelo Blazor
    public class LoginRequestDTO
    {
        public string NomeUsuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}