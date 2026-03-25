namespace VarejoAPI.DTO
{
    public class MovimentoInputDTO
    {

        public int? Documento { get; set; }

        public string Observacao { get; set; }

        public DateTime? DataMovimento { get; set; }

        public int PessoaId { get; set; }

        public int TipoMovimentoId { get; set; }

        public List<ProdutoMovimentoInputDTO> Produtos { get; set; }

    }
}
