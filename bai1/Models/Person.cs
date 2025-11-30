
using System.ComponentModel.DataAnnotations;

namespace bai1.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public  string Name { get; set; } = default!;
        public string? Description { get; set; } = string.Empty;
        //Ref
        public ICollection<BookAuthors> BookAuthors { get; set; } = default!;
        public ICollection<BookTranslators> BookTranslators { get; set; } = default!;
    }
}
