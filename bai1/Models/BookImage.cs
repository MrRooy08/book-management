using System.ComponentModel.DataAnnotations;

namespace bai1.Models
{
    public class BookImage
    {
        public int Id { get; set; } // PK của BookImage

        [Required]
        public  string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public bool IsPrimary { get; set; } // Đánh dấu ảnh bìa chính

            // Khóa Ngoại và Thuộc tính Điều hướng
        public int BookId { get; set; } // FK trỏ đến Book
        public Book Book { get; set; } // Thuộc tính điều hướng
    }
}
