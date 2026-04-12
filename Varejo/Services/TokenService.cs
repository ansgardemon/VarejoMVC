using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Varejo.Models;

namespace Varejo.Services
{
    public class TokenService
    {
        // ATENÇÃO: Em produção, coloque essa chave no appsettings.json!
        // A chave precisa ter pelo menos 32 caracteres para ser segura no SHA256.
        private readonly string _secretKey = "SuperChaveSecretaDoVitrinOVarejo2024!@#";

        public string GenerateToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // 1. Aqui definimos as "Claims" (informações carimbadas no token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.nomeUsuario),
                
                // INCLUÍDO O PESSOA ID (Fundamental para o PDV e Relatórios!)
                new Claim("PessoaId", usuario.PessoaId.ToString()),
                
                // Pegamos o nível de acesso que você já tem modelado
                new Claim(ClaimTypes.Role, usuario.TipoUsuario?.DescricaoTipoUsuario ?? "Visitante")
            };

            // 2. Configura a validade e a assinatura do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12), // Duração do token ajustada para cobrir um turno inteiro + horas extras
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // 3. Gera e retorna o token em formato string (eyJh...)
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}