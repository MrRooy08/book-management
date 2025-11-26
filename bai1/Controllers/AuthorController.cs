using bai1.Models;
using bai1.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace bai1.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public AuthorController(ApplicationDbContext context) {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search(string name) {

            var authors = _context.Persons.Where(
                    p => p.Name.Contains(name)
            ).ToList();

            if (authors != null) { 
                return Json ( new
                {
                    isSuccess = true,
                    messsage = "Success",
                    data = new { authors}   
                });
            }

            return Json(new
            {
                isSuccess = false,
                message = "Cannot find any authors in your request",
                data = new { authors}
            });
        }


        public IActionResult CreateAuthors(BookAuthorsViewModels models)
        {

            //var authors = _context.Persons.Where(
            //        p => p.Name == name
            //).FirstOrDefault();

            if (models != null)
            {
                return Json(new
                {
                    isSuccess = true,
                    messsage = "Success",
                    data = new { 
                        Id = models.Id,
                        name = models.Title,
                    }
                });
            }

            return Json(new
            {
                isSuccess = false,
                message = "Cannot find any authors in your request"
            });
        }
    }
}
