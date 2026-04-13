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

        // 1. Criada uma lista mestre privada para não duplicar código
        private List<RelatorioDefinicaoDTO> GetListaMestre()
        {
            return new List<RelatorioDefinicaoDTO>
            {
                // MÓDULO 100 - PRODUTOS
                new RelatorioDefinicaoDTO { Codigo = 101, Nome = "Produtos Categorizados", Categoria = "#100 - Produtos", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 102, Nome = "PRECIFICAÇÃO E MARGENS DE LUCRO", Categoria = "#100 - Produtos", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 103, Nome = "Movimento de Estoque por Produto", Categoria = "#100 - Produtos", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 104, Nome = "Curva ABC de Produtos", Categoria = "#100 - Produtos", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 105, Nome = "Produtos Sem Giro", Categoria = "#100 - Produtos", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 106, Nome = "Ranking de Vendas (Mais/Menos)", Categoria = "#100 - Produtos", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 107, Nome = "Histórico de Alteração de Preços", Categoria = "#100 - Produtos", IsFavorito = false },
                
                // MÓDULO 200 - ESTOQUE
                new RelatorioDefinicaoDTO { Codigo = 201, Nome = "Posição Atual de Estoque", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 202, Nome = "Lotes e Validades", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 203, Nome = "Movimentação de Estoque Geral", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 204, Nome = "Sugestão de Compras e Cobertura", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 205, Nome = "Ficha de Inventário (Contagem)", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 206, Nome = "Valorização de Estoque (Projeção)", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 207, Nome = "Giro e Velocidade de Estoque", Categoria = "#200 - Estoque", IsFavorito = false },
                new RelatorioDefinicaoDTO { Codigo = 208, Nome = "Divergência de Inventário", Categoria = "#200 - Estoque", IsFavorito = false },

                // MÓDULO 300 - MOVIMENTAÇÕES
                new RelatorioDefinicaoDTO { Codigo = 301, Nome = "Histórico Analítico Geral", Categoria = "#300 - Movimentações", IsFavorito = false }
            };
        }

        // 2. Método ajustado para receber o ID do Usuário e buscar os favoritos no banco
        public async Task<List<RelatorioDefinicaoDTO>> GetMenuRelatoriosAsync(int usuarioId)
        {
            var todosRelatorios = GetListaMestre();

            if (usuarioId > 0)
            {
                try
                {
                    // Consulta a API para pegar apenas os códigos que este usuário favoritou
                    var favoritosDoBanco = await _http.GetFromJsonAsync<List<int>>($"api/Relatorio/meus-favoritos/{usuarioId}");

                    if (favoritosDoBanco != null && favoritosDoBanco.Any())
                    {
                        foreach (var rel in todosRelatorios)
                        {
                            // Cruza as listas: se o código estiver no banco, marca a estrela
                            rel.IsFavorito = favoritosDoBanco.Contains(rel.Codigo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao carregar favoritos do usuário: " + ex.Message);
                }
            }

            return todosRelatorios;
        }

        // 3. Adicionado o parâmetro opcional usuarioId para não quebrar chamadas antigas
        public async Task<RelatorioDefinicaoDTO?> GetDefinicaoAsync(int codigo, int usuarioId = 0)
        {
            var menu = await GetMenuRelatoriosAsync(usuarioId);
            return menu.FirstOrDefault(x => x.Codigo == codigo);
        }
        #endregion

        #region BUSCAS E DADOS DOS RELATÓRIOS

        #region Relatório 101 (Produtos)
        public async Task<List<ProdutoDTO>> GetDadosRelatorio101Async(RelatorioFiltroProdutosDTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/101/dados", filtro);
            return await response.Content.ReadFromJsonAsync<List<ProdutoDTO>>() ?? new();
        }
        #endregion

        #region Relatorio 102 (Produto por valor)
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
        #endregion

        #region Relatório 103 (Movimento por Produto)
        public async Task<List<Relatorio103DTO>> GetDadosRelatorio103Async(RelatorioFiltro103DTO filtro)
        {
            var response = await _http.PostAsJsonAsync("api/relatorio/103/dados", filtro);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Relatorio103DTO>>() ?? new();
            }
            return new List<Relatorio103DTO>();
        }
        #endregion

        #region Relatório 104 (Curva ABC)
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
        #endregion

        #region Relatório 105 (Produtos sem giro)
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
        #endregion

        #region Relatório 106 (Ranking de vendas)
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
        #endregion

        #region Relatório 107 (Histórico de Alteração de Preços)
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
        #endregion

        #region Relatório 201 (Posição atual de estoque)
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
        #endregion

        #region Relatório 202 (Lotes e validades)
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
        #endregion

        #region Relatório 203 (Auditoria de movimentação de estoque)
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
        #endregion

        #region Relatório 204 (Cobertura e Sugestão de compras)
        public async Task<List<Relatorio204DTO>?> GetDadosRelatorio204Async(RelatorioFiltro204DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/204/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio204DTO>>();

                return new List<Relatorio204DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 204: {ex.Message}");
                return new List<Relatorio204DTO>();
            }
        }
        #endregion

        #region Relatório 205 (Produtos abaixo do estoque mínimo)
        public async Task<List<Relatorio205DTO>?> GetDadosRelatorio205Async(RelatorioFiltro205DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/205/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio205DTO>>();
                return new List<Relatorio205DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 205: {ex.Message}");
                return new List<Relatorio205DTO>();
            }
        }
        #endregion

        #region RELATÓRIO 206 - VALORIZAÇÃO DE ESTOQUE
        public async Task<List<Relatorio206DTO>?> GetDadosRelatorio206Async(RelatorioFiltro206DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/206/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio206DTO>>();
                return new List<Relatorio206DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 206: {ex.Message}");
                return new List<Relatorio206DTO>();
            }
        }
        #endregion

        #region RELATÓRIO 207 - GIRO E VELOCIDADE DE ESTOQUE
        public async Task<List<Relatorio207DTO>?> GetDadosRelatorio207Async(RelatorioFiltro207DTO filtro) { var r = await _http.PostAsJsonAsync("api/relatorio/207/dados", filtro); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<List<Relatorio207DTO>>() : new(); }
        #endregion

        #region RELATÓRIO 208 - DIVERGÊNCIA DE INVENTÁRIO
        public async Task<List<Relatorio208DTO>?> GetDadosRelatorio208Async(RelatorioFiltro208DTO filtro) { var r = await _http.PostAsJsonAsync("api/relatorio/208/dados", filtro); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<List<Relatorio208DTO>>() : new(); }

        #endregion

        #region MÓDULO 301 - MOVIMENTAÇÕES

        public async Task<List<Relatorio301DTO>?> GetDadosRelatorio301Async(RelatorioFiltro301DTO filtro)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/relatorio/301/dados", filtro);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<Relatorio301DTO>>();

                return new List<Relatorio301DTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatório 301: {ex.Message}");
                return new List<Relatorio301DTO>();
            }
        }

        #endregion

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
                204 => "api/relatorio/204/exportar/pdf",
                205 => "api/relatorio/205/exportar/pdf",
                206 => "api/relatorio/206/exportar/pdf",
                207 => "api/relatorio/207/exportar/pdf",
                208 => "api/relatorio/208/exportar/pdf",
                301 => "api/relatorio/301/exportar/pdf",
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

        public async Task<List<TipoMovimentoOutputDTO>> GetTiposMovimentoAsync()
        {
            try
            {
                // Consome a rota padrão da sua API para buscar os Tipos
                var response = await _http.GetAsync("api/TipoMovimento");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TipoMovimentoOutputDTO>>() ?? new();
                }
                return new List<TipoMovimentoOutputDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar tipos de movimento: {ex.Message}");
                return new List<TipoMovimentoOutputDTO>();
            }
        }

        public async Task<List<PessoaOutputDTO>> GetPessoasAsync()
        {
            try
            {
                // Consome a rota padrão da sua API para buscar as Pessoas/Usuários
                var response = await _http.GetAsync("api/Pessoa");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PessoaOutputDTO>>() ?? new();
                }
                return new List<PessoaOutputDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar pessoas: {ex.Message}");
                return new List<PessoaOutputDTO>();
            }
        }
        #endregion
    }
}