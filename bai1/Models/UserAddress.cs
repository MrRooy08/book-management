using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Column(TypeName = "nvarchar(100)")]
        public string? ReceiverName { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string? Phone { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Province { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? District { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Ward { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
