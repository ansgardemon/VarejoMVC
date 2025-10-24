using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Varejo.Models;
using Varejo.ViewModels;

namespace VarejoAPI.DTO
{
    public class ProdutoOutputDTO
    {

        public int IdProduto { get; set; }

        public string NomeProduto { get; set; }


        public string UrlImagem { get; set; }

        public decimal Preco { get; set; }

    }

}

