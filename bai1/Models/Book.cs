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
        public string? Language { get; set; }
        public DateTime PublishDate { get; set; }
        public int PageCount { get; set; }
        public float Weight { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }


        //Gia goc
        [Required]
        public decimal ListPrice { get; set; }


        // Gia ban
        [Required]
        public  decimal SalePrice { get; set; }

        //Gia von 
        [Required]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Format { get; set; }

        //refs
        public BookDimensions Dimensions { get; set; }

        // Quan hệ 1-1 với Inventory
        public Inventory? Inventory { get; set; }
        
        public ICollection<Category>? Categories { get; set; } = new List<Category>();
        public ICollection<BookImage>? Images { get; set; } = new List<BookImage>();

        public ICollection<BookAuthors> Authors { get; set; }
        public ICollection<BookTranslators> Translators { get; set; }

        
        public Publisher Publisher { get; set; }

        [Required]
        [ForeignKey("Publisher")]
        public int PublisherId { get; set; }
    }
}
