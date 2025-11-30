using System.Collections.Generic;
using bai1.Models;

namespace bai1.Models.Dto
{
 public class HomeIndexViewModel
 {
 public IEnumerable<Book> Books { get; set; } = new List<Book>();
 public int TotalBooks { get; set; }
 public int TotalPages { get; set; }
 public int CurrentPage { get; set; }
 public string? SearchTerm { get; set; }
 }
}
