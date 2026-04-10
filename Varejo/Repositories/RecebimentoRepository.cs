using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class RecebimentoRepository : IRecebimentoRepository
    {
        private readonly VarejoDbContext _context;
        private readonly ITituloFinanceiroRepository _financeiroRepo;
        private readonly IEstoqueRepository _estoqueRepo;

        public RecebimentoRepository(
            VarejoDbContext context,
            ITituloFinanceiroRepository financeiroRepo,
            IEstoqueRepository estoqueRepo)
        {
            _context = context;
            _financeiroRepo = financeiroRepo;
            _estoqueRepo = estoqueRepo;
        }

        public async Task<Recebimento> GetByIdAsync(int id) =>
            await _context.Recebimentos
                .Include(r => r.Fornecedor)
                .Include(r => r.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(r => r.Itens)
                    .ThenInclude(i => i.ProdutoEmbalagem)
                .FirstOrDefaultAsync(r => r.IdRecebimento == id);

        public async Task<bool> ExisteChaveAcessoAsync(string chaveAcesso) =>
            await _context.Recebimentos.AnyAsync(r => r.ChaveAcesso == chaveAcesso);

        public async Task<bool> RegistrarRecebimentoAsync(Recebimento recebimento)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parametros = await _context.Parametros.AsNoTracking().FirstOrDefaultAsync();
                if (parametros == null)
                    throw new Exception("Parâmetros do sistema não configurados.");

                // 1. Salva o Recebimento (Capa da Nota)
                _context.Recebimentos.Add(recebimento);
                await _context.SaveChangesAsync(); // Gera o IdRecebimento aqui

                var itensCompra = recebimento.Itens.Where(i => i.EhBonificacao == false).ToList();
                var itensBonif = recebimento.Itens.Where(i => i.EhBonificacao == true).ToList();

                // --- FUNÇÃO AUXILIAR PARA PROCESSAR OS MOVIMENTOS ---
                async Task ProcessarGrupoMovimento(List<RecebimentoItem> itens, int tipoMovimentoId, string sufixoObs)
                {
                    if (!itens.Any()) return;

                    var movimento = new Movimento
                    {
                        Documento = int.TryParse(recebimento.NumeroNota, out var n) ? n : recebimento.IdRecebimento,
                        DataMovimento = recebimento.DataEntrada,
                        TipoMovimentoId = tipoMovimentoId,
                        PessoaId = recebimento.PessoaId,
                        RecebimentoId = recebimento.IdRecebimento, // VÍNCULO DIRETO DA NOVA COLUNA
                        Observacao = $"NF: {recebimento.NumeroNota} - {sufixoObs}"
                    };

                    _context.Movimentos.Add(movimento);
                    await _context.SaveChangesAsync();

                    foreach (var item in itens)
                    {
                        var produtoMovimento = new ProdutoMovimento
                        {
                            MovimentoId = movimento.IdMovimento,
                            ProdutoId = item.ProdutoId,
                            ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                            Quantidade = item.Quantidade
                        };
                        _context.ProdutosMovimento.Add(produtoMovimento);

                        var estoqueOk = await _estoqueRepo.RegistrarMovimentacaoAsync(
                            produtoId: item.ProdutoId,
                            movimentoId: movimento.IdMovimento,
                            tipoId: movimento.TipoMovimentoId,
                            produtoEmbalagemId: item.ProdutoEmbalagemId,
                            quantidadeInformada: item.Quantidade,
                            observacao: $"{sufixoObs} NF {recebimento.NumeroNota}"
                        );

                        if (!estoqueOk) throw new Exception($"Erro no estoque: Produto {item.ProdutoId}");

                        if (tipoMovimentoId == parametros.TipoMovimentoCompraId)
                        {
                            var custosAntigos = await _context.ProdutosCusto
                                .Where(c => c.ProdutoId == item.ProdutoId && c.EhCustoAtual)
                                .ToListAsync();

                            foreach (var c in custosAntigos) c.EhCustoAtual = false;

                            _context.ProdutosCusto.Add(new ProdutoCusto
                            {
                                ProdutoId = item.ProdutoId,
                                RecebimentoId = recebimento.IdRecebimento,
                                ValorCustoUnitario = item.ValorUnitario,
                                EhCustoAtual = true,
                                DataRegistro = DateTime.Now
                            });
                        }
                    }
                }

                // 2. Processar Movimentos
                await ProcessarGrupoMovimento(itensCompra, parametros.TipoMovimentoCompraId, "Importação XML");
                await ProcessarGrupoMovimento(itensBonif, parametros.TipoMovimentoEntradaBonificacaoId, "Bonificação XML");

                // 3. Financeiro (Filtrando apenas itens de compra)
                decimal valorTotalFinanceiro = itensCompra.Sum(i => i.Quantidade * i.ValorUnitario);

                if (recebimento.PrazoPagamentoId.HasValue && valorTotalFinanceiro > 0)
                {
                    await _financeiroRepo.GerarTitulosAsync(
                        documento: int.TryParse(recebimento.NumeroNota, out var doc) ? doc : recebimento.IdRecebimento,
                        valorTotal: valorTotalFinanceiro,
                        prazoPagamentoId: recebimento.PrazoPagamentoId.Value,
                        especieTituloId: 1, // Entrada/Compra
                        formaPagamentoId: recebimento.FormaPagamentoId,
                        pessoaId: recebimento.PessoaId,
                        dataEmissao: recebimento.DataEntrada,
                        recebimentoId: recebimento.IdRecebimento, // Passa o ID do recebimento
                        vendaId: null // Explicitamente nulo para recebimentos
                    );
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Falha ao registrar recebimento: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Recebimento>> GetAllAsync()
        {
            return await _context.Recebimentos
               .Include(r => r.Fornecedor)
               .AsNoTracking()
               .OrderByDescending(r => r.DataEntrada)
               .ToListAsync();
        }

        public async Task<bool> DesintegrarRecebimentoAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recebimento = await _context.Recebimentos
                    .Include(r => r.Itens)
                    .FirstOrDefaultAsync(r => r.IdRecebimento == id);

                if (recebimento == null) return false;

                // 1. Estorno de Estoque
                foreach (var item in recebimento.Itens)
                {
                    await _estoqueRepo.EstornarMovimentacaoAsync(id, item.ProdutoId);
                }

                // 2. Identificar movimentos e títulos vinculados pelo ID do Recebimento (Blindado)
                var movimentosParaDeletarIds = await _context.Movimentos
                    .Where(m => m.RecebimentoId == id)
                    .Select(m => m.IdMovimento)
                    .ToListAsync();

                // 3. Limpeza do Kardex
                var historicos = _context.HistoricosProduto.Where(h => movimentosParaDeletarIds.Contains(h.MovimentoId));
                _context.HistoricosProduto.RemoveRange(historicos);

                // 4. Limpeza dos Itens do Movimento
                var itensMov = _context.ProdutosMovimento.Where(pm => movimentosParaDeletarIds.Contains(pm.MovimentoId));
                _context.ProdutosMovimento.RemoveRange(itensMov);

                // 5. Limpeza da Capa dos Movimentos
                var movimentos = _context.Movimentos.Where(m => m.RecebimentoId == id);
                _context.Movimentos.RemoveRange(movimentos);

                // 6. Financeiro: Deleta apenas o que nasceu deste recebimento
                var titulos = _context.TitulosFinanceiro.Where(t => t.RecebimentoId == id);
                _context.TitulosFinanceiro.RemoveRange(titulos);

                // 7. Limpeza de Custos
                var custos = _context.ProdutosCusto.Where(c => c.RecebimentoId == id);
                _context.ProdutosCusto.RemoveRange(custos);

                // 8. Limpeza dos itens e capa do Recebimento
                _context.RecebimentosItem.RemoveRange(recebimento.Itens);
                _context.Recebimentos.Remove(recebimento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Erro na desintegração: {ex.Message}");
            }
        }
    }
}