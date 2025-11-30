using bai1.Models;

namespace bai1.Models.Dto
{
 public class BookDetailsViewModel
 {
 public Book Book { get; set; } = null!;
 public string? PrimaryImageUrl { get; set; }
 public decimal DiscountPercent { get; set; }
 }
}
