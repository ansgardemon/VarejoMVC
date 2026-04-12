using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using BCrypt.Net; // Adicione este using!

namespace Varejo.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VarejoDbContext _context;
        public UsuarioRepository(VarejoDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task AddAsync(Usuario usuario)
        {
            // Criptografa a senha antes de salvar no banco
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        // READ (Mantido igual)
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario).Include(f => f.Pessoa).ToListAsync();
        }

        // READ - ID (Mantido igual)
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario).Include(f => f.Pessoa).FirstOrDefaultAsync(f => f.IdUsuario == id);
        }

        // READ - ATIVOS (Mantido igual)
        public async Task<Usuario> GetAllAtivosAsync()
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Ativo == true);
        }

        // UPDATE
        public async Task UpdateAsync(Usuario usuario)
        {
            // Precisamos garantir que não vamos criptografar uma senha que já está criptografada.
            // Se a senha não vier nula/vazia (ou seja, o usuário digitou uma nova na tela de edição):
            if (!string.IsNullOrEmpty(usuario.Senha) && !usuario.Senha.StartsWith("$2a$") && !usuario.Senha.StartsWith("$2b$") && !usuario.Senha.StartsWith("$2y$"))
            {
                // Criptografa a NOVA senha
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            }

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        // INATIVAR (Mantido igual)
        public async Task InativarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && usuario.Ativo)
            {
                usuario.Ativo = false;
                await _context.SaveChangesAsync();
            }
        }

        // REATIVAR (Mantido igual)
        public async Task ReativarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && !usuario.Ativo)
            {
                usuario.Ativo = true;
                await _context.SaveChangesAsync();
            }
        }

        // VALIDAR LOGIN - REFATORADO!
        public async Task<Usuario?> ValidarLoginAsync(string nomeUsuario, string senha)
        {
            // 1. Buscamos APENAS pelo nome de usuário e se está ativo. 
            // O banco não sabe reverter o Hash, então não podemos passar a senha no "Where".
            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.Pessoa)
                .FirstOrDefaultAsync(u => u.nomeUsuario == nomeUsuario && u.Ativo);

            // Se não encontrou o login ou está inativo, retorna nulo.
            if (usuario == null)
                return null;

            // 2. O BCrypt compara a senha em texto puro digitada com o Hash do banco de dados
            bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);

            // 3. Se a senha for válida, devolvemos o usuário. Se não, devolvemos nulo (Acesso Negado).
            return senhaValida ? usuario : null;
        }

        public List<TipoUsuario> GetTiposUsuario()
        {
            return _context.TiposUsuario.ToList();
        }

        public List<Pessoa> GetPessoa()
        {
            return _context.Pessoas.ToList();
        }
    }
}