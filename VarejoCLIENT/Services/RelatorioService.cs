using Microsoft.JSInterop;
using System.Net.Http.Json;
using VarejoSHARED.DTO;
using VarejoSHARED.DTO.Relatorios;

namespace VarejoCLIENT.Services
{
    public class RelatorioService
    {
        #region DEPENDÊNCIAS E CONSTRUTOR
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public RelatorioService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }
        #endregion

        #region MENU E DEFINIÇÕES
        public async Task<List<RelatorioDefinicaoDTO>> GetMenuRelatoriosAsync()
        {
            // É daqui que a sua tela de menu puxa os "Cards". 
            // Quando tiver banco de dados para isso, é só trocar esse Task.FromResult por uma chamada _http.GetFromJsonAsync
            return await Task.FromResult(new List<RelatorioDefinicaoDTO>
            {
                // MÓDULO 100 - PRODUTOS
                new RelatorioDefinicaoDTO { Codigo = 101, Nome = "Produtos Categorizados", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 102, Nome = "PRECIFICAÇÃO E MARGENS DE LUCRO", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 103, Nome = "Movimento de Estoque por Produto", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 104, Nome = "Curva ABC de Produtos", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 105, Nome = "Produtos Sem Giro", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 106, Nome = "Ranking de Vendas (Mais/Menos)", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 107, Nome = "Histórico de Alteração de Preços", Categoria = "#100 - Produtos" },
                
                // MÓDULO 200 - ESTOQUE
                new RelatorioDefinicaoDTO { Codigo = 201, Nome = "Posição Atual de Estoque", Categoria = "#200 - Estoque" },
                new RelatorioDefinicaoDTO { Codigo = 202, Nome = "Lotes e Validades", Categoria = "#200 - Estoque" },
                new RelatorioDefinicaoDTO { Codigo = 203, Nome = "Movimentação de Estoque Geral", Categoria = "#200 - Estoque" },
                
                // MÓDULO 300 - MOVIMENTAÇÕES
                new RelatorioDefinicaoDTO { Codigo = 301, Nome = "Histórico de Movimentações", Categoria = "#300 - Movimentações" }
            });
        }

        public async Task<RelatorioDefinicaoDTO?> GetDefinicaoAsync(int codigo)
        {
            var menu = await GetMenuRelatoriosAsync();
            return menu.FirstOrDefault(x => x.Codigo == codigo);
        }
        #endregion

        #region BUSCAS E DADOS DOS RELATÓRIOS
        // Relatório 101 (Produtos)
        public async Task<List<ProdutoDTO>> GetDadosRelatorio101Async(RelatorioFiltroProdutosDTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/101/dados", filtro);
            return await response.Content.ReadFromJsonAsync<List<ProdutoDTO>>() ?? new();
        }

        // Relatorio 102 (Produto por valor)
        public async Task<List<Relatorio102DTO>?> GetDadosRelatorio102Async(RelatorioFiltroProdutosDTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/102/dados", filtro);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Relatorio102DTO>>();
                }

                return new List<Relatorio102DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 102: {ex.Message}");
                return new List<Relatorio102DTO>();
            }
        }

        // Relatório 103 (Movimento por Produto)
        public async Task<List<Relatorio103DTO>> GetDadosRelatorio103Async(RelatorioFiltro103DTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/103/dados", filtro);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Relatorio103DTO>>() ?? new();
            }
            return new List<Relatorio103DTO>();
        }

        // Relatório 104 (Curva ABC)
        public async Task<List<Relatorio104DTO>?> GetDadosRelatorio104Async(RelatorioFiltroProdutosDTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/104/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio104DTO>>();

                return new List<Relatorio104DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 104: {ex.Message}");
                return new List<Relatorio104DTO>();
            }
        }

        // Relatório 105 (Produtos sem giro)
        public async Task<List<Relatorio105DTO>?> GetDadosRelatorio105Async(RelatorioFiltro105DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/105/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio105DTO>>();

                return new List<Relatorio105DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 105: {ex.Message}");
                return new List<Relatorio105DTO>();
            }
        }

        // Relatório 106 (Ranking de vendas)
        public async Task<List<Relatorio106DTO>?> GetDadosRelatorio106Async(RelatorioFiltro106DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/106/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio106DTO>>();

                return new List<Relatorio106DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 106: {ex.Message}");
                return new List<Relatorio106DTO>();
            }
        }

        // Relatório 107 (Histórico de Alteração de Preços)
        public async Task<List<Relatorio107DTO>?> GetDadosRelatorio107Async(RelatorioFiltroProdutosDTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/107/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio107DTO>>();
                return new List<Relatorio107DTO>();
            }
            catch { return new List<Relatorio107DTO>(); }
        }

        // Relatório 201 (Posição atual de estoque)
        public async Task<List<Relatorio201DTO>?> GetDadosRelatorio201Async(RelatorioFiltro201DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/201/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio201DTO>>();

                return new List<Relatorio201DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 201: {ex.Message}");
                return new List<Relatorio201DTO>();
            }
        }

        // Relatório 202 (Lotes e validades)
        public async Task<List<Relatorio202DTO>?> GetDadosRelatorio202Async(RelatorioFiltro202DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/202/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio202DTO>>();

                return new List<Relatorio202DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 202: {ex.Message}");
                return new List<Relatorio202DTO>();
            }
        }

        // Relatório 203 (Auditoria de movimentação de estoque)
        public async Task<List<Relatorio203DTO>?> GetDadosRelatorio203Async(RelatorioFiltro203DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/203/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio203DTO>>();

                return new List<Relatorio203DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 203: {ex.Message}");
                return new List<Relatorio203DTO>();
            }
        }












        // Relatório 301 (Movimentações)
        public async Task<List<MovimentoOutputDTO>> GetMovimentacoesFiltradasAsync(RelatorioFiltroMovimentacaoDTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/movimentacoes", filtro);
            return await response.Content.ReadFromJsonAsync<List<MovimentoOutputDTO>>() ?? new();
        }
        #endregion

        #region FAVORITOS
        public async Task<bool> ToggleFavoritoAsync(RelatorioFavoritoDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/favoritar", dto);
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        #endregion

        #region EXPORTAÇÃO (PDF)
        public async Task DownloadPdfAsync(int codigo, string nomeRelatorio, object filtro)
        {
            // Descobre a rota certa baseada no Código do Relatório
            string rotaApi = codigo switch
            {
                101 => "api/relatorio/101/exportar/pdf",
                102 => "api/relatorio/102/exportar/pdf",
                103 => "api/relatorio/103/exportar/pdf",
                104 => "api/relatorio/104/exportar/pdf",
                105 => "api/relatorio/105/exportar/pdf",
                106 => "api/relatorio/106/exportar/pdf",
                107 => "api/relatorio/107/exportar/pdf",
                201 => "api/relatorio/201/exportar/pdf",
                202 => "api/relatorio/202/exportar/pdf",
                203 => "api/relatorio/203/exportar/pdf",
                _ => $"api/relatorio/exportar/pdf"
            };

            var response = await _http.PostAsJsonAsync(rotaApi, filtro);

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = $"{nomeRelatorio.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf";

                // Chama a função JS para baixar o arquivo
                await _js.InvokeVoidAsync("downloadFileFromBytes", fileName, fileBytes);
            }
            else
            {
                Console.WriteLine($"Erro ao gerar PDF: {response.StatusCode}");
            }
        }
        #endregion

        #region MÉTODOS PARA POPULAR OS FILTROS
        public async Task<List<CategoriaOutputDTO>> GetCategoriasAsync()
        {
            var response = await _http.GetAsync("api/categoria");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoriaOutputDTO>>() ?? new();
            }
            return new List<CategoriaOutputDTO>();
        }

        public async Task<List<MarcaOutputDTO>> GetMarcasAsync()
        {
            var response = await _http.GetAsync("api/marca");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MarcaOutputDTO>>() ?? new();
            }
            return new List<MarcaOutputDTO>();
        }

        public async Task<List<FamiliaOutputDTO>?> GetFamiliasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<FamiliaOutputDTO>>("api/Familia");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar famílias: {ex.Message}");
                return new List<FamiliaOutputDTO>();
            }
        }
        #endregion
    }
}