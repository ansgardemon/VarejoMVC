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
        public DbSet<Pessoa> Pessosas { get; set; }

        /*
        metodo opcional deve ser usado para configurar o modelo
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().Property(u => u.Ativo).HasDefaultValue(true);
        }
        */
    }
}
