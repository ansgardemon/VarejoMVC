using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VarejoDbContext _context;
        public UsuarioRepository(VarejoDbContext context)
        {
            _context = context;
        }

        //CREATE
        public async Task AddAsync(Usuario usuario)
        {
            await _context.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        //READ
        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario).ToListAsync();
        }

        //READ - ID
        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario)
                                 .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        //READ - ATIVOS
        public async Task<Usuario> GetAllAtivosAsync()
        {
            return await _context.Usuarios
                                 .FirstOrDefaultAsync(u => u.Ativo == true);
        }

        //UPDATE
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        //DELETE - OFF

        //INATIVAR
        public async Task InativarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && usuario.Ativo)
            {
                usuario.Ativo = false;
                await _context.SaveChangesAsync();
            }
        }

        //REATIVAR
        public async Task ReativarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && !usuario.Ativo)
            {
                usuario.Ativo = true;
                await _context.SaveChangesAsync();
            }
        }

        //VALIDAR LOGIN
        public async Task<Usuario>? ValidarLoginAsync(string email, string senha)
        {
            return await _context.Usuarios.Include(u => u.TipoUsuario).FirstOrDefaultAsync(u => u.nomeUsuario == email && u.Senha == senha);

        }
    }
}
