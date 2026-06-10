using Aura.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Aura.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TiendaRopaDbContext _context;

        public HomeController(ILogger<HomeController> logger, TiendaRopaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var productosHome = _context.CatalogImages
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Id)
                .Take(8)
                .ToList();

            return View(productosHome);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PanelAdmin()
        {
            return Content("Felicidades! Eres Admin y el sistema te reconoció.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
