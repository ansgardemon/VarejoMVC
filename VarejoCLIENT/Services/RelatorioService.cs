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
            var response = await _http.PostAsJsonAsync("api/relatorio/101/dados", filtro);
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
            // Descobre a rota certa baseada no Código do Relatório
            string rotaApi = codigo switch
            {
                101 => "api/relatorio/101/exportar/pdf",
                // Quando criarmos o de movimentações será: 301 => "api/relatorio/301/exportar/pdf"
                _ => $"api/relatorio/exportar/pdf" // fallback provisório
            };

            var response = await _http.PostAsJsonAsync(rotaApi, filtro);

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = $"{nomeRelatorio.Replace(" ", "_")}_{DateTime.Now:yy_MM_dd_HH-mm}.pdf";

                // Chama a função JS para baixar o arquivo
                await _js.InvokeVoidAsync("downloadFileFromBytes", fileName, fileBytes);
            }
            else
            {
                Console.WriteLine($"Erro ao gerar PDF: {response.StatusCode}");
            }
        }

        //#101
        public async Task<List<Relatorio101DTO>> GetDadosRelatorio101Async(RelatorioFiltro101DTO filtro)
        {
            // Fazemos um POST enviando o filtro no corpo da requisição
            var response = await _http.PostAsJsonAsync("api/relatorio/101/dados", filtro);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Relatorio101DTO>>() ?? new List<Relatorio101DTO>();
            }

            return new List<Relatorio101DTO>(); // Retorna vazio em caso de erro para não quebrar a tela
        }
    }
}