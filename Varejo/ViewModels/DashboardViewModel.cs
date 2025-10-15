using System;
using System.Collections.Generic;

namespace Varejo.ViewModels
{
    // ViewModel principal da Dashboard
    public class DashboardViewModel
    {
        // CARDS
        public int TotalProdutos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalFamilias { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalMarcas { get; set; }

        // ÚLTIMOS 5 PRODUTOS ADICIONADOS
        public List<ProdutoItem> UltimosProdutos { get; set; } = new List<ProdutoItem>();

        // PRODUTOS POR CATEGORIA
        public List<CategoriaCount> ProdutosPorCategoria { get; set; } = new List<CategoriaCount>();

        // PRODUTOS POR FAMILIA
        public List<FamiliaCount> ProdutosPorFamilia { get; set; } = new List<FamiliaCount>();

        // ÚLTIMOS USUÁRIOS
        public List<UsuarioItem> UltimosUsuarios { get; set; } = new List<UsuarioItem>();
    }

    // Representa um produto na listagem de últimos produtos
    public class ProdutoItem
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Familia { get; set; } = string.Empty;
        public string UrlImagem { get; set; } = string.Empty;
    }

    // Contagem de produtos por categoria
    public class CategoriaCount
    {
        public int IdCategoria { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int QtdeProdutos { get; set; }
    }

    // Contagem de produtos por família
    public class FamiliaCount
    {
        public int IdFamilia { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int QtdeProdutos { get; set; }
    }

    // Representa um usuário na listagem de últimos usuários
    public class UsuarioItem
    {
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
    }
}
