using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

[Table("VariantesProducto")]
[Index("Sku", Name = "UQ__Variante__CA1FD3C5C7A30CCD", IsUnique = true)]
public partial class VariantesProducto
{
    [Key]
    [Column("varianteID")]
    public int VarianteId { get; set; }

    [Column("productoID")]
    public int ProductoId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    [Column("talla")]
    public string Talla { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    [Column("color")]
    public string Color { get; set; } = null!;


    [Column("stock")]
    public int Stock { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    [Column("sku")]
    public string? Sku { get; set; }

    [InverseProperty("Variante")]
    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();

    [ForeignKey("ProductoId")]
    [InverseProperty("VariantesProductos")]
    public virtual Producto Producto { get; set; } = null!;
}
