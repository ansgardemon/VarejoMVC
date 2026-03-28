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
        private readonly VarejoDbContext _context;

        public InventarioController(
            IInventarioRepository inventarioRepository,
            IProdutoRepository produtoRepository,
            VarejoDbContext context)
        {
            _inventarioRepository = inventarioRepository;
            _produtoRepository = produtoRepository;
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

        // TELA DE LANÇAMENTO (Onde a mágica acontece)
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

        [HttpPost]
        [IgnoreAntiforgeryToken] // Mantido conforme seu código
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

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var item = await _context.InventariosItem.FindAsync(itemId);
            if (item == null) return NotFound();

            var inventarioId = item.InventarioId;
            await _inventarioRepository.RemoverItemAsync(itemId);

            return RedirectToAction(nameof(Details), new { id = inventarioId });
        }

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

            // 2. Criar o cabeçalho do Movimento (Corrigido para não dar erro de NULL)
            var novoMovimento = new Movimento
            {
                Documento = inventario.Id,
                // Garantimos que Observacao nunca seja null
                Observacao = $"Ajuste via Inventário #{inventario.Id} - {inventario.Observacao ?? "Sem obs"}",
                DataMovimento = DateTime.Now,
                TipoMovimentoId = 7,
                PessoaId = 1,
                ProdutosMovimento = new List<ProdutoMovimento>()
            };

            _context.Movimentos.Add(novoMovimento);

            // --- LÓGICA DE PROCESSAMENTO ---

            // 3. Primeiro, registramos CADA linha de contagem no histórico de movimento
            foreach (var item in inventario.Itens)
            {
                var prodMov = new ProdutoMovimento
                {
                    ProdutoId = item.ProdutoId,
                    ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                    Quantidade = item.QuantidadeContada,
                    Movimento = novoMovimento
                };
                _context.ProdutosMovimento.Add(prodMov);
            }

            // 4. Agora a MÁGICA: Agrupamos por Produto para somar o estoque TOTAL (12 + 1 = 13)
            var totaisParaEstoque = inventario.Itens
                .GroupBy(it => it.ProdutoId)
                .Select(g => new {
                    IdProduto = g.Key,
                    SomaTotalUnidades = g.Sum(x => x.QuantidadeContada)
                });

            foreach (var consolidado in totaisParaEstoque)
            {
                var produtoNoBanco = await _context.Produtos.FindAsync(consolidado.IdProduto);
                if (produtoNoBanco != null)
                {
                    // Aqui o estoque vira 13, independente de quantas embalagens foram usadas
                    produtoNoBanco.EstoqueAtual = consolidado.SomaTotalUnidades;
                    _context.Update(produtoNoBanco);
                }
            }

            // 5. Fecha o inventário
            inventario.Finalizado = true;
            _context.Update(inventario);

            // 6. Salva tudo (Transação atômica)
            await _context.SaveChangesAsync();

            TempData["Success"] = "Estoque atualizado com sucesso (soma de todas as embalagens)!";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _inventarioRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

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
