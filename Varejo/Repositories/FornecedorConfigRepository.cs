using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Interfaces;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Repositories
{
    public class FornecedorConfigRepository : IFornecedorConfigRepository
    {
        private readonly VarejoDbContext _context;

        public FornecedorConfigRepository(VarejoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Familia>> GetFamiliasPorFornecedorAsync(int pessoaId)
        {
            return await _context.FornecedoresFamilia
                .Where(ff => ff.PessoaId == pessoaId)
                .Select(ff => ff.Familia)
                .ToListAsync();
        }

        public async Task<bool> VincularFamiliaAsync(int pessoaId, int familiaId)
        {
            if (await _context.FornecedoresFamilia.AnyAsync(ff => ff.PessoaId == pessoaId && ff.FamiliaId == familiaId))
                return true;

            _context.FornecedoresFamilia.Add(new FornecedorFamilia { PessoaId = pessoaId, FamiliaId = familiaId });
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SalvarLoteVinculosAsync(int pessoaId, List<ItemVinculoViewModel> itens)
        {
            foreach (var item in itens.Where(i => !string.IsNullOrEmpty(i.CodigoNoFornecedor)))
            {
                var vinculoExistente = await _context.ProdutosFornecedorVinculo
                    .FirstOrDefaultAsync(v => v.PessoaId == pessoaId && v.ProdutoId == item.ProdutoId);

                if (vinculoExistente != null)
                {
                    vinculoExistente.CodigoProdutoNoFornecedor = item.CodigoNoFornecedor;
                    vinculoExistente.DescricaoNoFornecedor = item.DescricaoNoFornecedor;
                }
                else
                {
                    _context.ProdutosFornecedorVinculo.Add(new ProdutoFornecedorVinculo
                    {
                        PessoaId = pessoaId,
                        ProdutoId = item.ProdutoId,
                        CodigoProdutoNoFornecedor = item.CodigoNoFornecedor,
                        DescricaoNoFornecedor = item.DescricaoNoFornecedor
                    });
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoverFamiliaAsync(int pessoaId, int familiaId)
        {
            var vinculo = await _context.FornecedoresFamilia
                .FirstOrDefaultAsync(ff => ff.PessoaId == pessoaId && ff.FamiliaId == familiaId);

            if (vinculo != null)
            {
                _context.FornecedoresFamilia.Remove(vinculo);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<ProdutoFornecedorVinculo>> GetVinculosPorFornecedorAsync(int pessoaId)
        {
            return await _context.ProdutosFornecedorVinculo
                .Where(v => v.PessoaId == pessoaId)
                .Include(v => v.Produto)
                .ToListAsync();
        }

        public async Task<ProdutoFornecedorVinculo> GetVinculoPorCodigoExternoAsync(int pessoaId, string codigoExterno)
        {
            return await _context.ProdutosFornecedorVinculo
                .FirstOrDefaultAsync(v => v.PessoaId == pessoaId && v.CodigoProdutoNoFornecedor == codigoExterno);
        }

        public async Task<bool> SalvarVinculoProdutoAsync(ProdutoFornecedorVinculo vinculo)
        {
            var existente = await _context.ProdutosFornecedorVinculo
                .FirstOrDefaultAsync(v => v.PessoaId == vinculo.PessoaId && v.ProdutoId == vinculo.ProdutoId);

            if (existente != null)
            {
                existente.CodigoProdutoNoFornecedor = vinculo.CodigoProdutoNoFornecedor;
                existente.DescricaoNoFornecedor = vinculo.DescricaoNoFornecedor;
                _context.ProdutosFornecedorVinculo.Update(existente);
            }
            else
            {
                _context.ProdutosFornecedorVinculo.Add(vinculo);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
