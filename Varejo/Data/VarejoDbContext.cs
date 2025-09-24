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
        public DbSet<FornecedorFamilia> FornecedorFamilias { get; set; }

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

            modelBuilder.Entity<ProdutoEmbalagem>()
          .Property(p => p.Preco)
          .HasColumnType("decimal(18,2)");

        }

    }

}

