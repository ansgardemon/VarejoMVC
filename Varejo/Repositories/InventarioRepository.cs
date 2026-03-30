using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;

namespace Varejo.Repositories
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly VarejoDbContext _context;
        private readonly IProdutoMovimentoRepository _produtoMovimentoRepository;

        public InventarioRepository(
            VarejoDbContext context,
            IProdutoMovimentoRepository movimentoRepo)
        {
            _context = context;
            _produtoMovimentoRepository = movimentoRepo;
        }

        public async Task<List<Inventario>> GetAllAsync()
        {
            return await _context.Inventarios
                .Include(i => i.Itens)
                .ToListAsync();
        }

        public async Task<Inventario?> GetByIdAsync(int id)
        {
            return await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task CriarInventarioAsync(Inventario inventario)
        {
            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();
        }

        public async Task AddItemAsync(InventarioItem item)
        {
            // 1. Busca apenas pelos IDs (que o controller enviou)
            var existente = await _context.InventariosItem
                .FirstOrDefaultAsync(i => i.InventarioId == item.InventarioId
                                     && i.ProdutoId == item.ProdutoId
                                     && i.ProdutoEmbalagemId == item.ProdutoEmbalagemId);

            if (existente != null)
            {
                // Se já existe essa combinação no inventário, apenas soma a nova contagem
                existente.QuantidadeContada += item.QuantidadeContada;
                _context.InventariosItem.Update(existente);
            }
            else
            {
                // Se é novo, adiciona
                _context.InventariosItem.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task FinalizarInventarioAsync(int inventarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var inventario = await _context.Inventarios
                    .Include(i => i.Itens)
                    .FirstOrDefaultAsync(i => i.Id == inventarioId);

                if (inventario == null || inventario.Finalizado)
                    throw new Exception("Inventário não encontrado ou já finalizado.");

                // 1. Localiza o Tipo de Movimento que pertence à Espécie 3 (Ajuste)
                var tipoAjuste = await _context.TiposMovimento
                    .FirstOrDefaultAsync(t => t.EspecieMovimentoId == 3);

                if (tipoAjuste == null)
                    throw new Exception("Configuração de Tipo de Movimento para Espécie 3 (Ajuste) não encontrada.");

                // 2. Cria o cabeçalho do Movimento
                var movimento = new Movimento
                {
                    DataMovimento = DateTime.Now,
                    TipoMovimentoId = tipoAjuste.IdTipoMovimento,
                    Observacao = $"Ajuste automático via Inventário #{inventario.Id}",
                    PessoaId = 1 // Pessoa padrão/Sistema
                };

                _context.Movimentos.Add(movimento);
                await _context.SaveChangesAsync(); // Gera o IdMovimento

                // 3. Processa os itens
                // Agrupamos por produto para garantir que se o cara contou "Caixa" e "Unidade" separadamente, 
                // a gente mande um único ajuste final com a soma total convertida.
                var itensAgrupados = inventario.Itens.GroupBy(i => i.ProdutoId);

                foreach (var grupo in itensAgrupados)
                {
                    decimal totalConvertidoParaUnidade = 0;
                    int? embalagemPadraoId = null;

                    foreach (var item in grupo)
                    {
                        var emb = await _context.ProdutosEmbalagem
                            .Include(pe => pe.TipoEmbalagem)
                            .FirstOrDefaultAsync(pe => pe.IdProdutoEmbalagem == item.ProdutoEmbalagemId);

                        var mult = emb?.TipoEmbalagem?.Multiplicador ?? 1;
                        totalConvertidoParaUnidade += item.QuantidadeContada * mult;

                        // Guardamos uma embalagem de referência (preferencialmente a unidade) 
                        // para registrar no ProdutoMovimento
                        if (embalagemPadraoId == null) embalagemPadraoId = item.ProdutoEmbalagemId;
                    }

                    // 4. Cria o registro em ProdutoMovimento
                    var prodMov = new ProdutoMovimento
                    {
                        MovimentoId = movimento.IdMovimento,
                        ProdutoId = grupo.Key,
                        ProdutoEmbalagemId = embalagemPadraoId.Value,
                        Quantidade = totalConvertidoParaUnidade // O seu repo Species 3 vai setar isso direto no estoque
                    };

                    _context.ProdutosMovimento.Add(prodMov);
                    await _context.SaveChangesAsync();

                    // 5. CHAMA O SEU REPOSITORY EXISTENTE
                    // Isso vai disparar o switch(especieId) case 3 que você já tem pronto.
                    await _produtoMovimentoRepository.AtualizarEstoqueAsync(prodMov);
                }

                // 6. Finaliza o lote de inventário
                inventario.Finalizado = true;
                _context.Inventarios.Update(inventario);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Itens)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null)
                throw new Exception("Inventário não encontrado.");

            if (inventario.Finalizado)
                throw new Exception("Não é possível excluir um inventário finalizado.");

            // 🔥 remove os itens primeiro
            if (inventario.Itens != null && inventario.Itens.Any())
                _context.InventariosItem.RemoveRange(inventario.Itens);

            _context.Inventarios.Remove(inventario);

            await _context.SaveChangesAsync();
        }

        public async Task AtualizarItemAsync(InventarioItem item)
        {
            var existente = await _context.InventariosItem
                .FirstOrDefaultAsync(i => i.Id == item.Id);

            if (existente == null)
                throw new Exception("Item não encontrado.");

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.Id == existente.InventarioId);

            if (inventario != null && inventario.Finalizado)
                throw new Exception("Inventário finalizado não pode ser alterado.");

            existente.QuantidadeContada = item.QuantidadeContada;

            _context.InventariosItem.Update(existente);
            await _context.SaveChangesAsync();
        }


        public async Task RemoverItemAsync(int itemId)
        {
            var item = await _context.InventariosItem
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                throw new Exception("Item não encontrado.");

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.Id == item.InventarioId);

            if (inventario != null && inventario.Finalizado)
                throw new Exception("Inventário finalizado não pode ser alterado.");

            _context.InventariosItem.Remove(item);
            await _context.SaveChangesAsync();
        }
    }


}
