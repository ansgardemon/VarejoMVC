using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly VarejoDbContext _context;
        private readonly ITituloFinanceiroRepository _financeiroRepo;
        private readonly IEstoqueRepository _estoqueRepo;

        public VendaRepository(
            VarejoDbContext context,
            ITituloFinanceiroRepository financeiroRepo,
            IEstoqueRepository estoqueRepo)
        {
            _context = context;
            _financeiroRepo = financeiroRepo;
            _estoqueRepo = estoqueRepo;
        }

        public async Task<Venda?> GetByIdAsync(int id)
        {
            return await _context.Vendas
                .Include(v => v.Pessoa)
                .Include(v => v.FormaPagamento)
                .Include(v => v.PrazoPagamento)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.IdVenda == id);
        }

        public async Task<List<Venda>> GetAllAsync()
        {
            return await _context.Vendas.Include(v => v.Pessoa).OrderByDescending(v => v.DataVenda).ToListAsync();
        }

        public async Task<Venda> CriarPedidoAsync(Venda venda)
        {
            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();
            return venda;
        }

        public async Task<bool> AtualizarPedidoAsync(Venda venda)
        {
            _context.Vendas.Update(venda);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> FaturarVendaAsync(int vendaId)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.IdVenda == vendaId);

            if (venda == null || venda.Finalizada) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Buscar o Tipo de Movimento nos Parâmetros
                var parametros = await _context.Parametros.FirstOrDefaultAsync();
                if (parametros == null) throw new Exception("Parâmetros do sistema não configurados.");

                // 2. Criar o Cabeçalho do Movimento
                var movimento = new Movimento
                {
                    Documento = venda.IdVenda,
                    DataMovimento = DateTime.Now,
                    TipoMovimentoId = parametros.TipoMovimentoVendaId,
                    PessoaId = venda.PessoaId,
                    Observacao = $"Faturamento Venda #{venda.IdVenda}"
                };
                _context.Movimentos.Add(movimento);
                await _context.SaveChangesAsync(); // Gera o IdMovimento

                // 3. Processar Itens
                foreach (var item in venda.Itens)
                {
                    // --- O QUE ESTAVA FALTANDO: Gravar o detalhe do movimento ---
                    var produtoMovimento = new ProdutoMovimento
                    {
                        MovimentoId = movimento.IdMovimento,
                        ProdutoId = item.ProdutoId,
                        ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                        Quantidade = item.Quantidade
                    };
                    _context.ProdutosMovimento.Add(produtoMovimento);
                    // -----------------------------------------------------------

                    // Registrar no Estoque/Histórico (Seu método que já funciona)
                    var ok = await _estoqueRepo.RegistrarMovimentacaoAsync(
                        produtoId: item.ProdutoId,
                        movimentoId: movimento.IdMovimento,
                        tipoId: movimento.TipoMovimentoId,
                        produtoEmbalagemId: item.ProdutoEmbalagemId,
                        quantidadeInformada: item.Quantidade,
                        observacao: $"Venda #{venda.IdVenda}"
                    );

                    if (!ok) throw new Exception($"Erro ao baixar estoque do produto ID {item.ProdutoId}");
                }

                // 4. Gerar o Financeiro (Seu método que faz 30/60/90 dias)
                if (venda.PrazoPagamentoId.HasValue)
                {
                    await _financeiroRepo.GerarTitulosAsync(
                        documento: venda.IdVenda,
                        valorTotal: venda.ValorFinal,
                        prazoPagamentoId: venda.PrazoPagamentoId.Value,
                        especieTituloId: 2,
                        formaPagamentoId: venda.FormaPagamentoId,
                        pessoaId: venda.PessoaId,
                        dataEmissao: DateTime.Now,
                        recebimentoId: null, // Não é um recebimento
                        vendaId: venda.IdVenda // É uma venda!
                    );
                }

                // 5. Finalizar a Venda
                venda.Finalizada = true;
                _context.Vendas.Update(venda);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Falha no faturamento: " + ex.Message);
            }
        }

        public async Task<bool> CancelarVendaAsync(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null || venda.Finalizada) return false;

            _context.Vendas.Remove(venda);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}