using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace bai1.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress);
        bool ValidateCallback(IQueryCollection query, out string transactionId, out int orderId);
    }

    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _config;
        private readonly ILogger<VnPayService> _logger;

        public VnPayService(IOptions<VnPayConfig> config, ILogger<VnPayService> logger)
        {
            _config = config.Value;
            _logger = logger;
            
            // Log để debug
            _logger.LogInformation("VnPay Config loaded - TmnCode: {TmnCode}, HashSecret length: {Length}", 
                _config.TmnCode, _config.HashSecret?.Length ?? 0);
        }

        public string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress)
        {
            // TxnRef phải unique
            var txnRef = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            // 1️⃣ Tạo danh sách tham số (RAW)
            var vnpay = new SortedList<string, string>
    {
        { "vnp_Version", _config.Version },           // 2.1.0
        { "vnp_Command", _config.Command },           // pay
        { "vnp_TmnCode", _config.TmnCode },           // Sandbox TMN
        { "vnp_Amount", ((long)(amount * 100)).ToString() }, // VNĐ x 100
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "vnp_CurrCode", _config.CurrCode },         // VND
        { "vnp_IpAddr", ipAddress },
        { "vnp_Locale", _config.Locale },             // vn
        { "vnp_OrderInfo", $"{orderId}|{orderInfo}" },
        { "vnp_OrderType", "other" },
        { "vnp_ReturnUrl", _config.ReturnUrl },
        { "vnp_TxnRef", txnRef }
    };

            // 2️⃣ Encode value trước
            var encodedParams = new SortedList<string, string>();
            foreach (var kv in vnpay)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    encodedParams.Add(
                        kv.Key,
                        WebUtility.UrlEncode(kv.Value)
                    );
                }
            }

            // 3️⃣ Build hash data (TỪ DATA ĐÃ ENCODE)
            var hashData = new StringBuilder();
            foreach (var kv in encodedParams)
            {
                if (hashData.Length > 0)
                    hashData.Append('&');

                hashData.Append(kv.Key);
                hashData.Append('=');
                hashData.Append(kv.Value);
            }

            var rawHashData = hashData.ToString();
            _logger.LogInformation("VNPay HashData: {Data}", rawHashData);

            // 4️⃣ Tạo SecureHash
            var secureHash = HmacSha512(_config.HashSecret, rawHashData);
            _logger.LogInformation("VNPay SecureHash: {Hash}", secureHash);

            // 5️⃣ Build query string
            var queryString = string.Join("&",
                encodedParams.Select(kv => $"{kv.Key}={kv.Value}")
            );

            // 6️⃣ Final payment URL
            var paymentUrl = $"{_config.BaseUrl}?{queryString}&vnp_SecureHash={secureHash}";
            _logger.LogInformation("VNPay PaymentUrl: {Url}", paymentUrl);

            return paymentUrl;
        }

        public bool ValidateCallback(IQueryCollection queryCollection, out string transactionId, out int orderId)
        {
            transactionId = string.Empty;
            orderId = 0;

            try
            {
                var vnpSecureHash = queryCollection["vnp_SecureHash"].ToString();
                var responseCode = queryCollection["vnp_ResponseCode"].ToString();
                transactionId = queryCollection["vnp_TransactionNo"].ToString();
                var orderInfoRaw = queryCollection["vnp_OrderInfo"].ToString();

                // Parse orderId từ OrderInfo
                if (orderInfoRaw.Contains('|'))
                {
                    var parts = orderInfoRaw.Split('|');
                    int.TryParse(parts[0], out orderId);
                }

                _logger.LogInformation("VnPay Callback - ResponseCode: {Code}, TransactionId: {TxnId}, OrderId: {OrderId}",
                    responseCode, transactionId, orderId);

                // Build data to validate hash
                var vnpay = new SortedList<string, string>();
                foreach (var key in queryCollection.Keys)
                {
                    var value = queryCollection[key].ToString();
                    if (!string.IsNullOrEmpty(value) && 
                        key.StartsWith("vnp_") && 
                        !key.Equals("vnp_SecureHash") && 
                        !key.Equals("vnp_SecureHashType"))
                    {
                        vnpay.Add(key, value);
                    }
                }

                // Build hash data
                var hashData = new StringBuilder();
                foreach (var kvp in vnpay)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        if (hashData.Length > 0)
                            hashData.Append('&');
                        hashData.Append(kvp.Key);
                        hashData.Append('=');
                        hashData.Append(kvp.Value);
                    }
                }

                var rawData = hashData.ToString();
                var checkSum = HmacSha512(_config.HashSecret, rawData);

                _logger.LogInformation("VnPay Callback - Received Hash: {Received}, Computed Hash: {Computed}",
                    vnpSecureHash, checkSum);

                if (!checkSum.Equals(vnpSecureHash, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("VnPay Callback - Hash mismatch!");
                    return false;
                }

                return responseCode == "00";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VnPay Callback validation error");
                return false;
            }
        }

        private string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
