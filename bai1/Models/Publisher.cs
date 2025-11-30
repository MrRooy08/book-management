using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(130)")]
        [Required]
        public string Name { get; set; }


        [Column(TypeName = "nvarchar(max)")]
        public string? Address { get; set; }

        [MinLength(10)]
        [MaxLength(10)]
        public string? Phone { get; set; }

        public string? Email { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Slug { get; set; }

        //Refs
        public ICollection<Book> Books { get; set; } = default!;

        //public IEnumerable<>
           //public IQueryable<>
    }
}
