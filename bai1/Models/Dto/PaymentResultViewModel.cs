namespace bai1.Models.Dto
{
    public class PaymentResultViewModel
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? TransactionId { get; set; }
        public int? OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? BankCode { get; set; }
        public DateTime? PayDate { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
