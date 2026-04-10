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
        private readonly IXmlService _xmlService;
        private readonly IProdutoEmbalagemRepository _embalagemRepo;
        private readonly IConfiguracaoEmpresaRepository _empresaRepo;


        public RecebimentoController(
            IRecebimentoRepository recebimentoRepo,
            IPessoaRepository pessoaRepo,
            IFornecedorConfigRepository vinculoRepo,
            IPrazoPagamentoRepository prazoRepo,
            IFormaPagamentoRepository formaRepo,
            IXmlService xmlService,
            IProdutoEmbalagemRepository embalagemRepo,
            IConfiguracaoEmpresaRepository empresaRepo)
        {
            _recebimentoRepo = recebimentoRepo;
            _pessoaRepo = pessoaRepo;
            _vinculoRepo = vinculoRepo;
            _prazoRepo = prazoRepo;
            _formaRepo = formaRepo;
            _xmlService = xmlService;
            _embalagemRepo = embalagemRepo;
            _empresaRepo = empresaRepo;
        }

        public IActionResult Importar() => View();

        public async Task<IActionResult> Index()
        {
            var recebimentos = await _recebimentoRepo.GetAllAsync();
            return View(recebimentos);
        }

        [HttpPost]
        public async Task<IActionResult> Revisar(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0) return RedirectToAction(nameof(Importar));

            // 1. Carrega os dados do XML
            var viewModel = _xmlService.CarregarDadosDoXml(xmlFile);

            // 2. Validação do Destinatário (Sua Empresa)
            var empresa = await _empresaRepo.GetConfiguracaoAsync(); // Assumindo que este método busque a config única
            if (empresa != null)
            {
                // Remove pontuação para comparar apenas os números
                string cnpjMinhaEmpresa = new string(empresa.Cnpj.Where(char.IsDigit).ToArray());
                string cnpjNoXml = new string(viewModel.CnpjDestinatarioXml.Where(char.IsDigit).ToArray());

                if (cnpjNoXml != cnpjMinhaEmpresa)
                {
                    TempData["Erro"] = $"Bloqueio de Segurança: Esta nota foi emitida para o CNPJ {viewModel.CnpjDestinatarioXml}. " +
                                       $"O sistema está configurado para: {empresa.RazaoSocial}.";
                    return RedirectToAction(nameof(Importar));
                }
            }

            // 3. Verificação de Nota Já Importada
            if (await _recebimentoRepo.ExisteChaveAcessoAsync(viewModel.ChaveAcesso))
            {
                TempData["Erro"] = "Nota já importada.";
                return RedirectToAction(nameof(Importar));
            }

            // 4. Identificação do Fornecedor (Emissor da Nota)
            var cnpjFornecedorXml = viewModel.CnpjCpfFornecedorXml;
            var fornecedor = await _pessoaRepo.GetByCnpjAsync(cnpjFornecedorXml);

            // Tenta formatar se não achar o CNPJ limpo
            if (fornecedor == null && cnpjFornecedorXml.Length == 14)
            {
                string cnpjFormatado = Convert.ToUInt64(cnpjFornecedorXml).ToString(@"00\.000\.000\/0000\-00");
                fornecedor = await _pessoaRepo.GetByCnpjAsync(cnpjFormatado);
            }

            if (fornecedor != null)
            {
                viewModel.PessoaId = fornecedor.IdPessoa;
                viewModel.NomeFornecedor = fornecedor.NomeRazao;

                // 5. Mapeamento de Itens (De-Para)
                foreach (var item in viewModel.Itens)
                {
                    var vinculo = await _vinculoRepo.GetVinculoPorCodigoExternoAsync(viewModel.PessoaId, item.CodigoFornecedor.Trim());

                    if (vinculo != null)
                    {
                        item.ProdutoIdInterno = vinculo.ProdutoId;
                        var embalagens = await _embalagemRepo.GetByProdutoIdAsync(vinculo.ProdutoId);

                        item.EmbalagensDisponiveis = embalagens.Select(e => new SelectListItem
                        {
                            Value = e.IdProdutoEmbalagem.ToString(),
                            Text = e.TipoEmbalagem?.DescricaoTipoEmbalagem ?? $"Embalagem {e.IdProdutoEmbalagem}"
                        }).ToList();
                    }
                }
            }

            await CarregarCombosFinanceiros();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar(RevisaoNotaViewModel model)
        {
            if (model.Itens.Any(i => i.ProdutoIdInterno == null || i.ProdutoEmbalagemId == null))
            {
                TempData["Erro"] = "Existem produtos não identificados ou sem embalagem.";
                await RecarregarDados(model);
                return View("Revisar", model);
            }

            try
            {
                var recebimento = new Recebimento
                {
                    PessoaId = model.PessoaId,
                    NumeroNota = model.NumeroNota,
                    ChaveAcesso = model.ChaveAcesso,
                    DataEntrada = model.DataEntrada,
                    PrazoPagamentoId = model.PrazoPagamentoId,
                    FormaPagamentoId = model.FormaPagamentoId,

                    // MANDE OS DADOS BRUTOS DA TELA
                    // Deixe o RegistrarRecebimentoAsync cuidar da matemática
                    Itens = model.Itens.Select(i => new RecebimentoItem
                    {
                        ProdutoId = i.ProdutoIdInterno.Value,
                        ProdutoEmbalagemId = i.ProdutoEmbalagemId.Value,
                        Quantidade = i.Quantidade, // Ex: 25
                        ValorUnitario = i.ValorUnitario, // Ex: 105.00
                        CodigoProdutoFornecedor = i.CodigoFornecedor,
                        EhBonificacao = i.EhBonificacao
                    }).ToList()
                };

                var sucesso = await _recebimentoRepo.RegistrarRecebimentoAsync(recebimento);
                // ... restante do código

                if (sucesso)
                {
                    TempData["Sucesso"] = "Recebimento registrado com sucesso!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao processar: " + ex.Message;
            }

            await RecarregarDados(model);
            return View("Revisar", model);
        }

        private async Task CarregarCombosFinanceiros()
        {
            ViewBag.Prazos = new SelectList(await _prazoRepo.GetAllAsync(), "IdPrazoPagamento", "Descricao");
            ViewBag.Formas = new SelectList(await _formaRepo.GetAllAsync(), "IdFormaPagamento", "DescricaoFormaPagamento");
        }

        private async Task RecarregarDados(RevisaoNotaViewModel model)
        {
            await CarregarCombosFinanceiros();
            foreach (var item in model.Itens.Where(i => i.ProdutoIdInterno.HasValue))
            {
                var embalagens = await _embalagemRepo.GetByProdutoIdAsync(item.ProdutoIdInterno.Value);
                item.EmbalagensDisponiveis = embalagens.Select(e => new SelectListItem
                {
                    Value = e.IdProdutoEmbalagem.ToString(),
                    Text = e.TipoEmbalagem?.DescricaoTipoEmbalagem ?? "Embalagem",
                    Selected = e.IdProdutoEmbalagem == item.ProdutoEmbalagemId
                }).ToList();
            }
        }


        // GET: Recebimento/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var recebimento = await _recebimentoRepo.GetByIdAsync(id);

            if (recebimento == null)
            {
                return NotFound();
            }

            return View(recebimento);
        }

        // POST: Recebimento/Desintegrar/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Boa prática para métodos de exclusão
        public async Task<IActionResult> Desintegrar(int id)
        {
            try
            {
                var sucesso = await _recebimentoRepo.DesintegrarRecebimentoAsync(id);

                if (sucesso)
                {
                    TempData["Sucesso"] = "Recebimento desintegrado, estoque estornado e financeiro removido!";
                }
                else
                {
                    TempData["Erro"] = "Não foi possível localizar o recebimento para desintegrar.";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao desintegrar nota: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}