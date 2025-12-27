using System.ComponentModel.DataAnnotations;

namespace bai1.Models.Dto
{
    public class BookAuthorsViewModels
    {
        public int Id { get; set; }

        public string? ISBN { get; set; }
        public string? Title { get; set; } 

        public string? Description { get; set; }

        public DateTime PublishDate { get; set; }

        // Dimension fields - nhận dạng string để hỗ trợ format "14.5" hoặc "14,5"
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }

        public string? Format { get; set; }

        public int PageCount { get; set; }

        public string? Language { get; set; }

        // Giá niêm yết (giá bìa) - nhận dạng string để hỗ trợ format "150.000"
        public string? ListPrice { get; set; }

        public int CategoryIds { get; set; }
        public int PublisherId { get; set; }

        // Giá vốn mua từ NCC - nhận dạng string để hỗ trợ format "150.000"
        public string? CostPrice { get; set; }

        // Giá bán thực tế - nhận dạng string để hỗ trợ format "150.000"
        public string? SalePrice { get; set; }

        public int Inventory { get; set; }

        // Tác giả - bắt buộc phải có ít nhất 1 tác giả
        public string? AuthorIds { get; set; }

        // Dịch giả - không bắt buộc (nhiều sách không cần dịch giả)
        public string? TranslatorData { get; set; }

        /// <summary>
        /// Parse dimension từ string (hỗ trợ format: 14.5, 14,5)
        /// </summary>
        public static float ParseDimension(string? dimensionString)
        {
            if (string.IsNullOrWhiteSpace(dimensionString))
                return 0;

            var cleaned = dimensionString.Trim();

            // Thay thế dấu phẩy thành dấu chấm để chuẩn hóa
            cleaned = cleaned.Replace(",", ".");

            if (float.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Lấy chiều dài đã parse
        /// </summary>
        public float GetLengthFloat() => ParseDimension(Length);

        /// <summary>
        /// Lấy chiều rộng đã parse
        /// </summary>
        public float GetWidthFloat() => ParseDimension(Width);

        /// <summary>
        /// Lấy chiều cao đã parse
        /// </summary>
        public float GetHeightFloat() => ParseDimension(Height);

        /// <summary>
        /// Lấy cân nặng đã parse
        /// </summary>
        public float GetWeightFloat() => ParseDimension(Weight);

        /// <summary>
        /// Parse giá từ string (hỗ trợ format: 150000, 150.000, 150,000)
        /// </summary>
        public static decimal ParsePrice(string? priceString)
        {
            if (string.IsNullOrWhiteSpace(priceString))
                return 0;

            // Loại bỏ ký tự không phải số (giữ lại dấu phẩy và chấm)
            var cleaned = priceString.Trim()
                .Replace("₫", "")
                .Replace("VND", "")
                .Replace(" ", "");

            // Xử lý format Việt Nam (150.000 hoặc 150.000,00)
            // Nếu có dấu chấm và không có dấu phẩy -> dấu chấm là phân cách hàng nghìn
            if (cleaned.Contains('.') && !cleaned.Contains(','))
            {
                cleaned = cleaned.Replace(".", "");
            }
            // Nếu có cả dấu chấm và dấu phẩy -> dấu chấm là hàng nghìn, dấu phẩy là thập phân
            else if (cleaned.Contains('.') && cleaned.Contains(','))
            {
                cleaned = cleaned.Replace(".", "").Replace(",", ".");
            }
            // Nếu chỉ có dấu phẩy -> có thể là hàng nghìn hoặc thập phân
            else if (cleaned.Contains(','))
            {
                // Kiểm tra nếu sau dấu phẩy có đúng 3 số -> phân cách hàng nghìn
                var parts = cleaned.Split(',');
                if (parts.Length == 2 && parts[1].Length == 3)
                {
                    cleaned = cleaned.Replace(",", "");
                }
                else
                {
                    // Dấu phẩy là thập phân
                    cleaned = cleaned.Replace(",", ".");
                }
            }

            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any, 
                System.Globalization.CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Lấy giá niêm yết đã parse
        /// </summary>
        public decimal GetListPriceDecimal() => ParsePrice(ListPrice);

        /// <summary>
        /// Lấy giá vốn đã parse
        /// </summary>
        public decimal GetCostPriceDecimal() => ParsePrice(CostPrice);

        /// <summary>
        /// Lấy giá bán đã parse
        /// </summary>
        public decimal GetSalePriceDecimal() => ParsePrice(SalePrice);
    }
}
