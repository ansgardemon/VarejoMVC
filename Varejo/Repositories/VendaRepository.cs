using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly VarejoDbContext _context;
        private readonly IEstoqueRepository _estoqueRepo;

        public VendaRepository(VarejoDbContext context, IEstoqueRepository estoqueRepo)
        {
            _context = context;
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
            var venda = await GetByIdAsync(vendaId);
            if (venda == null || venda.Finalizada) return false;

            // Iniciamos uma transação para garantir que ou grava TUDO ou NADA
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Obter Parâmetros de Movimentação para Venda
                var parametros = await _context.Parametros.FirstOrDefaultAsync();
                if (parametros == null) throw new Exception("Parâmetros de venda não configurados.");

                // 2. Criar o Movimento de Estoque (Cabeçalho)
                var movimento = new Movimento
                {
                    Documento = venda.IdVenda, // Usamos o ID da venda como número do doc
                    DataMovimento = DateTime.Now,
                    TipoMovimentoId = parametros.TipoMovimentoVendaId,
                    PessoaId = venda.PessoaId,
                    Observacao = $"Faturamento Venda #{venda.IdVenda}"
                };
                _context.Movimentos.Add(movimento);
                await _context.SaveChangesAsync();

                // 3. Processar Itens: Estoque, Kardex e MovimentoProduto
                foreach (var item in venda.Itens)
                {
                    // Grava o item no movimento (Para relatórios fiscais/gerenciais)
                    var movProd = new ProdutoMovimento
                    {
                        MovimentoId = movimento.IdMovimento,
                        ProdutoId = item.ProdutoId,
                        ProdutoEmbalagemId = item.ProdutoEmbalagemId,
                        Quantidade = item.Quantidade
                    };
                    _context.ProdutosMovimento.Add(movProd);

                    // BAIXA NO ESTOQUE REAL (Reutilizando seu método core)
                    // Ele já vai tratar o multiplicador da embalagem e gerar o HistoricoProduto
                    await _estoqueRepo.RegistrarMovimentacaoAsync(
                        item.ProdutoId,
                        movimento.IdMovimento,
                        movimento.TipoMovimentoId,
                        item.ProdutoEmbalagemId,
                        item.Quantidade,
                        movimento.Observacao
                    );
                }

                // 4. Gerar Título Financeiro
                // Aqui usamos o ValorFinal (Subtotal - DescontoTotal) da sua Model Venda
                var titulo = new TituloFinanceiro
                {
                    Documento = venda.IdVenda,
                    Parcela = 1,
                    PessoaId = venda.PessoaId,
                    Valor = venda.ValorFinal,
                    DataEmissao = DateTime.Now,
                    DataVencimento = DateTime.Now.AddDays(30), // Simplificado, pode vir do PrazoPagamento
                    EspecieTituloId = 1, // Ex: "Venda"
                    FormaPagamentoId = venda.FormaPagamentoId,
                    PrazoPagamentoId = venda.PrazoPagamentoId,
                    Observacao = $"Venda #{venda.IdVenda}"
                };
                titulo.AtualizarValores();
                _context.TitulosFinanceiro.Add(titulo);

                // 5. Marcar Venda como Finalizada
                venda.Finalizada = true;
                _context.Vendas.Update(venda);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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