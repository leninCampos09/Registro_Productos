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
        public DbSet<Models.Proveedor> Proveedores { get; set; }
        public DbSet<Models.Venta> Ventas { get; set; }
        public DbSet<Models.ReportVenta> InformesVentas { get; set; }
        public DbSet<Models.ReportCompra> InformesCompras { get; set; }

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

            // Informes de ventas (cache/definiciones)
            modelBuilder.Entity<Models.ReportVenta>(b =>
            {
                b.ToTable("InformesVentas");
                b.HasKey(r => r.Id);
                b.Property(r => r.FechaGeneracion).HasColumnType("datetime2").IsRequired();
                b.Property(r => r.PeriodoInicio).HasColumnType("datetime2");
                b.Property(r => r.PeriodoFin).HasColumnType("datetime2");
                b.Property(r => r.TotalVentas).HasColumnType("decimal(18,2)");
                b.Property(r => r.TotalItems).HasColumnType("int");
                b.Property(r => r.ReportData).HasColumnType("nvarchar(max)");
            });

            // Informes de compras (cache/definiciones)
            modelBuilder.Entity<Models.ReportCompra>(b =>
            {
                b.ToTable("InformesCompras");
                b.HasKey(r => r.Id);
                b.Property(r => r.FechaGeneracion).HasColumnType("datetime2").IsRequired();
                b.Property(r => r.PeriodoInicio).HasColumnType("datetime2");
                b.Property(r => r.PeriodoFin).HasColumnType("datetime2");
                b.Property(r => r.TotalCompras).HasColumnType("decimal(18,2)");
                b.Property(r => r.TotalItems).HasColumnType("int");
                b.Property(r => r.ReportData).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<Models.Categoria>(b =>
            {
                b.ToTable("Categorias");
                b.HasKey(c => c.idCategoria);
                b.Property(c => c.nombre).HasMaxLength(200).IsRequired();
            });

            // Proveedores
            modelBuilder.Entity<Models.Proveedor>(b =>
            {
                b.ToTable("Proveedores");
                b.HasKey(p => p.idProveedor);
                b.Property(p => p.nombre).HasMaxLength(250).IsRequired();
                b.Property(p => p.telefono).HasMaxLength(50);
                b.Property(p => p.email).HasMaxLength(200);
                b.Property(p => p.direccion).HasMaxLength(500);
            });

            // Ventas
            modelBuilder.Entity<Models.Venta>(b =>
            {
                b.ToTable("Ventas");
                b.HasKey(v => v.idVenta);
                b.Property(v => v.fecha).HasColumnType("datetime2").IsRequired();
                b.Property(v => v.cantidad).IsRequired();
                b.Property(v => v.total).HasColumnType("decimal(18,2)").IsRequired();
                b.Property(v => v.clienteNombre).HasMaxLength(250);
                b.Property(v => v.clienteTelefono).HasMaxLength(50);
                b.Property(v => v.clienteEmail).HasMaxLength(200);
                b.HasOne(v => v.Producto)
                 .WithMany()
                 .HasForeignKey(v => v.productoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
