using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }
        public string? Slug { get; set; }

        // Cột cho Phân cấp (Tùy chọn)
        // column for hirarchy (Optional)
        public int? ParentId { get; set; } // FK tự tham chiếu

        [ForeignKey("ParentId")]
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }

        // relationship many-many with Book
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
