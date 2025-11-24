using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    public class ValidadeController : Controller
    {
        private readonly IValidadeRepository _validadeRepository;
        private readonly IProdutoRepository _produtoRepository;

        public ValidadeController(IValidadeRepository validadeRepository, IProdutoRepository produtoRepository)
        {
            _validadeRepository = validadeRepository;
            _produtoRepository = produtoRepository;
        }

        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Index(
    DateTime? dataInicial,
    DateTime? dataFinal,
    string? produtoNome,
    string aba = "estoque") // estoque | esgotados
        {
            // ABA define EmEstoque = true ou false
            bool? emEstoque = aba == "estoque" ? true : false;

            // Se não foi passado nada → padrão de 6 meses
            if (!dataInicial.HasValue || !dataFinal.HasValue)
            {
                var hoje = DateTime.Today;
                dataInicial ??= hoje.AddMonths(-3);
                dataFinal ??= hoje.AddMonths(+3);
            }

            // Salvar no ViewBag os filtros
            ViewBag.FiltroDataInicial = dataInicial;
            ViewBag.FiltroDataFinal = dataFinal;
            ViewBag.FiltroProdutoNome = produtoNome;
            ViewBag.AbaAtual = aba;

            // Busca filtrada
            var validades = await _validadeRepository
                .FiltrarAsync(dataInicial, dataFinal, produtoNome, emEstoque);

            // ViewModels
            var viewModels = validades.Select(c => new ValidadeViewModel
            {
                IdValidade = c.IdValidade,
                DataValidade = c.DataValidade,
                EmEstoque = c.EmEstoque,
                ProdutoId = c.ProdutoId,
                ProdutoNome = c.Produto.NomeProduto
            });

            return View(viewModels);
        }



        [HttpGet]
        public async Task<IActionResult> SearchProduto(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<object>());

            var produtos = await _produtoRepository.GetByNameAsync(query);

            return Json(produtos.Select(p => new
            {
                p.IdProduto,
                p.NomeProduto,
                p.UrlImagem,
                p.Embalagens
            }));
        }




        // GET: Validade/Create
        [Authorize(Roles = "Administrador, Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Validade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ValidadeViewModel c)
        {
            Console.WriteLine("===== POST /Validade/Create =====");
            Console.WriteLine($"ProdutoId Recebido: {c.ProdutoId}");
            Console.WriteLine($"DataValidade Recebida: {c.DataValidade}");
            Console.WriteLine($"EmEstoque: {c.EmEstoque}");

            // Validação manual de ProdutoId
            if (c.ProdutoId == 0)
            {
                Console.WriteLine("ERRO: ProdutoId == 0 (nenhum produto selecionado)");
                ModelState.AddModelError("ProdutoId", "Selecione um produto.");
            }

            // Se ModelState for inválido, mostra todos os erros
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido. Erros:");

                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($" - Campo: {error.Key}");
                        foreach (var e in error.Value.Errors)
                            Console.WriteLine($"   > {e.ErrorMessage}");
                    }
                }

                return View(c);
            }

            // Se chegou aqui, está tudo válido
            Console.WriteLine("ModelState Válido. Tentando salvar...");

            var validade = new Validade
            {
                DataValidade = c.DataValidade,
                EmEstoque = c.EmEstoque,
                ProdutoId = c.ProdutoId
            };

            try
            {
                await _validadeRepository.AddAsync(validade);
                Console.WriteLine("SALVO COM SUCESSO.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEÇÃO AO SALVAR:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                ModelState.AddModelError("", "Erro ao salvar. Verifique os dados.");
            }

            return View(c);
        }



        // GET: Marca/Edit/5
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var c = await _validadeRepository.GetByIdAsync(id);
            if (c == null)
                return NotFound();

            var viewModel = new ValidadeViewModel
            {
                IdValidade = c.IdValidade,
                DataValidade = c.DataValidade,
                EmEstoque = c.EmEstoque,
                ProdutoId = c.ProdutoId,
                ProdutoNome = c.Produto.NomeProduto
            };

            return View(viewModel);
        }

        // POST: Marca/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ValidadeViewModel viewModel)
        {
            if (id != viewModel.IdValidade)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var validade = new Validade
                {
                    IdValidade = viewModel.IdValidade,
                    DataValidade = viewModel.DataValidade,
                    EmEstoque = viewModel.EmEstoque,
                    ProdutoId = viewModel.ProdutoId,
                };


                try
                {
                    await _validadeRepository.UpdateAsync(validade);
                    return RedirectToAction(nameof(Index));
                }

                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Ao criar Validade: " + ex.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível atualizar a Validar. Verifique se a data foi digitada corretamente.");
                }

            }
            return View(viewModel);
        }

 

    }
}

