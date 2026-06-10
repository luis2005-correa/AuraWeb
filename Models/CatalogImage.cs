using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aura.Models;

[Table("Product_Images", Schema = "public")]
public class CatalogImage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("image_name")]
    [StringLength(255)]
    public string ImageName { get; set; } = null!;

    [Column("image_url")]
    public string ImageUrl { get; set; } = null!;

    [Column("image_url_2")]
    public string? ImageUrl2 { get; set; }

    [Column("precio", TypeName = "numeric(10,2)")]
    public decimal Precio { get; set; } = 0;

    [Column("activo")]
    public bool Activo { get; set; } = true;
}
