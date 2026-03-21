using System.ComponentModel.DataAnnotations;
using Varejo.Models;

public class Parametro
{
    [Key]
    public int IdParametroMovimento { get; set; }

    [Required]
    public int TipoMovimentoVendaId { get; set; }
    public TipoMovimento TipoMovimentoVenda { get; set; }

    //[Required]
    //public int TipoMovimentoSaidaBonificacaoId { get; set; }
    //public TipoMovimento TipoMovimentoSaidaBonificacao { get; set; }

    [Required]
    public int TipoMovimentoCompraId { get; set; }
    public TipoMovimento TipoMovimentoCompra { get; set; }

    //[Required]
    //public int TipoMovimentoEntradaBonificacaoId { get; set; }
    //public TipoMovimento TipoMovimentoEntradaBonificacao { get; set; }

    [Required]
    public int TipoMovimentoAvariaId { get; set; }
    public TipoMovimento TipoMovimentoAvaria { get; set; }
}