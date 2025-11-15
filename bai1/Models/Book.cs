using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Book
    {
        public int Id { get; set; }

        [MinLength(13)]
        [MaxLength(13)]
        [Required]
        public string ISBN { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Title { get; set; }
        public string? Size { get; set; }
        public string? Language { get; set; }
        public DateTime PublishDate { get; set; }
        public int PageCount { get; set; }
        public float Weight { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }

        [Required]
        public decimal ListPrice { get; set; }

        [Required]
        public  decimal SalePrice { get; set; }

        [Required]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Format { get; set; }


        public ICollection<Inventory> Inventories { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<BookImage>? Images { get; set; }

        [Required]
        public  ICollection<Publisher> Publisher { get; set; }
        [Required]
        public  ICollection<BookAuthors> Author { get; set; }
        [Required]
        public ICollection<BookTranslators> Translator { get; set; }


    }
}
