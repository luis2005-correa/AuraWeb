using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Aura.Models;

[Index("Nombre", Name = "UQ__Categori__75E3EFCF9ED487FE", IsUnique = true)]
public partial class Categoria
{
    [Key]
    [Column("categoriaID")]
    public int CategoriaId { get; set; }

    [StringLength(100)]
    [Unicode(false)]

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [InverseProperty("Categoria")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
