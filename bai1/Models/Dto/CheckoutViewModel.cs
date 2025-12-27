using System.ComponentModel.DataAnnotations;

namespace bai1.Models.Dto
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nh?p h? tên")]
        [Display(Name = "H? và tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nh?p email")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nh?p s? ?i?n tho?i")]
        [Phone(ErrorMessage = "S? ?i?n tho?i không h?p l?")]
        [Display(Name = "S? ?i?n tho?i")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "T?nh/Thành ph?")]
        public string? Province { get; set; }

        [Display(Name = "Qu?n/Huy?n")]
        public string? District { get; set; }

        [Display(Name = "Ph??ng/Xã")]
        public string? Ward { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p ??a ch? chi ti?t")]
        [Display(Name = "??a ch? chi ti?t")]
        public string DetailAddress { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "L?u ??a ch? cho l?n sau")]
        public bool SaveAddress { get; set; }

        /// <summary>
        /// Ph??ng th?c thanh toán: VNPay, COD
        /// </summary>
        [Required(ErrorMessage = "Vui lòng ch?n ph??ng th?c thanh toán")]
        [Display(Name = "Ph??ng th?c thanh toán")]
        public string PaymentMethod { get; set; } = "COD";

        // Cart info
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public decimal FinalAmount => TotalAmount + ShippingFee;

        // For logged in users
        public int? SelectedAddressId { get; set; }
        public List<UserAddress>? SavedAddresses { get; set; }
    }
}
