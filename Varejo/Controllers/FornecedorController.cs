using Microsoft.AspNetCore.Mvc;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class FornecedorController : Controller
    {
        private readonly IFornecedorConfigRepository _configRepo;
        private readonly IPessoaRepository _pessoaRepo;
        private readonly IFamiliaRepository _familiaRepo;

        public FornecedorController(
            IFornecedorConfigRepository configRepo,
            IPessoaRepository pessoaRepo,
            IFamiliaRepository familiaRepo)
        {
            _configRepo = configRepo;
            _pessoaRepo = pessoaRepo;
            _familiaRepo = familiaRepo;
        }

        // 1. Listagem de Fornecedores com busca por termo
        public async Task<IActionResult> Index(string termo)
        {
            List<Pessoa> fornecedores = new List<Pessoa>();

            // Só busca se o termo não for vazio e tiver pelo menos 3 letras
            if (!string.IsNullOrWhiteSpace(termo) && termo.Length >= 3)
            {
                fornecedores = await _pessoaRepo.SearchFornecedoresAsync(termo);
            }
            else if (!string.IsNullOrWhiteSpace(termo))
            {
                TempData["MensagemAviso"] = "Digite pelo menos 3 caracteres para pesquisar.";
            }

            ViewData["TermoBusca"] = termo;
            return View(fornecedores);
        }

        // 2. Tela para gerenciar quais FAMÍLIAS o fornecedor atende
        public async Task<IActionResult> Gerenciar(int id)
        {
            var fornecedor = await _pessoaRepo.GetByIdAsync(id);
            if (fornecedor == null) return NotFound();

            var familiasVinculadas = await _configRepo.GetFamiliasPorFornecedorAsync(id);

            // Precisamos de todas as famílias para o Dropdown de novo vínculo
            var todasFamilias = await _familiaRepo.GetAllAsync();

            ViewBag.Fornecedor = fornecedor;
            ViewBag.FamiliasDisponiveis = todasFamilias.Where(f => !familiasVinculadas.Any(fv => fv.IdFamilia == f.IdFamilia));

            return View(familiasVinculadas);
        }

        // 3. A Grade de Produtos (Onde inserimos os códigos <cProd>)
        [HttpGet]
        public async Task<IActionResult> ConfigurarGrade(int pessoaId, int familiaId)
        {
            var fornecedor = await _pessoaRepo.GetByIdAsync(pessoaId);
            // O GetById da família já deve vir com os .Produtos incluídos
            var familia = await _familiaRepo.GetByIdAsync(familiaId);

            if (fornecedor == null || familia == null) return NotFound();

            // Buscamos o que já foi mapeado anteriormente
            var vinculosExistentes = await _configRepo.GetVinculosPorFornecedorAsync(pessoaId);

            var viewModel = new VinculoGradeViewModel
            {
                PessoaId = pessoaId,
                NomeFornecedor = fornecedor.NomeRazao,
                FamiliaId = familiaId,
                NomeFamilia = familia.NomeFamilia,
                Itens = familia.Produtos.Select(p => new ItemVinculoViewModel
                {
                    ProdutoId = p.IdProduto,
                    NomeProduto = p.NomeProduto,
                    // Preenche se já existir vínculo na tabela ProdutoFornecedorVinculo
                    CodigoNoFornecedor = vinculosExistentes
                        .FirstOrDefault(v => v.ProdutoId == p.IdProduto)?.CodigoProdutoNoFornecedor,
                    DescricaoNoFornecedor = vinculosExistentes
                        .FirstOrDefault(v => v.ProdutoId == p.IdProduto)?.DescricaoNoFornecedor
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarGrade(VinculoGradeViewModel model)
        {
            if (model.Itens == null || !model.Itens.Any())
            {
                TempData["MensagemErro"] = "Nenhum produto encontrado para vincular.";
                return RedirectToAction(nameof(Gerenciar), new { id = model.PessoaId });
            }

            // Chama o seu SalvarLoteVinculosAsync no Repositório
            var sucesso = await _configRepo.SalvarLoteVinculosAsync(model.PessoaId, model.Itens);

            if (sucesso)
            {
                TempData["MensagemSucesso"] = $"Vínculos da família {model.NomeFamilia} atualizados!";
            }
            else
            {
                TempData["MensagemErro"] = "Não houve alterações ou ocorreu um erro ao salvar.";
            }

            return RedirectToAction(nameof(Gerenciar), new { id = model.PessoaId });
        }

        [HttpGet]
        public async Task<JsonResult> BuscarFamilias(string termo)
        {
            var familias = await _familiaRepo.SearchFamiliasAsync(termo);

            // Retornamos apenas o necessário para o card de seleção
            var resultado = familias.Select(f => new {
                idFamilia = f.IdFamilia,
                nomeFamilia = f.NomeFamilia,
                categoria = f.Categoria?.DescricaoCategoria ?? "Sem Categoria"
            });

            return Json(resultado);
        }


        // Métodos auxiliares de POST para vincular/desvincular família
        [HttpPost]
        public async Task<IActionResult> VincularFamilia(int pessoaId, int familiaId)
        {
            await _configRepo.VincularFamiliaAsync(pessoaId, familiaId);
            return RedirectToAction(nameof(Gerenciar), new { id = pessoaId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoverFamilia(int pessoaId, int familiaId)
        {
            await _configRepo.RemoverFamiliaAsync(pessoaId, familiaId);
            return RedirectToAction(nameof(Gerenciar), new { id = pessoaId });
        }
    }
}