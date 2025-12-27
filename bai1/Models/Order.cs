using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(200)")]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Ph??ng th?c thanh toán: VNPay, COD
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string PaymentMethod { get; set; } = "VNPay";

        /// <summary>
        /// Tr?ng thái thanh toán: Pending, Paid, Failed, Refunded
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column(TypeName = "nvarchar(100)")]
        public string? VnpayTransactionId { get; set; }

        /// <summary>
        /// Tr?ng thái ??n hàng: Pending, Confirmed, Processing, Shipping, Delivered, Cancelled
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string OrderStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PaidAt { get; set; }

        /// <summary>
        /// Ngày giao hàng thành công
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// Ngày h?y ??n
        /// </summary>
        public DateTime? CancelledAt { get; set; }

        /// <summary>
        /// Lý do h?y ??n
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? CancelReason { get; set; }

        /// <summary>
        /// Phí v?n chuy?n
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;

        /// <summary>
        /// Mã gi?m giá ?ã áp d?ng
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string? CouponCode { get; set; }

        /// <summary>
        /// S? ti?n gi?m giá
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    /// <summary>
    /// Các tr?ng thái ??n hàng
    /// </summary>
    public static class OrderStatusConstants
    {
        public const string Pending = "Pending";           // Ch? xác nh?n
        public const string Confirmed = "Confirmed";       // ?ã xác nh?n
        public const string Processing = "Processing";    // ?ang x? lý
        public const string Shipping = "Shipping";        // ?ang giao hàng
        public const string Delivered = "Delivered";      // ?ã giao hàng
        public const string Cancelled = "Cancelled";      // ?ã h?y
    }

    /// <summary>
    /// Các tr?ng thái thanh toán
    /// </summary>
    public static class PaymentStatusConstants
    {
        public const string Pending = "Pending";       // Ch? thanh toán
        public const string Paid = "Paid";             // ?ã thanh toán
        public const string Failed = "Failed";         // Thanh toán th?t b?i
        public const string Refunded = "Refunded";     // ?ã hoàn ti?n
    }

    /// <summary>
    /// Các ph??ng th?c thanh toán
    /// </summary>
    public static class PaymentMethodConstants
    {
        public const string VNPay = "VNPay";
        public const string COD = "COD";
    }
}
