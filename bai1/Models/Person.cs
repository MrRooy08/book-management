using System.ComponentModel.DataAnnotations;

namespace bai1.Models
{
    public class    Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public  string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
