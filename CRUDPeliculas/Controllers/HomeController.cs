using CRUDPeliculas.Entidades;
using CRUDPeliculas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace CRUDPeliculas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var peliculas = _context.Peliculas.ToList();
            Random rand = new Random();
            List<Pelicula> peliculasAleatorias = peliculas.OrderBy(x => rand.Next()).Take(3).ToList();
            //var model = RecomendadasParaHoy(); // Llamada al método RecomendadasParaHoy para obtener el modelo
            return View(peliculasAleatorias); // Pasar el modelo a la vista Index
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
