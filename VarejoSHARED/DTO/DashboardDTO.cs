using System;
using System.Collections.Generic;

namespace VarejoSHARED.DTO
{
    public class DashboardDTO
    {
        // =====================================
        // 1. KPIs GERAIS
        // =====================================
        public int TotalProdutos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalFamilias { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalMarcas { get; set; }
        public int TotalMovimentos { get; set; }
        public int TotalProdutosMovimentados { get; set; }

        // =====================================
        // 2. KPIs ESTRATÉGICOS (Alertas do Gestor)
        // =====================================
        public int TotalClientes { get; set; }
        public int ProdutosSemEstoque { get; set; }
        public int ProdutosVencendo { get; set; }

        // =====================================
        // 3. LISTAS PARA TABELAS E GRÁFICOS
        // =====================================
        // Usa: VarejoSHARED.DTO.ProdutoOutputDTO
        public List<ProdutoOutputDTO> UltimosProdutos { get; set; } = new();

        // Usa: VarejoSHARED.DTO.CategoriaCountDTO (Definida abaixo)
        public List<CategoriaCountDTO> ProdutosPorCategoria { get; set; } = new();

        // Usa: VarejoSHARED.DTO.MovimentoItemDTO (Definida abaixo)
        public List<MovimentoItemDTO> UltimosMovimentos { get; set; } = new();

        // Usa: VarejoSHARED.DTO.ValidadeOutputDTO
        public List<ValidadeOutputDTO> ValidadesProximas { get; set; } = new();
    }

    // =====================================
    // CLASSES AUXILIARES EXCLUSIVAS DO DASHBOARD
    // =====================================
    public class CategoriaCountDTO
    {
        public int IdCategoria { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int QtdeProdutos { get; set; }
    }

    public class MovimentoItemDTO
    {
        public int IdMovimento { get; set; }
        public string Pessoa { get; set; } = string.Empty;
        public string TipoMovimento { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int QtdeProdutos { get; set; }
    }
}