using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Controllers
{
    [Authorize(Roles = "Administrador, Gerente")]
    public class InventarioController : Controller
    {
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IProdutoRepository _produtoRepository; // Adicionado para buscar dados do produto
        private readonly IEstoqueRepository _estoqueRepo; // Adicionado para buscar dados do produto
        private readonly VarejoDbContext _context;

        public InventarioController(
            IInventarioRepository inventarioRepository,
            IProdutoRepository produtoRepository,
            IEstoqueRepository estoqueRepo,
            VarejoDbContext context)
        {
            _inventarioRepository = inventarioRepository;
            _produtoRepository = produtoRepository;
            _estoqueRepo = estoqueRepo;
            _context = context;
        }

        // LISTAGEM DE INVENTÁRIOS
        public async Task<IActionResult> Index()
        {
            var inventariosBanco = await _inventarioRepository.GetAllAsync();

            var viewModel = inventariosBanco.Select(i => new InventarioViewModel
            {
                Id = i.Id,
                DataCriacao = i.Data,
                Observacao = i.Observacao,
                Finalizado = i.Finalizado,
                // Na Index, geralmente só precisamos saber se existem itens ou a quantidade
                Itens = i.Itens?.Select(it => new InventarioItemViewModel
                {
                    ProdutoId = it.ProdutoId
                }).ToList() ?? new List<InventarioItemViewModel>()
            }).ToList();

            return View(viewModel);
        }

        // TELA DE LANÇAMENTO
        public async Task<IActionResult> Details(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                    .ThenInclude(it => it.Produto)
                .Include(i => i.Itens)
                    .ThenInclude(it => it.ProdutoEmbalagem)
                        .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null) return NotFound();

            var viewModel = new InventarioViewModel
            {
                Id = inventario.Id,
                DataCriacao = inventario.Data,
                Observacao = inventario.Observacao,
                // ESTA LINHA É A CHAVE:
                Finalizado = inventario.Finalizado,

                Itens = inventario.Itens.Select(it => new InventarioItemViewModel
                {
                    Id = it.Id,
                    ProdutoId = it.ProdutoId,
                    ProdutoEmbalagemId = it.ProdutoEmbalagemId,
                    NomeProduto = it.Produto?.NomeProduto,
                    DescricaoEmbalagem = it.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem,
                    Multiplicador = it.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1,
                    QuantidadeSistema = it.QuantidadeSistema,
                    QuantidadeContada = it.QuantidadeContada
                }).ToList()
            };

            return View(viewModel);
        }



        //ATUALIZAR QUANTIDADE NO INVENTARIO

        [HttpPost]
        [IgnoreAntiforgeryToken] 
        public async Task<IActionResult> UpdateItemQuantity([FromForm] int itemId, [FromForm] string quantidade)
        {
            // BUSCA DIRETA PELA CHAVE PRIMÁRIA
            var item = await _context.InventariosItem
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(it => it.Id == itemId);

            if (item == null) return NotFound("Item não encontrado no inventário.");

            // Tratamento do decimal (mantendo sua lógica de Replace)
            decimal.TryParse(quantidade.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal qtdDigitada);

            // O multiplicador vem da Embalagem/TipoEmbalagem desta linha específica
            int mult = item.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1;

            // Atualiza a quantidade contada (ex: 1 caixa * 12 = 12 unidades)
            item.QuantidadeContada = qtdDigitada * mult;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // GET: Inventario/Create
        public IActionResult Create()
        {
            // Retorna a View com uma ViewModel vazia para evitar erros de referência nula
            var viewModel = new InventarioViewModel { DataCriacao = DateTime.Now };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string observacao)
        {
            var inventario = new Inventario
            {
                Data = DateTime.Now,
                Observacao = observacao,
                Finalizado = false
            };

            await _inventarioRepository.CriarInventarioAsync(inventario);
            return RedirectToAction(nameof(Details), new { id = inventario.Id });
        }


        //EDITAR INVENTARIO

        // GET: Inventario/Edit/5
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var inventario = await _context.Inventarios.FindAsync(id);
            if (inventario == null) return NotFound();

            var viewModel = new InventarioViewModel
            {
                Id = inventario.Id,
                DataCriacao = inventario.Data,
                Observacao = inventario.Observacao
            };

            return View(viewModel);
        }

        // POST: Inventario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventarioViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            // Buscamos o objeto original do banco
            var inventarioBanco = await _context.Inventarios.FindAsync(id);
            if (inventarioBanco == null) return NotFound();

            // Em vez de if(ModelState.IsValid), verificamos apenas os campos que importam
            if (!string.IsNullOrEmpty(viewModel.Observacao))
            {
                try
                {
                    inventarioBanco.Data = viewModel.DataCriacao;
                    inventarioBanco.Observacao = viewModel.Observacao;

                    _context.Update(inventarioBanco);
                    await _context.SaveChangesAsync();

                    TempData["Sucesso"] = "Inventário atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao salvar: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("Observacao", "A observação é obrigatória.");
            }

            return View(viewModel);
        }





        //ADICIONAR ITEM AO INVENTÁRIO

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddItem([FromForm] AddItemRequest request)
        {
            // Validação de segurança para o objeto request
            if (request == null) return BadRequest("Dados da requisição inválidos.");

            // 1. Busca o produto garantindo que as embalagens existam
            var produto = await _context.Produtos
                .Include(p => p.ProdutosEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(p => p.IdProduto == request.ProdutoId);

            if (produto == null) return NotFound("Produto não encontrado no banco.");

            // 2. Busca a embalagem específica
            var embalagemSelecionada = produto.ProdutosEmbalagem?
                .FirstOrDefault(e => e.IdProdutoEmbalagem == request.ProdutoEmbalagemId);

            if (embalagemSelecionada == null || embalagemSelecionada.TipoEmbalagem == null)
                return BadRequest("Embalagem ou Tipo de Embalagem inválidos.");

            // 3. Conversão da quantidade
            decimal.TryParse(request.QuantidadeContada?.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal qtdDigitada);

            // 4. Cálculo com multiplicador
            decimal quantidadeFinal = qtdDigitada * (embalagemSelecionada.TipoEmbalagem.Multiplicador);

            // 5. Criação do objeto (Preenchendo apenas os IDs para o EF)
            var item = new InventarioItem
            {
                InventarioId = request.InventarioId,
                ProdutoId = request.ProdutoId,
                ProdutoEmbalagemId = request.ProdutoEmbalagemId,
                QuantidadeSistema = produto.EstoqueAtual,
                QuantidadeContada = quantidadeFinal,
                ProdutoEmbalagem = embalagemSelecionada
            };

            try
            {
                // Se o erro morre aqui, o problema está DENTRO do método AddItemAsync do Repository
                await _inventarioRepository.AddItemAsync(item);
                return Ok();
            }
            catch (Exception ex)
            {
                // Isso vai te mostrar exatamente onde no Repository a coisa fedeu
                return StatusCode(500, $"Erro ao persistir no repositório: {ex.Message}");
            }
        }


        //REMOVER ITEM DO INVENTARIO

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var item = await _context.InventariosItem.FindAsync(itemId);
            if (item == null) return NotFound();

            var inventarioId = item.InventarioId;
            await _inventarioRepository.RemoverItemAsync(itemId);

            return RedirectToAction(nameof(Details), new { id = inventarioId });
        }

        //FINALIZAR INVENTARIO

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            // 1. Busca o inventário e os itens
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null || inventario.Finalizado)
                return BadRequest("Inventário não encontrado ou já finalizado.");

            // 2. Criar o cabeçalho do Movimento
            var novoMovimento = new Movimento
            {
                Documento = inventario.Id,
                Observacao = $"Ajuste via Inventário #{inventario.Id} - {inventario.Observacao ?? "Sem obs"}",
                DataMovimento = DateTime.Now,
                TipoMovimentoId = 7, // Inventário
                PessoaId = 1,
                ProdutosMovimento = new List<ProdutoMovimento>()
            };

            _context.Movimentos.Add(novoMovimento);

            // SALVAMOS AQUI para gerar o ID do movimento que o Repository precisa
            await _context.SaveChangesAsync();

            // 3. Registramos as linhas de contagem no detalhe do movimento
            foreach (var item in inventario.Itens)
            {
                var prodMov = new ProdutoMovimento
                {
                    ProdutoId = item.ProdutoId,
                    ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                    Quantidade = item.QuantidadeContada,
                    MovimentoId = novoMovimento.IdMovimento // Usando o ID já gerado
                };
                _context.ProdutosMovimento.Add(prodMov);
            }

            // 4. Agrupamos por Produto para somar o estoque TOTAL
            var totaisParaEstoque = inventario.Itens
                .GroupBy(it => it.ProdutoId)
                .Select(g => new {
                    IdProduto = g.Key,
                    SomaTotalUnidades = g.Sum(x => x.QuantidadeContada)
                });

            // 5. ATUALIZAÇÃO VIA REPOSITORY (Centralizado)
            foreach (var consolidado in totaisParaEstoque)
            {
                // AJUSTE: Passando os 5 parâmetros conforme sua nova assinatura no Repository
                await _estoqueRepo.AjustarEstoqueInventarioAsync(
                    consolidado.IdProduto,
                    novoMovimento.IdMovimento, // O ID do movimento (FK)
                    inventario.Id,             // O ID do inventário (para a observação)
                    consolidado.SomaTotalUnidades,
                    novoMovimento.Observacao);
            }

            // 6. Fecha o inventário
            inventario.Finalizado = true;
            _context.Update(inventario);

            // 7. Salva tudo (Itens do movimento, Históricos e Status do Inventário)
            await _context.SaveChangesAsync();

            TempData["Success"] = "Estoque atualizado com sucesso e histórico gerado!";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        //DELETAR INVENTARIO


        // GET: Inventario/Delete/5
        [Authorize(Roles = "Administrador, Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null) return NotFound();

            var viewModel = new InventarioViewModel
            {
                Id = inventario.Id,
                DataCriacao = inventario.Data,
                Observacao = inventario.Observacao,
                Itens = inventario.Itens.Select(it => new InventarioItemViewModel()).ToList() // Para contar os itens
            };

            return View(viewModel);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null) return NotFound();

            try
            {
                // Regra: Não deleta se tiver itens
                if (inventario.Itens != null && inventario.Itens.Any())
                    throw new InvalidOperationException("O inventário possui itens lançados e não pode ser excluído.");

                if (inventario.Finalizado)
                    throw new InvalidOperationException("Não é possível excluir um inventário finalizado.");

                await _inventarioRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["DeleteError"] = "❌ " + ex.Message;

                var viewModel = new InventarioViewModel
                {
                    Id = inventario.Id,
                    DataCriacao = inventario.Data,
                    Observacao = inventario.Observacao
                };
                return View("Delete", viewModel);
            }
        }


        //IMPRIMIR INVENTARIO

        public async Task<IActionResult> Imprimir(int id)
        {
            // 1. Busca os dados completos no banco (incluindo as embalagens para mostrar a descrição correta)
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                    .ThenInclude(it => it.Produto)
                .Include(i => i.Itens)
                    .ThenInclude(it => it.ProdutoEmbalagem)
                        .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inventario == null) return NotFound();

            // 2. Converte para a ViewModel
            var viewModel = new InventarioViewModel
            {
                Id = inventario.Id,
                DataCriacao = inventario.Data,
                Observacao = inventario.Observacao,
                Finalizado = inventario.Finalizado,
                Itens = inventario.Itens.Select(it => new InventarioItemViewModel
                {
                    Id = it.Id,
                    ProdutoId = it.ProdutoId,
                    NomeProduto = it.Produto?.NomeProduto,
                    DescricaoEmbalagem = it.ProdutoEmbalagem?.TipoEmbalagem?.DescricaoTipoEmbalagem,
                    // Opcional: se quiser mostrar o multiplicador no relatório
                    Multiplicador = it.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1,
                    QuantidadeSistema = it.QuantidadeSistema,
                    QuantidadeContada = it.QuantidadeContada
                }).ToList()
            };

            // 3. Retorna a View de relatório passando a ViewModel
            return View("RelatorioConferencia", viewModel);
        }
    }
}
