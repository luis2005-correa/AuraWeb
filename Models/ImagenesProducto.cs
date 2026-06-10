using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

[Table("ImagenesProducto")]
public partial class ImagenesProducto
{
    [Key]
    [Column("ImagenID")]
    public int ImagenId { get; set; }

    [Column("ProductoID")]
    public int ProductoId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string UrlImagen { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    [ForeignKey("ProductoId")]
    [InverseProperty("ImagenesProductos")]
    public virtual Producto Producto { get; set; } = null!;
}
