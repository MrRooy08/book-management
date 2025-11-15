using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName ="nvarchar(100)")]
        public string RoleName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string RoleDescription { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
