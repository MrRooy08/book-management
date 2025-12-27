namespace bai1.Helpers
{
    public static class OrderStatusHelper
    {
        public static string GetStatusDisplayName(string status)
        {
            return status switch
            {
                "Pending" => "Ch? xác nh?n",
                "Confirmed" => "?ã xác nh?n",
                "Processing" => "?ang chu?n b? hàng",
                "Shipping" => "?ang giao hàng",
                "Delivered" => "?ã giao hàng",
                "Cancelled" => "?ã h?y",
                _ => status
            };
        }

        public static string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Pending" => "bg-warning text-dark",
                "Confirmed" => "bg-info",
                "Processing" => "bg-primary",
                "Shipping" => "bg-purple",
                "Delivered" => "bg-success",
                "Cancelled" => "bg-danger",
                _ => "bg-secondary"
            };
        }

        public static string GetStatusIcon(string status)
        {
            return status switch
            {
                "Pending" => "bi-clock",
                "Confirmed" => "bi-check-circle",
                "Processing" => "bi-box-seam",
                "Shipping" => "bi-truck",
                "Delivered" => "bi-check2-all",
                "Cancelled" => "bi-x-circle",
                _ => "bi-question-circle"
            };
        }

        public static string GetPaymentStatusDisplayName(string status)
        {
            return status switch
            {
                "Pending" => "Ch? thanh toán",
                "Paid" => "?ã thanh toán",
                "Failed" => "Thanh toán th?t b?i",
                "Refunded" => "?ã hoàn ti?n",
                _ => status
            };
        }

        public static string GetPaymentMethodDisplayName(string method)
        {
            return method switch
            {
                "COD" => "Thanh toán khi nh?n hàng",
                "VNPay" => "VNPay",
                _ => method
            };
        }
    }
}
