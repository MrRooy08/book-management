using System.ComponentModel.DataAnnotations;

namespace bai1.Models
{
    public class Inventory
    {
        [Key]
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public Book Book { get; set; }
    }
}
