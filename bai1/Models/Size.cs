using Microsoft.EntityFrameworkCore;

namespace bai1.Models
{
    [Owned]
    public class BookDimensions
    {
        public float Height { get; set; }
        public float Width { get; set; }

        public float Length { get; set; }

        
    }
}
