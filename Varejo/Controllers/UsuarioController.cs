using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


        public async Task<UsuarioViewModel> CriarUsuarioViewModel(UsuarioViewModel? model = null)
        {
            return new UsuarioViewModel
            {
                IdUsuario = model?.IdUsuario ?? 0,
                nomeUsuario = model?.nomeUsuario,
                Senha = model?.Senha,
                Ativo = model?.Ativo ?? true,
                PessoaId = model?.PessoaId ?? 0,
                TipoUsuarioId = model?.TipoUsuarioId ?? 0
            };
        }

        public async Task<IActionResult> Index(int? tipoUsuarioId, string? search)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            if (tipoUsuarioId.HasValue && tipoUsuarioId.Value > 0)
                usuarios = usuarios.Where(u => u.TipoUsuarioId == tipoUsuarioId).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                usuarios = usuarios.Where(u =>
                    u.nomeUsuario.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (int.TryParse(search, out int idBusca) && u.IdUsuario == idBusca)
                ).ToList();
            }

            usuarios = usuarios.OrderByDescending(u => u.IdUsuario).ToList();

            ViewBag.TiposUsuario = new SelectList(await _tipoUsuarioRepository.GetAllAsync(),
                "IdTipoUsuario", "DescricaoTipoUsuario");
            ViewBag.FiltroTipoId = tipoUsuarioId;
            ViewBag.TermoBusca = search;

            return View(usuarios);
        }

        public async Task<IActionResult> Create()
        {
            var vm = await CriarUsuarioViewModel();
            return View(vm);
        }
        [HttpPost]

        public async Task<IActionResult> Create(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = await CriarUsuarioViewModel();
                return View(vm);
            }

            var usuario = new Usuario
            {
                IdUsuario = model?.IdUsuario ?? 0,
                nomeUsuario = model.nomeUsuario,
                Senha = model.Senha,
                Ativo = model.Ativo,
                PessoaId = model.PessoaId,
                TipoUsuarioId = model.TipoUsuarioId,
            };

            await _usuarioRepository.AddAsync(usuario);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            var vm = new UsuarioViewModel
            {

                IdUsuario = usuario?.IdUsuario ?? 0,
                nomeUsuario = usuario.nomeUsuario,
                Senha = usuario.Senha,
                Ativo = usuario.Ativo,
                PessoaId = usuario.PessoaId,
                TipoUsuario = usuario.TipoUsuario,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioViewModel viewModel)
        {
            if (id != viewModel.IdUsuario) return NotFound();

            if (!ModelState.IsValid)
            {
                viewModel = await CriarUsuarioViewModel(viewModel);
                return View(viewModel);
            }

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.IdUsuario = usuario?.IdUsuario ?? 0;
            usuario.nomeUsuario = usuario.nomeUsuario;
            usuario.Senha = viewModel.Senha;
            usuario.Ativo = usuario.Ativo;
            usuario.TipoUsuarioId = viewModel.TipoUsuarioId;
            usuario.PessoaId = viewModel.PessoaId;

            await _usuarioRepository.UpdateAsync(usuario);
            return RedirectToAction(nameof(Index));
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
