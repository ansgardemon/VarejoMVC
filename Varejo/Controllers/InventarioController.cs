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
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateItemQuantity([FromForm] int inventarioId, [FromForm] int produtoId, [FromForm] int produtoEmbalagemId, [FromForm] string quantidade)
        {
            // BUSCA PELA CHAVE COMPOSTA: Inventário + Produto + Embalagem específica
            var item = await _context.InventariosItem
                .Include(i => i.ProdutoEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(it => it.InventarioId == inventarioId
                                        && it.ProdutoId == produtoId
                                        && it.ProdutoEmbalagemId == produtoEmbalagemId);

            if (item == null) return NotFound("Item não encontrado.");

            decimal.TryParse(quantidade.Replace(",", "."), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal qtdDigitada);

            int mult = item.ProdutoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1;
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
            Console.WriteLine($"======= DEBUG ADD ITEM =======");
            Console.WriteLine($"Inv: {request.InventarioId} | Prod: {request.ProdutoId} | Emb: {request.ProdutoEmbalagemId}");

            // 1. Busca o produto com as embalagens incluídas
            var produto = await _context.Produtos
                .Include(p => p.ProdutosEmbalagem)
                    .ThenInclude(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(p => p.IdProduto == request.ProdutoId);

            if (produto == null) return NotFound("Produto não encontrado no banco.");

            // 2. Busca a embalagem específica selecionada para pegar o multiplicador
            var embalagemSelecionada = produto.ProdutosEmbalagem
                .FirstOrDefault(e => e.IdProdutoEmbalagem == request.ProdutoEmbalagemId);

            if (embalagemSelecionada == null || embalagemSelecionada.TipoEmbalagem == null)
                return BadRequest("Embalagem inválida.");

            // 3. Converte a quantidade contada
            decimal.TryParse(request.QuantidadeContada.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal qtdDigitada);

            // 4. APLICA O MULTIPLICADOR (Ex: 2 fardos * 12 unidades = 24)
            decimal quantidadeFinal = qtdDigitada * embalagemSelecionada.TipoEmbalagem.Multiplicador;

            var item = new InventarioItem
            {
                InventarioId = request.InventarioId,
                ProdutoId = request.ProdutoId,
                ProdutoEmbalagemId = request.ProdutoEmbalagemId,
                QuantidadeSistema = produto.EstoqueAtual,
                QuantidadeContada = quantidadeFinal // Agora com multiplicador aplicado
            };

            await _inventarioRepository.AddItemAsync(item);
            return Ok();
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
            // 1. Busca o inventário e os itens (com os dados dos produtos)
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                    .ThenInclude(it => it.Produto)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null || inventario.Finalizado)
                return BadRequest("Inventário não encontrado ou já finalizado.");

            // 2. Criar o cabeçalho do Movimento
            // IMPORTANTE: Ajuste os IDs abaixo (PessoaId e TipoMovimentoId) 
            // conforme os registros que você tem no banco (ex: Pessoa "Sistema", Tipo "Ajuste")
            var novoMovimento = new Movimento
            {
                Documento = inventario.Id, // Usando o ID do inventário como número de documento
                Observacao = $"Ajuste via Inventário #{inventario.Id} - {inventario.Observacao}",
                DataMovimento = DateTime.Now,
                TipoMovimentoId = 7, // EX: ID 3 pode ser "Ajuste de Inventário" no seu banco
                PessoaId = 1,        // EX: ID 1 pode ser seu "Consumidor Padrão" ou "Empresa"
                ProdutosMovimento = new List<ProdutoMovimento>()
            };

            _context.Movimentos.Add(novoMovimento);

            // 3. Agrupar itens para atualizar o estoque e gerar os itens do movimento
            // Agrupamos por Produto e Embalagem para respeitar sua estrutura de ProdutoMovimento
            var itensParaProcessar = inventario.Itens
                .GroupBy(it => new { it.ProdutoId, it.ProdutoEmbalagemId })
                .Select(g => new {
                    IdProduto = g.Key.ProdutoId,
                    IdEmbalagem = g.Key.ProdutoEmbalagemId,
                    TotalContado = g.Sum(x => x.QuantidadeContada)
                });

            foreach (var item in itensParaProcessar)
            {
                // 4. Cria o item do movimento (ProdutoMovimento)
                var prodMov = new ProdutoMovimento
                {
                    ProdutoId = item.IdProduto,
                    ProdutoEmbalagemId = item.IdEmbalagem,
                    Quantidade = item.TotalContado,
                    Movimento = novoMovimento
                };
                _context.ProdutosMovimento.Add(prodMov);

                // 5. Atualiza o saldo real na tabela Produto
                var produtoNoBanco = await _context.Produtos.FindAsync(item.IdProduto);
                if (produtoNoBanco != null)
                {
                    // O estoque atual do produto vira o total contado no inventário
                    produtoNoBanco.EstoqueAtual = item.TotalContado;
                    _context.Update(produtoNoBanco);
                }
            }

            // 6. Fecha o inventário para evitar re-processamento
            inventario.Finalizado = true;
            _context.Update(inventario);

            // 7. Salva tudo em uma única transação
            await _context.SaveChangesAsync();

            TempData["Success"] = "Inventário finalizado, estoque ajustado e movimentação registrada!";
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
