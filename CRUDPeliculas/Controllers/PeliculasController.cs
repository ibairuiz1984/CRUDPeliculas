using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRUDPeliculas;
using CRUDPeliculas.Entidades;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CRUDPeliculas.Servicios;
using Microsoft.AspNetCore.Authorization;

namespace CRUDPeliculas.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PeliculasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Peliculas
        public async Task<IActionResult> Index(string genero, string nombre, string director, string ordenarPor, bool? conComentarios)
        {
            var peliculas = _context.Peliculas.AsQueryable(); // Obtener todas las películas

            // Filtrar por género si se ha seleccionado uno
            if (!string.IsNullOrEmpty(genero))
            {
                peliculas = peliculas.Where(p => p.Genero == genero);
            }

            // Filtrar por nombre si se ha proporcionado uno
            if (!string.IsNullOrEmpty(nombre))
            {
                peliculas = peliculas.Where(p => p.Titulo.Contains(nombre.Trim()));
            }

            // Filtrar por director si se ha proporcionado uno
            if (!string.IsNullOrEmpty(director))
            {
                peliculas = peliculas.Where(p => p.Director.Contains(director.Trim()));
            }

            // Filtrar películas con comentarios
            if (conComentarios.HasValue && conComentarios.Value)
            {
                peliculas = peliculas.Where(p => p.Comentarios.Any());
            }

            // Ordenar las películas según la opción seleccionada
            var peliculasList = peliculas.ToList(); // Materializar la lista
            switch (ordenarPor)
            {
                case "Titulo":

                    peliculasList = peliculasList.OrderBy(p => LimpiarTitulo(p.Titulo)).ToList();
                    return View(peliculasList);

                case "Anio":
                    peliculas = peliculas.OrderBy(p => p.Anio);
                    break;
                case "Genero":
                    peliculas = peliculas.OrderBy(p => p.Genero);
                    break;
                default:
                    peliculasList = peliculasList.OrderBy(p => LimpiarTitulo(p.Titulo)).ToList();
                    return View(peliculasList);

            }

            return View(await peliculas.ToListAsync());
        }
        private string LimpiarTitulo(string titulo)
        {
            // Remover los símbolos ¿ y ¡ de la cadena de título
            titulo = titulo.Replace("¿", "").Replace("¡", "");
            // Remover los artículos "el" y "la" de la cadena de título
            if (titulo.StartsWith("El "))
            {
                titulo = titulo.Substring(3);
            }
            else if (titulo.StartsWith("La "))
            {
                titulo = titulo.Substring(3);
            }
            // También puedes aplicar otras operaciones de limpieza según sea necesario
            return titulo;
        }

        // GET: Peliculas/Details/5
        public IActionResult Details(int id)
        {
            var pelicula = _context.Peliculas
                .Include(p => p.Comentarios) // Cargar comentarios junto con la película
                .FirstOrDefault(p => p.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarComentario(int peliculaId, string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                ModelState.AddModelError("contenido", "El comentario no puede estar vacío.");
                return RedirectToAction("Details", new { id = peliculaId });
            }

            // Obtener el correo electrónico del usuario actual
            var userEmail = User.Identity.Name;

            // Obtener el nombre del usuario utilizando el correo electrónico
            var userName = userEmail; // Puedes personalizar cómo se muestra el nombre de usuario si lo deseas

            // Si prefieres obtener el nombre de usuario de la base de datos, puedes hacerlo de esta manera:
            var userFromDb = await _context.Users
                .Where(u => u.Email == userEmail)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            // Utiliza el nombre de usuario obtenido de la base de datos si está disponible
            if (!string.IsNullOrEmpty(userFromDb))
            {
                userName = userFromDb;
            }

            // Crear el comentario con el nombre del usuario obtenido
            var comentario = new Comentario
            {
                Contenido = contenido,
                UsuarioNombre = userName, // Asociar el nombre de usuario
                PeliculaId = peliculaId
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = peliculaId });
        }

        // GET: Peliculas/Create
        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)] // Solo usuarios con rol de administrador pueden acceder
        public IActionResult Create()
        {
            return View();
        }

        // POST: Peliculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Director,Anio,Sinopsis,ImagenUrl,Genero")] Pelicula pelicula)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(pelicula);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                // Manejar errores de la base de datos, como claves duplicadas, aquí
                ModelState.AddModelError("", "No se pudo guardar la película. Inténtelo de nuevo más tarde.");
                // Log the exception
            }
            // Si llegamos aquí, algo ha fallado, volver a la vista con los datos del modelo
            return View(pelicula);
        }


        // GET: Peliculas/Edit/5
        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)] // Solo usuarios con rol de administrador pueden acceder
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            return View(pelicula);
        }

        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.RolAdmin)] // Solo usuarios con rol de administrador pueden acceder
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Director,Anio,Genero,Sinopsis,ImagenUrl")] Pelicula pelicula)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pelicula);
        }

        // GET: Peliculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // POST: Peliculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula != null)
            {
                _context.Peliculas.Remove(pelicula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(e => e.Id == id);
        }

    }
}
