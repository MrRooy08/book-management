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
        public int? ParentId { get; set; } // FK tự tham chiếu
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? Children { get; set; }

        // Mối quan hệ M:N với Book
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
