using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Name { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }

        [ValidBirthDate]
        [Required]
        [Column(TypeName = "date")]
        public DateTime BirthDay { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Password { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}
