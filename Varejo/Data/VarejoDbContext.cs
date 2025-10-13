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



        }

    }

}

