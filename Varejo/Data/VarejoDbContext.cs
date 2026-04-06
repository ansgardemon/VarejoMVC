using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Varejo.Models;
using Varejo.ViewModels;

namespace Varejo.Data
{
    public class VarejoDbContext : DbContext
    {
        //construtor
        public VarejoDbContext(DbContextOptions<VarejoDbContext> options) : base(options)
        {
        }

        //DbSets
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<TipoUsuario> TiposUsuario { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<TipoEmbalagem> TiposEmbalagem { get; set; }
        public DbSet<ProdutoEmbalagem> ProdutosEmbalagem { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<FornecedorFamilia> FornecedoresFamilia { get; set; }
        public DbSet<EspecieMovimento> EspeciesMovimento { get; set; }
        public DbSet<TipoMovimento> TiposMovimento { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<ProdutoMovimento> ProdutosMovimento { get; set; }
        public DbSet<Validade> Validades { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<TituloFinanceiro> TitulosFinanceiro { get; set; }
        public DbSet<FormaPagamento> FormasPagamento { get; set; }
        public DbSet<PrazoPagamento> PrazosPagamento { get; set; }
        public DbSet<EspecieTitulo> EspeciesTitulo { get; set; }
        public DbSet<PagamentoTitulo> PagamentosTitulo { get; set; }
        public DbSet<EstoqueConfig> EstoquesConfig { get; set; }
        public DbSet<EstoqueSnapshot> EstoquesSnapshot { get; set; }
        public DbSet<HistoricoProduto> HistoricosProduto { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<InventarioItem> InventariosItem { get; set; }
        public DbSet<HistoricoPreco> HistoricosPreco { get; set; }

        public DbSet<UsuarioRelatorioFavorito> UsuarioRelatoriosFavoritos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<VendaItem> VendasItem { get; set; }

        /*
        metodo opcional deve ser usado para configurar o modelo
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().Property(u => u.Ativo).HasDefaultValue(true);
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProdutoEmbalagem>()
                .HasIndex(p => p.Ean)
                .IsUnique();

            modelBuilder.Entity<Pessoa>()
                .HasIndex(p => p.CpfCnpj)
                .IsUnique();

            modelBuilder.Entity<ProdutoEmbalagem>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProdutoMovimento>()
                .Property(q => q.Quantidade)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<ProdutoMovimento>()
               .HasOne(pm => pm.Produto)
               .WithMany()
               .HasForeignKey(pm => pm.ProdutoId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoMovimento>()
                .HasOne(pm => pm.ProdutoEmbalagem)
                .WithMany()
                .HasForeignKey(pm => pm.ProdutoEmbalagemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoMovimento>()
                .HasOne(pm => pm.Movimento)
                .WithMany()
                .HasForeignKey(pm => pm.MovimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .Property(p => p.CustoMedio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Produto>()
                .Property(p => p.EstoqueInicial)
                .HasColumnType("decimal(18,4)");            
            
            modelBuilder.Entity<Produto>()
                .Property(p => p.EstoqueAtual)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Produto>()
             .HasIndex(f => f.NomeProduto)
             .IsUnique();

            modelBuilder.Entity<Usuario>()
            .HasIndex(f => f.nomeUsuario)
            .IsUnique();

                   modelBuilder.Entity<Usuario>()
            .HasIndex(f => f.PessoaId)
            .IsUnique();

            modelBuilder.Entity<Familia>()
                .HasIndex(f => f.NomeFamilia)
                .IsUnique();

            modelBuilder.Entity<Marca>()
                .HasIndex(m => m.NomeMarca)
                .IsUnique(); 
            modelBuilder.Entity<Categoria>()
                .HasIndex(m => m.DescricaoCategoria)
                .IsUnique();

            modelBuilder.Entity<TipoEmbalagem>()
    .HasIndex(m => m.DescricaoTipoEmbalagem)
    .IsUnique();

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Familia)
                .WithMany(f => f.Produtos)
                .HasForeignKey(p => p.FamiliaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoEmbalagem>()
                .HasOne(pe => pe.Produto)
                .WithMany(p => p.ProdutosEmbalagem)
                .HasForeignKey(pe => pe.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade); // ao excluir produto, apaga embalagens


            modelBuilder.Entity<ProdutoMovimento>()
                .HasOne(pm => pm.Movimento)
                .WithMany(m => m.ProdutosMovimento)
                .HasForeignKey(pm => pm.MovimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Familia>()
    .HasOne(f => f.Marca)
    .WithMany(m => m.Familias)
    .HasForeignKey(f => f.MarcaId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parametro>()
    .HasOne(p => p.TipoMovimentoVenda)
    .WithMany()
    .HasForeignKey(p => p.TipoMovimentoVendaId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parametro>()
                .HasOne(p => p.TipoMovimentoCompra)
                .WithMany()
                .HasForeignKey(p => p.TipoMovimentoCompraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parametro>()
                .HasOne(p => p.TipoMovimentoAvaria)
                .WithMany()
                .HasForeignKey(p => p.TipoMovimentoAvariaId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TituloFinanceiro>()
    .Property(t => t.Valor)
    .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PagamentoTitulo>()
                .Property(t => t.ValorPago)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TituloFinanceiro>()
                .Property(t => t.ValorAberto)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HistoricoProduto>()
    .HasIndex(h => h.ProdutoId);

            modelBuilder.Entity<HistoricoProduto>()
                .HasIndex(h => h.Data);

            modelBuilder.Entity<HistoricoProduto>()
       .Property(h => h.EstoqueAntes)
       .HasPrecision(18, 4);

            modelBuilder.Entity<HistoricoProduto>()
                .Property(h => h.EstoqueDepois)
                .HasPrecision(18, 4);

            modelBuilder.Entity<HistoricoProduto>()
                .Property(h => h.QuantidadeMovimento)
                .HasPrecision(18, 4);

            modelBuilder.Entity<EstoqueSnapshot>()
                .Property(e => e.Estoque)
                .HasPrecision(18, 4);

            modelBuilder.Entity<EstoqueConfig>()
                .Property(e => e.EstoqueMinimo)
                .HasPrecision(18, 4);

            modelBuilder.Entity<EstoqueConfig>()
                .Property(e => e.EstoqueMaximo)
                .HasPrecision(18, 4);

            modelBuilder.Entity<InventarioItem>()
                .Property(i => i.QuantidadeSistema)
                .HasPrecision(18, 4);

            modelBuilder.Entity<InventarioItem>()
                .Property(i => i.QuantidadeContada)
                .HasPrecision(18, 4);

            modelBuilder.Entity<InventarioItem>()
    .HasOne(i => i.ProdutoEmbalagem)
    .WithMany()
    .HasForeignKey(i => i.ProdutoEmbalagemId)
    .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<InventarioItem>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade); 
            modelBuilder.Entity<UsuarioRelatorioFavorito>()
    .HasIndex(f => new { f.UsuarioId, f.CodigoRelatorio })
    .IsUnique();


            // Configuração para evitar o ciclo de cascata no Histórico
            modelBuilder.Entity<HistoricoProduto>()
                .HasOne(h => h.Movimento)
                .WithMany() // ou .WithMany(m => m.Historicos) se você criou a coleção na Model
                .HasForeignKey(h => h.MovimentoId)
                .OnDelete(DeleteBehavior.Restrict); // O Ponto Chave: Restrict ou NoAction

            modelBuilder.Entity<HistoricoProduto>()
                .HasOne(h => h.TipoMovimento)
                .WithMany()
                .HasForeignKey(h => h.TipoMovimentoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita o segundo caminho de cascata

            modelBuilder.Entity<HistoricoProduto>()
                .HasOne(h => h.EspecieMovimento)
                .WithMany()
                .HasForeignKey(h => h.EspecieMovimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração para evitar o Ciclo de Cascade no Item da Venda
            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Produto)
                .WithMany() // ou .WithMany(p => p.VendaItems) se existir a lista no Model
                .HasForeignKey(vi => vi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict); // <--- A MÁGICA ESTÁ AQUI

            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.ProdutoEmbalagem)
                .WithMany()
                .HasForeignKey(vi => vi.ProdutoEmbalagemId)
                .OnDelete(DeleteBehavior.Restrict); // <--- E AQUI TAMBÉM

            // Se você tiver o mesmo problema entre Venda e Pessoa:
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Pessoa)
                .WithMany()
                .HasForeignKey(v => v.PessoaId)
                .OnDelete(DeleteBehavior.Restrict);



            //PROPRIEDADES OCULTAS PARA AUDITORIA
            modelBuilder.Entity<Pessoa>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Pessoa>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Endereco>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Endereco>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TipoUsuario>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TipoUsuario>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Usuario>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Usuario>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Familia>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Familia>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Categoria>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Categoria>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Marca>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Marca>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TipoEmbalagem>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TipoEmbalagem>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProdutoEmbalagem>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ProdutoEmbalagem>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Produto>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Produto>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<FornecedorFamilia>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FornecedorFamilia>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<EspecieMovimento>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<EspecieMovimento>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TipoMovimento>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TipoMovimento>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Movimento>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Movimento>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProdutoMovimento>()
                .Property<DateTime>("DataCriacao")
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ProdutoMovimento>()
                .Property<DateTime>("DataAtualizacao")
                .HasDefaultValueSql("GETDATE()");


            modelBuilder.Entity<Validade>()
            .Property<DateTime>("DataCriacao")
            .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Validade>()
            .Property<DateTime>("DataAtualizacao")
            .HasDefaultValueSql("GETDATE()");


        }

    }

}

