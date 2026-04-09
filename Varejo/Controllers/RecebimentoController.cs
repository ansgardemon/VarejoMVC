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

        public RecebimentoController(
            IRecebimentoRepository recebimentoRepo,
            IPessoaRepository pessoaRepo,
            IFornecedorConfigRepository vinculoRepo,
            IPrazoPagamentoRepository prazoRepo,
            IFormaPagamentoRepository formaRepo,
            IXmlService xmlService,
            IProdutoEmbalagemRepository embalagemRepo)
        {
            _recebimentoRepo = recebimentoRepo;
            _pessoaRepo = pessoaRepo;
            _vinculoRepo = vinculoRepo;
            _prazoRepo = prazoRepo;
            _formaRepo = formaRepo;
            _xmlService = xmlService;
            _embalagemRepo = embalagemRepo;
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

            var viewModel = _xmlService.CarregarDadosDoXml(xmlFile);

            if (await _recebimentoRepo.ExisteChaveAcessoAsync(viewModel.ChaveAcesso))
            {
                TempData["Erro"] = "Nota já importada.";
                return RedirectToAction(nameof(Importar));
            }

            // Tratativa para o CNPJ: Se o banco tem pontuação e o XML não,
            // garantimos que a busca funcione tentando ambos ou limpando.
            var cnpjDoXml = viewModel.CnpjCpfFornecedorXml;
            var fornecedor = await _pessoaRepo.GetByCnpjAsync(cnpjDoXml);

            // Se não achou pelo CNPJ limpo, tenta formatar para o padrão do seu banco
            if (fornecedor == null && cnpjDoXml.Length == 14)
            {
                string cnpjFormatado = Convert.ToUInt64(cnpjDoXml).ToString(@"00\.000\.000\/0000\-00");
                fornecedor = await _pessoaRepo.GetByCnpjAsync(cnpjFormatado);
            }

            if (fornecedor != null)
            {
                viewModel.PessoaId = fornecedor.IdPessoa;
                viewModel.NomeFornecedor = fornecedor.NomeRazao;

                foreach (var item in viewModel.Itens)
                {
                    // Busca o De/Para (754 do XML vs ID do fornecedor no seu sistema)
                    var vinculo = await _vinculoRepo.GetVinculoPorCodigoExternoAsync(viewModel.PessoaId, item.CodigoFornecedor.Trim());

                    if (vinculo != null)
                    {
                        item.ProdutoIdInterno = vinculo.ProdutoId;
                        var embalagens = await _embalagemRepo.GetByProdutoIdAsync(vinculo.ProdutoId);

                        item.EmbalagensDisponiveis = embalagens.Select(e => new SelectListItem
                        {
                            Value = e.IdProdutoEmbalagem.ToString(),
                            Text = e.TipoEmbalagem != null ? e.TipoEmbalagem.DescricaoTipoEmbalagem : "Embalagem " + e.IdProdutoEmbalagem
                        }).ToList();
                    }
                    else
                    {
                        item.ProdutoIdInterno = null;
                        item.EmbalagensDisponiveis = new List<SelectListItem>();
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
                var listaItensFinal = new List<RecebimentoItem>();

                foreach (var item in model.Itens)
                {
                    // Busca a embalagem para saber o multiplicador (Ex: Caixa com 12)
                    var embalagemObj = await _embalagemRepo.GetByIdAsync(item.ProdutoEmbalagemId.Value);
                    decimal multiplicador = embalagemObj?.TipoEmbalagem?.Multiplicador ?? 1;

                    // CONVERSÃO: 
                    // Se o XML diz 25 (caixas) a 105.00 e a embalagem interna é x12:
                    // Quantidade = 25 * 12 = 300 unidades
                    // Valor Unitário = 105.00 / 12 = 8.75 cada
                    decimal qtdConvertida = item.Quantidade * multiplicador;
                    decimal vlrConvertido = item.ValorUnitario / multiplicador;

                    listaItensFinal.Add(new RecebimentoItem
                    {
                        ProdutoId = item.ProdutoIdInterno.Value,
                        ProdutoEmbalagemId = item.ProdutoEmbalagemId.Value,
                        Quantidade = qtdConvertida,
                        ValorUnitario = vlrConvertido,
                        CodigoProdutoFornecedor = item.CodigoFornecedor
                    });
                }

                var recebimento = new Recebimento
                {
                    PessoaId = model.PessoaId,
                    NumeroNota = model.NumeroNota,
                    ChaveAcesso = model.ChaveAcesso,
                    DataEntrada = model.DataEntrada,
                    PrazoPagamentoId = model.PrazoPagamentoId,
                    FormaPagamentoId = model.FormaPagamentoId,
                    Itens = listaItensFinal
                };

                var sucesso = await _recebimentoRepo.RegistrarRecebimentoAsync(recebimento);

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
    }
}