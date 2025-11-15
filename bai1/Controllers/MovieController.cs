using bai1.Models;
using Microsoft.AspNetCore.Mvc;

namespace bai1.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public MovieController (ApplicationDbContext context) { 
            _context = context;    
        }

        public IActionResult Index()
        {
            List<Movie> movies = _context.Movie.ToList();
            return View(movies);
        }

        [HttpPost]
        public IActionResult Add(Movie movie) {

            _context.Movie.Add(movie);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
