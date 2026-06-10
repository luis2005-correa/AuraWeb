using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

// 1. Apuntamos la tabla al esquema auradb y corregimos el nombre del campo en el índice a "Email"
[Table("usuarios", Schema = "auradb")]
[Index(nameof(Email), Name = "UQ__Usuarios__A9D10534E139ABDA", IsUnique = true)]
public partial class Usuario
{
    [Key]
    [Column("usuarioid")]
    public int UsuarioId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    [Column("passwordhash")]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Column("direccion")]
    public string? Direccion { get; set; }

    [Column("rolid")]
    public int RolId { get; set; }

    [Column("fecharegistro")]
    public DateTime? FechaRegistro { get; set; }

    [InverseProperty("Usuario")]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    // 2. Mapeo exacto de la llave foránea y la relación inversa con la clase Role
    [ForeignKey(nameof(RolId))]
    [InverseProperty("Usuarios")]
    public virtual Role Rol { get; set; } = null!;
}