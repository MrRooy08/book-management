using bai1.Models;
using Microsoft.AspNetCore.Mvc;

namespace bai1.Controllers
{
    public class MovieController : Controller
    {
        //new () { Id = 1, Title = " Fish & Cat",
        //        Description = " A fish and a cat are best friends, ", Rate = 5 },
        //    new () { Id = 2, Title = " Dog & Goat",
        //        Description = " A Dog and a Goat aren't best friends, ", Rate = 3
        //    },
        //    new()
        //    {
        //        Id = 3,
        //        Title = " Fast & Furios",
        //        Description = " Fast & FUrios are the best movie, ",
        //        Rate = 5
        //    },
        public List<Movie> listMovies = new List<Movie>() {
            
        };

        public IActionResult Index()
        {
            return View(listMovies);
        }
    }
}
