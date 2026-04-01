using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly VarejoDbContext _context;

        public EstoqueRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<EstoqueConfig?> GetByProdutoIdAsync(int produtoId)
        {
            return await _context.EstoquesConfig
                .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
        }

        public async Task<List<EstoqueConfig>> GetAllAsync()
        {
            return await _context.EstoquesConfig.ToListAsync();
        }

        public async Task AddAsync(EstoqueConfig config)
        {
            _context.EstoquesConfig.Add(config);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EstoqueConfig config)
        {
            _context.EstoquesConfig.Update(config);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RegistrarMovimentacaoAsync(int produtoId, int movimentoId, int tipoId, int produtoEmbalagemId, decimal quantidadeInformada, string? observacao)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null) return false;

            // 1. Busca o movimento para saber a Espécie (Entrada/Saída/Ajuste)
            var movimento = await _context.Movimentos
                .Include(m => m.TipoMovimento)
                .ThenInclude(t => t.EspecieMovimento)
                .FirstOrDefaultAsync(m => m.IdMovimento == movimentoId);

            if (movimento == null) return false;

            // 2. Busca o multiplicador da embalagem (Ex: Caixa com 12)
            var produtoEmbalagem = await _context.ProdutosEmbalagem
                .Include(pe => pe.TipoEmbalagem)
                .FirstOrDefaultAsync(pe => pe.IdProdutoEmbalagem == produtoEmbalagemId);

            decimal multiplicador = produtoEmbalagem?.TipoEmbalagem?.Multiplicador ?? 1;
            decimal quantidadeReal = quantidadeInformada * multiplicador;

            // 3. Define o impacto no estoque baseado na Espécie
            decimal estoqueAnterior = produto.EstoqueAtual;
            decimal estoquePosterior = estoqueAnterior;
            decimal quantidadeParaHistorico = 0;

            var especieId = movimento.TipoMovimento.EspecieMovimento.IdEspecieMovimento;

            switch (especieId)
            {
                case 1: // Entrada
                    quantidadeParaHistorico = quantidadeReal;
                    estoquePosterior = estoqueAnterior + quantidadeReal;
                    break;
                case 2: // Saída
                    quantidadeParaHistorico = -quantidadeReal; // Fica negativo no Kardex
                    estoquePosterior = estoqueAnterior - quantidadeReal;
                    break;
                case 3: // Ajuste (Inventário)
                    quantidadeParaHistorico = quantidadeReal - estoqueAnterior;
                    estoquePosterior = quantidadeReal; // O valor informado vira o novo saldo
                    break;
            }

            // 4. Cria o registro de Histórico (Kardex)
            var historico = new HistoricoProduto
            {
                ProdutoId = produtoId,
                MovimentoId = movimentoId,
                Data = DateTime.Now,
                TipoMovimentoId = tipoId, // GRAVA O TIPO (Venda, Compra...)
                EspecieMovimentoId = especieId, // GRAVA A ESPÉCIE (Entrada, Saída...)
                QuantidadeMovimento = quantidadeParaHistorico,
                EstoqueAntes = estoqueAnterior,
                EstoqueDepois = estoquePosterior,
                Observacao = observacao
            };

            // 5. Atualiza o saldo real no cadastro do Produto
            produto.EstoqueAtual = estoquePosterior;

            _context.HistoricosProduto.Add(historico);
            _context.Produtos.Update(produto);

            // Nota: O SaveChangesAsync será chamado no Controller para garantir a transação única
            return true;
        }

        public async Task<bool> AjustarEstoqueInventarioAsync(int produtoId, int movimentoId, int inventarioId, decimal novaQuantidade, string? observacao)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null) return false;

            decimal estoqueAnterior = produto.EstoqueAtual;

            // 1. Cria o rastro no Histórico
            var historico = new HistoricoProduto
            {
                ProdutoId = produtoId,
                MovimentoId = movimentoId, // No inventário o vínculo é pelo campo Observacao ou Documento
                Data = DateTime.Now,
                TipoMovimentoId = 7, // FIXO PARA AJUSTE DE ESTOQUE
                EspecieMovimentoId = 3, //FIXO PARA AJUSTE DE ESTOQUE
                QuantidadeMovimento = novaQuantidade - estoqueAnterior, // A diferença gerada
                EstoqueAntes = estoqueAnterior,
                EstoqueDepois = novaQuantidade,
                Observacao = observacao ?? $"Ajuste via Inventário #{inventarioId}"
            };

            // 2. Atualiza o saldo real no cadastro do produto
            produto.EstoqueAtual = novaQuantidade;

            _context.HistoricosProduto.Add(historico);
            _context.Produtos.Update(produto);

            return true; // O SaveChangesAsync pode ser chamado na Controller ao fim do loop
        }

        public async Task<int> GerarSnapshotDiarioAsync()
        {
            var hoje = DateTime.Now.Date;

            // Limpa se já existir snapshot de hoje (para permitir re-processar se necessário)
            var antigos = _context.EstoquesSnapshot.Where(s => s.Data == hoje);
            _context.EstoquesSnapshot.RemoveRange(antigos);

            // Seleção direta para a Model de Snapshot
            var snapshots = await _context.Produtos.Select(p => new EstoqueSnapshot
            {
                ProdutoId = p.IdProduto,
                Data = hoje,
                Estoque = p.EstoqueAtual
            }).ToListAsync();

            await _context.EstoquesSnapshot.AddRangeAsync(snapshots);
            return await _context.SaveChangesAsync();
        }

        public async Task<EstoqueViewModel> ObterEstoqueGeralAsync(EstoqueFiltroViewModel filtro)
        {
            var query = _context.Produtos
     .Include(p => p.Familia)
         .ThenInclude(f => f.Marca) // Carrega a marca via família
     .Include(p => p.Familia)
         .ThenInclude(f => f.Categoria) // Carrega a categoria via família
     .Include(p => p.EstoqueConfig)
     .AsQueryable();

            // --- Filtros de Texto e IDs ---
            if (!string.IsNullOrEmpty(filtro.NomeProduto))
                query = query.Where(p => p.NomeProduto.Contains(filtro.NomeProduto));

            if(filtro.MarcaId.HasValue)
{
                // Navega: Produto -> Familia -> MarcaId
                query = query.Where(p => p.Familia != null && p.Familia.MarcaId == filtro.MarcaId);
            }


            if (filtro.CategoriaId.HasValue)
            {
                // Navega: Produto -> Familia -> CategoriaId
                query = query.Where(p => p.Familia != null && p.Familia.CategoriaId == filtro.CategoriaId);
            }

            // --- Filtros de Status (A Mágica) ---
            if (filtro.EstoqueNegativo)
                query = query.Where(p => p.EstoqueAtual < 0);

            if (filtro.EstoqueZerado)
                query = query.Where(p => p.EstoqueAtual == 0);

            if (filtro.AbaixoMinimo)
                query = query.Where(p => p.EstoqueConfig != null && p.EstoqueAtual < p.EstoqueConfig.EstoqueMinimo);

            if (filtro.AcimaMaximo)
                query = query.Where(p => p.EstoqueConfig != null && p.EstoqueAtual > p.EstoqueConfig.EstoqueMaximo);

            // --- Projeção para a ViewModel de Lista ---
            var itens = await query.Select(p => new EstoqueListViewModel
            {
                ProdutoId = p.IdProduto,
                NomeProduto = p.NomeProduto,
                Familia = p.Familia != null ? p.Familia.NomeFamilia : "Sem Família",
                EstoqueAtual = p.EstoqueAtual,
                EstoqueMinimo = p.EstoqueConfig != null ? p.EstoqueConfig.EstoqueMinimo : 0,
                EstoqueMaximo = p.EstoqueConfig != null ? p.EstoqueConfig.EstoqueMaximo : 0,
                AbaixoMinimo = p.EstoqueConfig != null && p.EstoqueAtual < p.EstoqueConfig.EstoqueMinimo,
                AcimaMaximo = p.EstoqueConfig != null && p.EstoqueAtual > p.EstoqueConfig.EstoqueMaximo
            }).ToListAsync();

            return new EstoqueViewModel
            {
                Itens = itens,
                Filtro = filtro
            };
        }
    }
}
