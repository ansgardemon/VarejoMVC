using Microsoft.JSInterop;
using System.Net.Http.Json;
using VarejoAPI.DTO;
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

        // --- MENU E DEFINIÇÕES ---
        public async Task<List<RelatorioDefinicaoDTO>> GetMenuRelatoriosAsync()
        {
            // Lista mockada com a sua nova regra de prefixos #100, #200...
            return await Task.FromResult(new List<RelatorioDefinicaoDTO>
            {
                new RelatorioDefinicaoDTO { Codigo = 101, Nome = "Relação Geral de Produtos", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 102, Nome = "Tabela de Preços e Custos", Categoria = "#100 - Produtos" },
                new RelatorioDefinicaoDTO { Codigo = 201, Nome = "Posição Atual de Estoque", Categoria = "#200 - Estoque" },
                new RelatorioDefinicaoDTO { Codigo = 202, Nome = "Lotes e Validades", Categoria = "#200 - Estoque" },
                new RelatorioDefinicaoDTO { Codigo = 301, Nome = "Histórico de Movimentações", Categoria = "#300 - Movimentações" }
            });
        }

        public async Task<RelatorioDefinicaoDTO?> GetDefinicaoAsync(int codigo)
        {
            var menu = await GetMenuRelatoriosAsync();
            return menu.FirstOrDefault(x => x.Codigo == codigo);
        }

        // --- BUSCAS ---

        // Relatório 101 (Produtos)
        public async Task<List<ProdutoDTO>> GetProdutosFiltradosAsync(RelatorioFiltroProdutosDTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/produtos", filtro);
            return await response.Content.ReadFromJsonAsync<List<ProdutoDTO>>() ?? new();
        }

        // Relatório 301 (Movimentações)
        public async Task<List<MovimentoOutputDTO>> GetMovimentacoesFiltradasAsync(RelatorioFiltroMovimentacaoDTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/movimentacoes", filtro);
            return await response.Content.ReadFromJsonAsync<List<MovimentoOutputDTO>>() ?? new();
        }

        // --- FAVORITOS ---
        public async Task<bool> ToggleFavoritoAsync(RelatorioFavoritoDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/favoritar", dto);
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // --- EXPORTAÇÃO ---
        public async Task DownloadPdfAsync(int codigo, string nomeRelatorio, object filtro)
        {
            var url = codigo == 101 ? "api/relatorio/exportar/pdf" : $"api/relatorio/exportar/pdf/{codigo}";
            var response = await _http.PostAsJsonAsync(url, filtro);

            if (response.IsSuccessStatusCode)
            {
                // Limpa o nome para não ter espaços ou caracteres estranhos no arquivo
                string nomeArquivo = $"Relatorio_{codigo}_{nomeRelatorio.Replace(" ", "_")}.pdf";

                var fileStream = await response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: fileStream);

                // Passa o nome dinâmico para o JS
                await _js.InvokeVoidAsync("downloadFileFromStream", nomeArquivo, streamRef);
            }
        }
    }
}