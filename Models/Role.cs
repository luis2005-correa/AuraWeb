using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

// 1. Mapeamos el nombre exacto de la tabla y su esquema personalizado
[Table("roles", Schema = "auradb")]
[Index("Nombre", Name = "UQ__Roles__75E3EFCFB9733A1F", IsUnique = true)]
public partial class Role
{
    [Key]
    // 2. Forzamos minúscula para que coincida con 'rolid' en Supabase
    [Column("rolid")]
    public int RolId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    // 3. Forzamos minúscula para que coincida con 'nombre' en Supabase
    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    // 4. Mapea la relación inversa apuntando a la propiedad virtual "Rol" en Usuario.cs
    [InverseProperty("Rol")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}