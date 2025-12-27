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

        [Column(TypeName = "nvarchar(20)")]
        public string? Phone { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? Avatar { get; set; }

        /// <summary>
        /// Trạng thái tài khoản: true = hoạt động, false = bị khóa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Khách hàng VIP: được giảm giá, ưu tiên hỗ trợ
        /// </summary>
        public bool IsVip { get; set; } = false;

        /// <summary>
        /// Ngày trở thành VIP
        /// </summary>
        public DateTime? VipStartDate { get; set; }

        /// <summary>
        /// Ngày hết hạn VIP (null = vĩnh viễn)
        /// </summary>
        public DateTime? VipEndDate { get; set; }

        /// <summary>
        /// Tổng số tiền đã chi tiêu
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSpent { get; set; } = 0;

        /// <summary>
        /// Tổng số đơn hàng đã đặt
        /// </summary>
        public int TotalOrders { get; set; } = 0;

        /// <summary>
        /// Ngày tạo tài khoản
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật gần nhất
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Lần đăng nhập gần nhất
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Ghi chú của admin về user này
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? AdminNote { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();

        // Navigation properties
        public ICollection<Order>? Orders { get; set; }
    }
}
