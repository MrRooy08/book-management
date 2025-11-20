using System.ComponentModel.DataAnnotations;

namespace bai1.Models.Dto
{
    public class BookAuthorsViewModels
    {
        public int Id { get; set; }

        public string ISBN { get; set; }
        public string Title { get; set; } 

        public string? Description { get; set; }

        public DateTime PublishDate { get; set; }

        //Gia goc
        public float ListPrice { get; set; }

        //Gia von mua tu ncc
        public float CostPrice { get; set; }

        // Gia ban thuc te 
        public float SalePrice { get; set; }

        public int Inventory { get; set; }

        [Required]
        public string AuthorIds { get; set; }

    }
}
