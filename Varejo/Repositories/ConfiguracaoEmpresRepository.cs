using Microsoft.EntityFrameworkCore;
using Varejo.Data;
using Varejo.Models;
using Varejo.Interfaces;

namespace Varejo.Repositories
{
 
   
        public class ConfiguracaoEmpresaRepository : IConfiguracaoEmpresaRepository
        {
            private readonly VarejoDbContext _context;

            public ConfiguracaoEmpresaRepository(VarejoDbContext context)
            {
                _context = context;
            }

            public async Task<ConfiguracaoEmpresa> GetConfiguracaoAsync()
            {
                // Pega o primeiro registro encontrado (geralmente o único)
                return await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync();
            }

            public async Task<bool> SalvarConfiguracaoAsync(ConfiguracaoEmpresa config)
            {
                try
                {
                    var registroAtual = await _context.ConfiguracoesEmpresa.FirstOrDefaultAsync();

                    if (registroAtual == null)
                    {
                        // Se não existe, adiciona o primeiro
                        _context.ConfiguracoesEmpresa.Add(config);
                    }
                    else
                    {
                        // Se já existe, atualiza os dados do registro existente
                        registroAtual.RazaoSocial = config.RazaoSocial;
                        registroAtual.Cnpj = config.Cnpj;
                        registroAtual.NomeFantasia = config.NomeFantasia;
                        registroAtual.Logotipo = config.Logotipo;

                        _context.ConfiguracoesEmpresa.Update(registroAtual);
                    }

                    return await _context.SaveChangesAsync() > 0;
                }
                catch (Exception ex)
                {
                    // Aqui você pode logar o erro conforme seu padrão
                    return false;
                }
            }
        }
    }

