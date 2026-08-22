using Microsoft.EntityFrameworkCore;

namespace Registro_Productos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Producto> Productos { get; set; }
        public DbSet<Models.Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Producto>(b =>
            {
                b.ToTable("Productos");
                b.HasKey(p => p.idProducto);
                b.Property(p => p.producto).HasMaxLength(500).IsRequired();
                b.Property(p => p.precio).HasColumnType("decimal(18,2)");
                b.Property(p => p.img).HasMaxLength(1000);
                b.Property(p => p.descripcion).HasColumnType("nvarchar(max)");
                b.HasOne(p => p.Categoria)
                 .WithMany(c => c.Productos)
                 .HasForeignKey(p => p.categoriaId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Models.Categoria>(b =>
            {
                b.ToTable("Categorias");
                b.HasKey(c => c.idCategoria);
                b.Property(c => c.nombre).HasMaxLength(200).IsRequired();
            });
        }
    }
}
