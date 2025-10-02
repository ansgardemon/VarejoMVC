using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository, ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }

        // ====================LOGIN=====LOGOUT====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Usuario");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = await _usuarioRepository.ValidarLoginAsync(email, senha);
            if (usuario == null || !usuario.Ativo)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View();
            }

            string role = NormalizeRole(usuario?.TipoUsuario?.DescricaoTipoUsuario);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.nomeUsuario ?? "Usuário"),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);

            await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(30)
                });

            // após logar somos redircionado para a index
            return RedirectToAction("Index", "Usuario");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //apaga os cookies de antiforgery
            Response.Cookies.Delete("Vitrino.AntiCsfr");


            return RedirectToAction("Index", "Home", new { _ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        }

        [HttpGet]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Vitrino.AntiCsfr");
            return RedirectToAction("Index", "Home", new { _ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AcessoNegado() => View();

        // ====================CRUD====================

        //READ
        [Authorize]
        public async Task<IActionResult> Index(int? tipoUsuarioId, string? search)
        {
            var usuarios = await _usuarioRepository.GetAllAtivosAsync();

            if (tipoUsuarioId.HasValue && tipoUsuarioId > 0)
                usuarios = usuarios.Where(u => u.TipoUsuarioId == tipoUsuarioId).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                usuarios = usuarios.Where(u =>
                    (u.nomeUsuario ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            usuarios = usuarios.OrderByDescending(u => u.nomeUsuario).ToList();

            ViewBag.TiposUsuario = new SelectList(await _tipoUsuarioRepository.GetAllAsync(),
                "IdTipoUsuario", "DescricaoTipoUsuario");
            ViewBag.FiltroTipoId = tipoUsuarioId;
            ViewBag.TermoBusca = search;

            return View(usuarios);
        }

        //CREATE
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Create()
        {
            var vm = await CriarUsuarioViewModel();
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var vm = await CriarUsuarioViewModel(viewModel);
                return View(vm);
            }

            var usuario = new Usuario
            {
                nomeUsuario = viewModel.nomeUsuario,
                Senha = viewModel.Senha,
                TipoUsuarioId = viewModel.TipoUsuarioId,
                Ativo = true
            };

            await _usuarioRepository.AddAsync(usuario);
            return RedirectToAction(nameof(Index));
        }

        //UPDATE
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            var vm = new UsuarioViewModel
            {
                IdUsuario = usuario.IdUsuario,
                nomeUsuario = usuario.nomeUsuario,
                Senha = usuario.Senha,
                TipoUsuarioId = usuario.TipoUsuarioId,
                TiposUsuario = (await _tipoUsuarioRepository.GetAllAsync()).Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = t.IdTipoUsuario.ToString(),
                    Text = t.DescricaoTipoUsuario
                })
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
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

            usuario.nomeUsuario = viewModel.nomeUsuario;
            usuario.Senha = viewModel.Senha;
            usuario.TipoUsuarioId = viewModel.TipoUsuarioId;

            await _usuarioRepository.UpdateAsync(usuario);
            return RedirectToAction(nameof(Index));
        }

        //DELETE - OFF

        //INATIVOS
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Inativos()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var inativos = usuarios.Where(u => !u.Ativo)
                                   .OrderByDescending(u => u.IdUsuario)
                                   .ToList();
            return View(inativos);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Ativar(int id)
        {
            if (id <= 0) return NotFound();

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.Ativo = true;
            await _usuarioRepository.UpdateAsync(usuario);

            return RedirectToAction(nameof(Inativos));
        }

        private async Task<UsuarioViewModel> CriarUsuarioViewModel(UsuarioViewModel? model = null)
        {
            var tipos = await _tipoUsuarioRepository.GetAllAsync();
            return new UsuarioViewModel
            {
                IdUsuario = model?.IdUsuario ?? 0,
                nomeUsuario = model?.nomeUsuario,
                Senha = model?.Senha,
                TipoUsuarioId = model?.TipoUsuarioId ?? 0,
                TiposUsuario = tipos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoUsuario.ToString(),
                    Text = t.DescricaoTipoUsuario
                })
            };
        }


        // ====================APOIO====================
        private static string NormalizeRole(string? raw)
        {
            var r = (raw ?? "").Trim().ToLowerInvariant();
            return r switch
            {
                "administrador" or "admin" => "Administrador",
                "gerente" or "manager" => "Gerente",
                _ => "Outros"
            };
        }
    }
}