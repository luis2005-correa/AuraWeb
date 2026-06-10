using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

public partial class Pedido
{
    [Key]
    [Column("PedidoID")]
    public int PedidoId { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaPedido { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Estado { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string DireccionEnvio { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MetodoPago { get; set; }

    [InverseProperty("Pedido")]
    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();

    [ForeignKey("UsuarioId")]
    [InverseProperty("Pedidos")]
    public virtual Usuario Usuario { get; set; } = null!;
}
