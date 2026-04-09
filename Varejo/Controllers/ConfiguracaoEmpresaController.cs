using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class ConfiguracaoEmpresaController : Controller
    {
        private readonly IConfiguracaoEmpresaRepository _empresaRepo;

        public ConfiguracaoEmpresaController(IConfiguracaoEmpresaRepository empresaRepo)
        {
            _empresaRepo = empresaRepo;
        }

        // GET: Configuracoes/Empresa
        public async Task<IActionResult> Empresa()
        {
            // 1. Busca a Model do banco
            var config = await _empresaRepo.GetConfiguracaoAsync();

            // 2. Cria a ViewModel que a View espera
            var viewModel = new ConfiguracaoEmpresaViewModel();

            // 3. Se já existe dado no banco, mapeia para a ViewModel
            if (config != null)
            {
                viewModel.Id = config.Id;
                viewModel.RazaoSocial = config.RazaoSocial;
                viewModel.NomeFantasia = config.NomeFantasia;
                viewModel.Cnpj = config.Cnpj;
                viewModel.UrlLogotipo = config.Logotipo;
            }

            // 4. Envia a VIEWMODEL (e não a Model) para a View
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Empresa(ConfiguracaoEmpresaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 1. Tratar a Imagem (Igual ao ProdutoController)
                string urlLogotipo = viewModel.UrlLogotipo; // Mantém a atual se não subir nova

                if (viewModel.LogotipoUpload != null)
                {
                    // Gera nome único para evitar cache ou nomes duplicados
                    var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.LogotipoUpload.FileName);
                    var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/logos", nomeArquivo);

                    // Garante que a pasta existe
                    Directory.CreateDirectory(Path.GetDirectoryName(caminho));

                    using var stream = new FileStream(caminho, FileMode.Create);
                    await viewModel.LogotipoUpload.CopyToAsync(stream);

                    urlLogotipo = "/img/logos/" + nomeArquivo;
                }
                else if (string.IsNullOrEmpty(urlLogotipo))
                {
                    // Fallback caso não tenha logo nenhuma
                    urlLogotipo = "/img/sem-imagem.png";
                }

                // 2. Mapear ViewModel para a Model
                var config = new ConfiguracaoEmpresa
                {
                    Id = viewModel.Id,
                    RazaoSocial = viewModel.RazaoSocial,
                    NomeFantasia = viewModel.NomeFantasia,
                    Cnpj = viewModel.Cnpj,
                    Logotipo = urlLogotipo // Salva o caminho final
                };

                var sucesso = await _empresaRepo.SalvarConfiguracaoAsync(config);
                if (sucesso)
                {
                    TempData["MensagemSucesso"] = "Configurações salvas com sucesso!";
                    return RedirectToAction(nameof(Empresa));
                }

                ModelState.AddModelError("", "Erro ao salvar no banco de dados.");
            }

            return View(viewModel);
        }
    }
}