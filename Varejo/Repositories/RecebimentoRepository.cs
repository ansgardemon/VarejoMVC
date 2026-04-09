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
                .Include(r => r.Fornecedor) // AJUSTADO: era Pessoa
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
                await _context.SaveChangesAsync();

                // Use ToList() para desvincular da coleção rastreada se necessário
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
                        Observacao = $"NF: {recebimento.NumeroNota} - {sufixoObs}"
                    };

                    _context.Movimentos.Add(movimento);
                    await _context.SaveChangesAsync(); // Gera o IdMovimento

                    foreach (var item in itens)
                    {
                        // Gravar o detalhe do movimento
                        var produtoMovimento = new ProdutoMovimento
                        {
                            MovimentoId = movimento.IdMovimento,
                            ProdutoId = item.ProdutoId,
                            ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                            Quantidade = item.Quantidade
                        };
                        _context.ProdutosMovimento.Add(produtoMovimento);

                        // Registrar no Estoque
                        var estoqueOk = await _estoqueRepo.RegistrarMovimentacaoAsync(
                            produtoId: item.ProdutoId,
                            movimentoId: movimento.IdMovimento,
                            tipoId: movimento.TipoMovimentoId,
                            produtoEmbalagemId: item.ProdutoEmbalagemId,
                            quantidadeInformada: item.Quantidade,
                            observacao: $"{sufixoObs} NF {recebimento.NumeroNota}"
                        );

                        if (!estoqueOk) throw new Exception($"Erro no estoque: Produto {item.ProdutoId}");

                        // Atualização de Custo (Somente se não for bonificação, ou conforme sua regra de negócio)
                        // Geralmente bonificação não gera novo custo médio por ser valor zero na nota
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

                // 2. Processar Movimento de Compra
                await ProcessarGrupoMovimento(itensCompra, parametros.TipoMovimentoCompraId, "Importação XML");

                // 3. Processar Movimento de Bonificação (Usando a nova coluna da migration)
                await ProcessarGrupoMovimento(itensBonif, parametros.TipoMovimentoEntradaBonificacaoId, "Bonificação XML");

                // 4. Financeiro (Apenas sobre os itens que NÃO são bonificação)
                decimal valorTotalFinanceiro = itensCompra.Sum(i => i.Quantidade * i.ValorUnitario);

                if (recebimento.PrazoPagamentoId.HasValue && valorTotalFinanceiro > 0)
                {
                    await _financeiroRepo.GerarTitulosAsync(
                        documento: int.TryParse(recebimento.NumeroNota, out var doc) ? doc : recebimento.IdRecebimento,
                        valorTotal: valorTotalFinanceiro,
                        prazoPagamentoId: recebimento.PrazoPagamentoId.Value,
                        especieTituloId: 1,
                        formaPagamentoId: recebimento.FormaPagamentoId,
                        pessoaId: recebimento.PessoaId,
                        dataEmissao: recebimento.DataEntrada
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
               .Include(r => r.Fornecedor) // AJUSTADO: era Pessoa
               .AsNoTracking()
               .OrderByDescending(r => r.DataEntrada)
               .ToListAsync();
        }
    }
}