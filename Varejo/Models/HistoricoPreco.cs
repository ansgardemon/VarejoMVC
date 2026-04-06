using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class HistoricoPreco
    {
        [Key]
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }

        public DateTime DataAlteracao { get; set; } = DateTime.Now;

        public decimal PrecoAntigo { get; set; }
        public decimal PrecoNovo { get; set; }

        // Como você ainda não tem a tabela de usuários amarrada, 
        // podemos salvar o nome/email do usuário logado direto como string por enquanto!
        public string Usuario { get; set; } = string.Empty;
    }
}