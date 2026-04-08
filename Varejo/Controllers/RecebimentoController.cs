using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class RecebimentoController : Controller
    {
        private readonly IRecebimentoRepository _recebimentoRepo;
        private readonly IPessoaRepository _pessoaRepo;
        private readonly IFornecedorConfigRepository _vinculoRepo;
        private readonly IPrazoPagamentoRepository _prazoRepo;
        private readonly IFormaPagamentoRepository _formaRepo;
        private readonly IXmlService _xmlService; // Seu serviço de leitura de XML

        public RecebimentoController(
            IRecebimentoRepository recebimentoRepo,
            IPessoaRepository pessoaRepo,
            IFornecedorConfigRepository vinculoRepo,
            IPrazoPagamentoRepository prazoRepo,
            IFormaPagamentoRepository formaRepo,
            IXmlService xmlService)
        {
            _recebimentoRepo = recebimentoRepo;
            _pessoaRepo = pessoaRepo;
            _vinculoRepo = vinculoRepo;
            _prazoRepo = prazoRepo;
            _formaRepo = formaRepo;
            _xmlService = xmlService;
        }

        // 1. Tela Inicial: Apenas para upload do arquivo
        public IActionResult Importar() => View();

        // 2. Ação que lê o XML e joga para a Tela de Revisão
        [HttpPost]
        public async Task<IActionResult> Revisar(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                TempData["Erro"] = "Selecione um arquivo XML válido.";
                return RedirectToAction(nameof(Importar));
            }

            // Lê dados básicos do XML via seu serviço
            var notaXml = _xmlService.LerXml(xmlFile);

            // Verifica se a nota já foi importada
            if (await _recebimentoRepo.ExisteChaveAcessoAsync(notaXml.ChaveAcesso))
            {
                TempData["Erro"] = "Esta nota fiscal já foi importada anteriormente.";
                return RedirectToAction(nameof(Importar));
            }

            // Busca o fornecedor pelo CNPJ do XML
            var fornecedor = await _pessoaRepo.GetByCnpjAsync(notaXml.CnpjFornecedor);

            var viewModel = new RevisaoNotaViewModel
            {
                NumeroNota = notaXml.NumeroNota,
                ChaveAcesso = notaXml.ChaveAcesso,
                PessoaId = fornecedor?.IdPessoa ?? 0,
                FornecedorNome = fornecedor?.Nome ?? "FORNECEDOR NÃO ENCONTRADO",
                DataEntrada = DateTime.Now
            };

            // Processa os itens com o De-Para
            foreach (var itemXml in notaXml.Itens)
            {
                // USA O SEU VINCULO QUE JÁ EXISTE NO BANCO
                var vinculo = await _vinculoRepo.GetVinculoAsync(viewModel.PessoaId, itemXml.CodigoFornecedor);

                viewModel.Itens.Add(new ItemRevisaoViewModel
                {
                    CodigoFornecedor = itemXml.CodigoFornecedor,
                    DescricaoXml = itemXml.Descricao,
                    Quantidade = itemXml.Quantidade,
                    ValorUnitario = itemXml.ValorUnitario,

                    // Se não houver vínculo, estes IDs ficarão nulos e a View avisará o usuário
                    ProdutoIdInterno = vinculo?.ProdutoId,
                    ProdutoEmbalagemId = vinculo?.ProdutoEmbalagemId,
                    NomeProdutoInterno = vinculo?.Produto?.NomeProduto
                });
            }

            // Preenche os Dropdowns da tela de revisão
            ViewBag.Prazos = new SelectList(await _prazoRepo.GetAllAsync(), "IdPrazoPagamento", "Descricao");
            ViewBag.Formas = new SelectList(await _formaRepo.GetAllAsync(), "IdFormaPagamento", "Descricao");

            return View(viewModel);
        }

        // 3. Ação Final: Recebe os dados conferidos e salva de verdade
        [HttpPost]
        public async Task<IActionResult> Confirmar(RevisaoNotaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Verifique os dados preenchidos.";
                return View("Revisar", model);
            }

            // Verifica se há produtos não identificados
            if (model.Itens.Any(i => !i.ProdutoIdInterno.HasValue))
            {
                TempData["Erro"] = "Existem itens sem vínculo. Vincule-os antes de confirmar.";
                return View("Revisar", model);
            }

            // Mapeia ViewModel para a Model final
            var recebimento = new Recebimento
            {
                PessoaId = model.PessoaId,
                NumeroNota = model.NumeroNota,
                ChaveAcesso = model.ChaveAcesso,
                DataEntrada = model.DataEntrada,
                PrazoPagamentoId = model.PrazoPagamentoId,
                FormaPagamentoId = model.FormaPagamentoId,
                Itens = model.Itens.Select(i => new RecebimentoItem
                {
                    ProdutoId = i.ProdutoIdInterno.Value,
                    ProdutoEmbalagemId = i.ProdutoEmbalagemId.Value,
                    CodigoProdutoFornecedor = i.CodigoFornecedor,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario
                }).ToList()
            };

            try
            {
                var sucesso = await _recebimentoRepo.RegistrarRecebimentoAsync(recebimento);
                if (sucesso)
                {
                    TempData["Sucesso"] = "Recebimento registrado com sucesso!";
                    return RedirectToAction(nameof(Index), "Estoque");
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Falha ao registrar: " + ex.Message;
            }

            return View("Revisar", model);
        }
    }
}