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

        // --- MENU E DEFINIÇÕES ---
        public async Task<List<RelatorioDefinicaoDTO>> GetMenuRelatoriosAsync()
        {
            // É daqui que a sua tela de menu puxa os "Cards". 
            // Quando tiver banco de dados para isso, é só trocar esse Task.FromResult por uma chamada _http.GetFromJsonAsync
            return await Task.FromResult(new List<RelatorioDefinicaoDTO>
            {
                // MÓDULO 100 - PRODUTOS
                new RelatorioDefinicaoDTO { Codigo = 101, Nome = "Produtos Categorizados", Categoria = "#100 - Produtos", IsFavorito = true },
                new RelatorioDefinicaoDTO { Codigo = 102, Nome = "Produtos por Valores", Categoria = "#100 - Produtos" },
                new RelatorioDefinicaoDTO { Codigo = 103, Nome = "Movimento de Estoque por Produto", Categoria = "#100 - Produtos" }, // <-- NOSSO NOVO RELATÓRIO AQUI!
                new RelatorioDefinicaoDTO { Codigo = 104, Nome = "Curva ABC de Produtos", Categoria = "#100 - Produtos" },
                new RelatorioDefinicaoDTO { Codigo = 105, Nome = "Produtos Sem Giro", Categoria = "#100 - Produtos" },
                
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

        // --- BUSCAS ---

        // Relatório 101 (Produtos)
        public async Task<List<ProdutoDTO>> GetProdutosFiltradosAsync(RelatorioFiltroProdutosDTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/101/dados", filtro);
            return await response.Content.ReadFromJsonAsync<List<ProdutoDTO>>() ?? new();
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
                103 => "api/relatorio/103/exportar/pdf",
                _ => $"api/relatorio/exportar/pdf"
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

        // --- MÉTODOS PARA POPULAR OS FILTROS ---

        public async Task<List<CategoriaOutputDTO>> GetCategoriasAsync()
        {
            // Verifique se a rota na sua API é esta mesma (ex: api/categorias ou api/relatorio/categorias)
            var response = await _http.GetAsync("api/categoria");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoriaOutputDTO>>() ?? new();
            }
            return new List<CategoriaOutputDTO>();
        }

        public async Task<List<MarcaOutputDTO>> GetMarcasAsync()
        {
            // Verifique se a rota na sua API é esta mesma
            var response = await _http.GetAsync("api/marca");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MarcaOutputDTO>>() ?? new();
            }
            return new List<MarcaOutputDTO>();
        }

        // Adicione este método dentro do seu RelatorioService
        public async Task<List<FamiliaOutputDTO>?> GetFamiliasAsync()
        {
            try
            {
                // Ajuste a rota "api/familias" para a rota correta da sua API Varejo
                return await _http.GetFromJsonAsync<List<FamiliaOutputDTO>>("api/familias");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar famílias: {ex.Message}");
                return new List<FamiliaOutputDTO>();
            }
        }
    }
}