using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Aura.Models;

public partial class TiendaRopaDbContext : DbContext
{
    public TiendaRopaDbContext()
    {
    }

    public TiendaRopaDbContext(DbContextOptions<TiendaRopaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<DetallesPedido> DetallesPedidos { get; set; }

    public virtual DbSet<ImagenesProducto> ImagenesProductos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VariantesProducto> VariantesProductos { get; set; }

    public virtual DbSet<CatalogImage> CatalogImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C5F92839EF");
        });
     
    
        // 🚀 INDICAMOS EL ESQUEMA CORRECTO DE TU SUPABASE:
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<Categoria>(entity =>
        {
            // Forzamos a que busque la tabla en minúsculas como está en PostgreSQL
            entity.ToTable("categorias");
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C5F92839EF");
        });

        modelBuilder.Entity<DetallesPedido>(entity =>
        {
            entity.ToTable("detallespedidos");
            entity.HasKey(e => e.DetalleId).HasName("PK__Detalles__6E19D6FA4075F2C3");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallesPedidos).HasConstraintName("FK_Detalles_Pedidos");
            entity.HasOne(d => d.Variante).WithMany(p => p.DetallesPedidos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Detalles_Variantes");
        });

        modelBuilder.Entity<ImagenesProducto>(entity =>
        {
            entity.ToTable("imagenesproducto");
            entity.HasKey(e => e.ImagenId).HasName("PK__Imagenes__0C7D20D786CE4664");

            entity.Property(e => e.EsPrincipal).HasDefaultValue(false);
            entity.HasOne(d => d.Producto).WithMany(p => p.ImagenesProductos).HasConstraintName("FK_Imagenes_Productos");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("pedidos");
            entity.HasKey(e => e.PedidoId).HasName("PK__Pedidos__09BA1410833E652C");

            entity.Property(e => e.Estado).HasDefaultValue("Pendiente");

            // 💡 Cambiamos (getdate()) de SQL Server por la función nativa de PostgreSQL
            entity.Property(e => e.FechaPedido).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Pedidos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Usuarios");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("productos");
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE83F05C551B");

            // 💡 Cambiamos (getdate()) de SQL Server por la función nativa de PostgreSQL
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Categorias");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302D1B35F2F7F");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798E3925673");

            // 💡 Cambiamos (getdate()) de SQL Server por la función nativa de PostgreSQL
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<VariantesProducto>(entity =>
        {
            entity.ToTable("variantesproducto");
            entity.HasKey(e => e.VarianteId).HasName("PK__Variante__FE18F96B38FB7B30");

            entity.HasOne(d => d.Producto).WithMany(p => p.VariantesProductos).HasConstraintName("FK_Variantes_Productos");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
