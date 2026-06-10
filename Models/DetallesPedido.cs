using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

[Table("DetallesPedido")]
public partial class DetallesPedido
{
    [Key]
    [Column("DetalleID")]
    public int DetalleId { get; set; }

    [Column("PedidoID")]
    public int PedidoId { get; set; }

    [Column("VarianteID")]
    public int VarianteId { get; set; }

    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PrecioUnitario { get; set; }

    [ForeignKey("PedidoId")]
    [InverseProperty("DetallesPedidos")]
    public virtual Pedido Pedido { get; set; } = null!;

    [ForeignKey("VarianteId")]
    [InverseProperty("DetallesPedidos")]
    public virtual VariantesProducto Variante { get; set; } = null!;
}
