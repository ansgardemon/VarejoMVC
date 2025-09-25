using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Varejo.Models;

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
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
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



            modelBuilder.Entity<ProdutoMovimento>()
    .HasOne(pm => pm.Movimento)
    .WithMany(m => m.ProdutosMovimento)
    .HasForeignKey(pm => pm.MovimentoId)
    .OnDelete(DeleteBehavior.Restrict);

        }

    }

}

