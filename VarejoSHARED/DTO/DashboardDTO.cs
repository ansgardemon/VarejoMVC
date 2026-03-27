using VarejoAPI.DTO;

namespace VarejoSHARED.DTO
{
    public class DashboardDTO
    {
        // 1. Kpis Gerais (Topo da Tela)
        public int TotalProdutos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalFamilias { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalClientes { get; set; } // NOVO: Baseado em PessoaOutputDTO (EhCliente = true)
        public int TotalMovimentos { get; set; }
        public int TotalProdutosMovimentados { get; set; }

        // 2. Alertas Críticos (Cards Vermelhos/Laranjas)
        public int ProdutosVencendo { get; set; } // NOVO: Contagem de Validade < 30 dias
        public int ProdutosSemEstoque { get; set; } // NOVO: Contagem EstoqueAtual == 0

        // 3. Listas para as Tabelas
        public List<ProdutoOutputDTO> UltimosProdutos { get; set; } = new();
        public List<CategoriaOutputDTO> ProdutosPorCategoria { get; set; } = new();
        public List<MovimentoOutputDTO> UltimosMovimentos { get; set; } = new();

        // NOVO: Lista para a tabela de atenção
        public List<ValidadeOutputDTO> ValidadesProximas { get; set; } = new();
    }
}