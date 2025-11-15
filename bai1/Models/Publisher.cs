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

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Address { get; set; }

        [MinLength(10)]
        [MaxLength(10)]
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Slug { get; set; }
    }
}
