using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        
        public int Quantity { get; set; }
        
        public int ReservedQuantity { get; set; } = 0; // Số lượng đã đặt nhưng chưa thanh toán
        
        public int SoldQuantity { get; set; } = 0; // Số lượng đã bán
        
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        
        public Book Book { get; set; } = null!;
        
        // Số lượng có thể bán = Tồn kho - Đã đặt
        [NotMapped]
        public int AvailableQuantity => Quantity - ReservedQuantity;
    }
}
