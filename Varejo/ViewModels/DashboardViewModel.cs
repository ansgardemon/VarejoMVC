using System;
using System.Collections.Generic;

namespace Varejo.ViewModels
{
    // ViewModel principal da Dashboard
    public class DashboardViewModel
    {
        // =====================
        // CARDS
        // =====================
        public int TotalProdutos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalFamilias { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalMarcas { get; set; }
        public int TotalMovimentos { get; set; }
        public int TotalProdutosMovimentados { get; set; }

        // =====================
        // ÚLTIMOS 5 PRODUTOS ADICIONADOS
        // =====================
        public List<ProdutoItem> UltimosProdutos { get; set; } = new List<ProdutoItem>();

        // =====================
        // PRODUTOS POR CATEGORIA
        // =====================
        public List<CategoriaCount> ProdutosPorCategoria { get; set; } = new List<CategoriaCount>();

        // =====================
        // PRODUTOS POR FAMILIA
        // =====================
        public List<FamiliaCount> ProdutosPorFamilia { get; set; } = new List<FamiliaCount>();

        // =====================
        // ÚLTIMOS USUÁRIOS
        // =====================
        public List<UsuarioItem> UltimosUsuarios { get; set; } = new List<UsuarioItem>();

        // =====================
        // NOVOS: MOVIMENTOS
        // =====================
        public List<MovimentoItem> UltimosMovimentos { get; set; } = new List<MovimentoItem>();
        public List<MovimentosPorTipo> MovimentosPorTipo { get; set; } = new List<MovimentosPorTipo>();
    }

    // =====================
    // PRODUTO
    // =====================
    public class ProdutoItem
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Familia { get; set; } = string.Empty;
        public string UrlImagem { get; set; } = string.Empty;
    }

    // =====================
    // CATEGORIA
    // =====================
    public class CategoriaCount
    {
        public int IdCategoria { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int QtdeProdutos { get; set; }
    }

    // =====================
    // FAMÍLIA
    // =====================
    public class FamiliaCount
    {
        public int IdFamilia { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int QtdeProdutos { get; set; }
    }

    // =====================
    // USUÁRIO
    // =====================
    public class UsuarioItem
    {
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
    }

    // =====================
    // MOVIMENTO
    // =====================
    public class MovimentoItem
    {
        public int IdMovimento { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
        public string Pessoa { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int QtdeProdutos { get; set; }
    }

    public class MovimentosPorTipo
    {
        public int IdTipoMovimento { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
        public int QtdeMovimentos { get; set; }
    }
}
