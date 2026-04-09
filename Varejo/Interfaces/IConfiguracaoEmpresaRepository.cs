using System;
using Varejo.Models;

namespace Varejo.Interfaces
{




        public interface IConfiguracaoEmpresaRepository
        {
            // Busca a configuração única. Se não existir, retorna null ou um objeto vazio
            Task<ConfiguracaoEmpresa> GetConfiguracaoAsync();

            // Cria ou Atualiza a configuração (Upsert)
            Task<bool> SalvarConfiguracaoAsync(ConfiguracaoEmpresa config);
        }

}
