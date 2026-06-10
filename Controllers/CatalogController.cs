using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aura.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aura.Controllers;

public class CatalogController : Controller
{
    private readonly TiendaRopaDbContext _context;

    public CatalogController(TiendaRopaDbContext context)
    {
        _context = context;
    }

    private bool IsAdmin()
    {
        var token = HttpContext.Session.GetString("FirebaseToken");
        if (string.IsNullOrEmpty(token)) return false;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c =>
                c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))?.Value
                ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
        }
        catch { return false; }
    }

    public async Task<IActionResult> Index()
    {
        var query = _context.CatalogImages.AsQueryable();
        if (!IsAdmin())
            query = query.Where(i => i.Activo);

        var images = await query
            .OrderByDescending(img => img.Id)
            .ToListAsync();

        ViewBag.IsAdmin = IsAdmin();
        return View(images);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CatalogImage model)
    {
        if (!IsAdmin()) return Forbid();
        if (string.IsNullOrWhiteSpace(model.ImageName) || string.IsNullOrWhiteSpace(model.ImageUrl))
            return BadRequest(new { error = "Nombre y URL son requeridos." });

        model.Activo = true;
        _context.CatalogImages.Add(model);
        await _context.SaveChangesAsync();
        return Ok(new { id = model.Id, message = "Producto creado correctamente." });
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] CatalogImage model)
    {
        if (!IsAdmin()) return Forbid();

        var existing = await _context.CatalogImages.FindAsync(model.Id);
        if (existing == null) return NotFound(new { error = "Producto no encontrado." });

        existing.ImageName = model.ImageName;
        existing.ImageUrl = model.ImageUrl;
        existing.ImageUrl2 = model.ImageUrl2;
        existing.Precio = model.Precio;
        existing.Activo = model.Activo;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Producto actualizado correctamente." });
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] DeleteRequest req)
    {
        if (!IsAdmin()) return Forbid();

        var existing = await _context.CatalogImages.FindAsync(req.Id);
        if (existing == null) return NotFound(new { error = "Producto no encontrado." });

        existing.Activo = false;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Producto eliminado correctamente." });
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }
}
