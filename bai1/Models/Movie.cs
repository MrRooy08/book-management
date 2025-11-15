using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Column("Title", TypeName ="nvarchar(100)")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Rate { get; set; }
    }
}
