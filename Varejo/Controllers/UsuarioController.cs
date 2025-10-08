using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.Repositories;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        private readonly IPessoaRepository _pessoaRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository, IPessoaRepository pessoaRepository, ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _pessoaRepository = pessoaRepository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }


        public async Task<IActionResult> Index(int? pessoaid, int? tipoUsuarioId, string search)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            // FILTROS
            if (!string.IsNullOrEmpty(search))
                usuarios = usuarios.Where(f => f.nomeUsuario.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (pessoaid.HasValue)
                usuarios = usuarios.Where(f => f.PessoaId == pessoaid.Value).ToList();

            if (tipoUsuarioId.HasValue)
                usuarios = usuarios.Where(f => f.TipoUsuarioId == tipoUsuarioId.Value).ToList();

            // ViewBag para filtros — agora como SelectList, não List<SelectListItem>
            ViewBag.TipoUsuario = new SelectList(await _usuarioRepository.GetTiposUsuario(), "IdTipoUsuario", "DescricaoTipoUsuario", tipoUsuarioId);
            ViewBag.Search = search;


            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {

            // ViewBag para dropdowns de Marca e Categoria
            ViewBag.TipoUsuario = new SelectList(await _usuarioRepository.GetTiposUsuario(), "IdTipoUsuario", "DescricaoTipoUsuario");

            return View(new UsuarioViewModel());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Pessoas = (await _pessoaRepository.GetAllAsync())
                    .Select(p => new SelectListItem { Value = p.IdPessoa.ToString(), Text = p.NomeRazao });
                vm.TipoUsuarios = (await _tipoUsuarioRepository.GetAllAsync())
                    .Select(t => new SelectListItem { Value = t.IdTipoUsuario.ToString(), Text = t.DescricaoTipoUsuario });
                return View(vm);
            }

            var usuario = new Usuario
            {
                nomeUsuario = vm.nomeUsuario,
                Senha = vm.Senha,
                Ativo = vm.Ativo,
                PessoaId = vm.PessoaId,
                TipoUsuarioId = vm.TipoUsuarioId // 👈 ESSENCIAL
            };

            await _usuarioRepository.AddAsync(usuario);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            var usuariovm = new UsuarioViewModel
            {

                nomeUsuario = usuario.nomeUsuario,
                Senha = usuario.Senha,
                Ativo = usuario.Ativo,
                PessoaId = usuario.PessoaId,
                TipoUsuarioId = usuario.TipoUsuarioId,
                TipoUsuarios = (await _tipoUsuarioRepository.GetAllAsync()).Select(u => new SelectListItem
                {
                    Text = u.DescricaoTipoUsuario,
                    Value = u.IdTipoUsuario.ToString(),
                }),
            };

            return View(usuariovm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioViewModel usuariovm)
        {
            if (usuariovm == null) return NotFound();

            int id = usuariovm.IdUsuario;

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            if (ModelState.IsValid)
            {
                usuario.nomeUsuario = usuariovm.nomeUsuario;
                usuario.Senha = usuariovm.Senha;
                usuario.Ativo = usuariovm.Ativo;
                usuario.PessoaId = usuariovm.PessoaId;
                usuario.TipoUsuarioId = usuariovm.TipoUsuarioId;
                await _usuarioRepository.UpdateAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuariovm);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _usuarioRepository.InativarUsuario(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Inativos()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var inativos = usuarios.Where(u => !u.Ativo).OrderByDescending(u => u.IdUsuario).ToList();
            return View(inativos);
        }
        public async Task<IActionResult> Ativar(int id)
        {
            if (id <= 0) return NotFound();

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.Ativo = true;
            await _usuarioRepository.UpdateAsync(usuario);

            return RedirectToAction(nameof(Inativos));
        }
    }
}