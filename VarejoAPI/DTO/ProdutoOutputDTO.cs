using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Varejo.Models;
using Varejo.ViewModels;

namespace VarejoAPI.DTO
{
    public class ProdutoOutputDTO
    {

        public int IdProduto { get; set; }

        public string? Complemento { get; set; }

        public string NomeProduto { get; set; }

      
        public decimal EstoqueInicial { get; set; } = 0;

        public bool Ativo { get; set; } = true;

        public string UrlImagem { get; set; }

        public decimal CustoMedio { get; set; }

        public int FamiliaId { get; set; }

        public List<ProdutoEmbalagemOutputDTO>? EmbalagemProd { get; set; }

    }

}

