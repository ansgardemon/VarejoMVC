using Microsoft.JSInterop;
using System.Net.Http.Json;
using VarejoSHARED.DTO;

namespace VarejoCLIENT.Services
{
    public class RelatorioService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public RelatorioService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        #region DEFINIÇÕES E MENU (SIDEBAR)

        /// Busca a lista de relatórios disponíveis para montar o menu lateral (pastas e itens
        public async Task<List<RelatorioDefinicaoDTO>> GetMenuRelatoriosAsync()
        {
            try
            {
                // No futuro, isso virá do banco: return await _http.GetFromJsonAsync<List<RelatorioDefinicaoDTO>>("api/relatorio/menu");
                return await Task.FromResult(new List<RelatorioDefinicaoDTO>
                {
                    new RelatorioDefinicaoDTO { Codigo = 101, Nome = "Posição de Estoque", Categoria = "Estoque", IsFavorito = true },
                    new RelatorioDefinicaoDTO { Codigo = 201, Nome = "Movimentação de Produtos", Categoria = "Movimentos" }
                });
            }
            catch { return new List<RelatorioDefinicaoDTO>(); }
        }

        /// Obtém os detalhes de um relatório específico pelo seu código (101, 201, etc
        public async Task<RelatorioDefinicaoDTO?> GetDefinicaoAsync(int codigo)
        {
            var menu = await GetMenuRelatoriosAsync();
            return menu.FirstOrDefault(x => x.Codigo == codigo);
        }

        #endregion

        #region EXECUÇÃO DE RELATÓRIOS (BUSCA)

        /// Executa a busca do Relatório 101 - Posição de Estoque
        public async Task<List<ProdutoDTO>> GetProdutosFiltradosAsync(RelatorioFiltroProdutosDTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/produtos", filtro);
                return await response.Content.ReadFromJsonAsync<List<ProdutoDTO>>() ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
                return new List<ProdutoDTO>();
            }
        }

        /// Executa a busca do Relatório 201 - Movimentações
        public async Task<List<MovimentoItemDTO>> GetMovimentacoesFiltradasAsync(RelatorioFiltroMovimentacaoDTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/movimentacoes", filtro);
                return await response.Content.ReadFromJsonAsync<List<MovimentoItemDTO>>() ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar movimentações: {ex.Message}");
                return new List<MovimentoItemDTO>();
            }
        }

        #endregion

        #region EXPORTAÇÃO (DOWNLOAD)



        public async Task DownloadPdfProdutosAsync(RelatorioFiltroProdutosDTO filtro)
        {
            // 1. Faz o POST para o endpoint de exportação da API
            var response = await _http.PostAsJsonAsync("api/relatorio/exportar/pdf", filtro);

            if (response.IsSuccessStatusCode)
            {
                // 2. Lê o arquivo como um Stream
                var fileStream = await response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: fileStream);

                // 3. Chama a função JS que criamos no index.html
                await _js.InvokeVoidAsync("downloadFileFromStream", "RelatorioProdutos.pdf", streamRef);
            }
        }

        #endregion
    }
}