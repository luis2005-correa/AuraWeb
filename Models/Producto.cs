using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

public partial class Producto
{
    [Key]
    [Column("ProductoID")]
    public int ProductoId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Descripcion { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }

    [Column("CategoriaID")]
    public int CategoriaId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("CategoriaId")]
    [InverseProperty("Productos")]
    public virtual Categoria Categoria { get; set; } = null!;

    [InverseProperty("Producto")]
    public virtual ICollection<ImagenesProducto> ImagenesProductos { get; set; } = new List<ImagenesProducto>();

    [InverseProperty("Producto")]
    public virtual ICollection<VariantesProducto> VariantesProductos { get; set; } = new List<VariantesProducto>();
}
