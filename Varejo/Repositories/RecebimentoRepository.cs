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

                // 2. Cria o Cabeçalho do Movimento de Estoque
                var movimento = new Movimento
                {
                    Documento = int.TryParse(recebimento.NumeroNota, out var n) ? n : recebimento.IdRecebimento,
                    DataMovimento = recebimento.DataEntrada,
                    TipoMovimentoId = parametros.TipoMovimentoCompraId,
                    PessoaId = recebimento.PessoaId,
                    Observacao = $"NF: {recebimento.NumeroNota} - Importação XML"
                };

                _context.Movimentos.Add(movimento);
                await _context.SaveChangesAsync(); // Gera o IdMovimento

                decimal valorTotalNota = 0;

                // 3. Processar Itens
                foreach (var item in recebimento.Itens)
                {
                    valorTotalNota += (item.Quantidade * item.ValorUnitario);

                    // --- CORREÇÃO: Gravar o detalhe do movimento (ProdutoMovimento) ---
                    var produtoMovimento = new ProdutoMovimento
                    {
                        MovimentoId = movimento.IdMovimento,
                        ProdutoId = item.ProdutoId,
                        ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                        Quantidade = item.Quantidade
                    };
                    _context.ProdutosMovimento.Add(produtoMovimento);
                    // ----------------------------------------------------------------

                    // 4. Registrar no Estoque/Histórico/Kardex (via repositório de estoque)
                    var estoqueOk = await _estoqueRepo.RegistrarMovimentacaoAsync(
                        produtoId: item.ProdutoId,
                        movimentoId: movimento.IdMovimento,
                        tipoId: movimento.TipoMovimentoId,
                        produtoEmbalagemId: item.ProdutoEmbalagemId,
                        quantidadeInformada: item.Quantidade,
                        observacao: $"Entrada NF {recebimento.NumeroNota}"
                    );

                    if (!estoqueOk) throw new Exception($"Erro no estoque: Produto {item.ProdutoId}");

                    // 5. Atualização de Custo (Invalida o atual e cria o novo)
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

                // 6. Financeiro (Gera as parcelas do Contas a Pagar)
                if (recebimento.PrazoPagamentoId.HasValue)
                {
                    await _financeiroRepo.GerarTitulosAsync(
                        documento: movimento.Documento ?? 0,
                        valorTotal: valorTotalNota,
                        prazoPagamentoId: recebimento.PrazoPagamentoId.Value,
                        especieTituloId: 1, // Espécie para Compra/Entrada
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
                // Melhorei o log para você identificar onde falhou nesses 3 dias de prazo
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