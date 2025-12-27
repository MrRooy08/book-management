using bai1.Models;
using bai1.Models.Dto;
using bai1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace bai1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ApplicationDbContext context, 
            IVnPayService vnPayService,
            IInventoryService inventoryService,
            ILogger<CheckoutController> logger)
        {
            _context = context;
            _vnPayService = vnPayService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cart))
            {
                TempData["Error"] = "Gi? hàng tr?ng!";
                return RedirectToAction("Index", "Cart");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Gi? hàng tr?ng!";
                return RedirectToAction("Index", "Cart");
            }

            // Không ki?m tra t?n kho - cho phép ??t hàng t? do (overselling/backorder)

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
                PaymentMethod = PaymentMethodConstants.COD
            };

            // Check if user is logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null)
                    {
                        model.CustomerName = user.Name ?? string.Empty;
                        model.Email = user.Email;
                    }

                    // Load saved addresses
                    model.SavedAddresses = await _context.UserAddresses
                        .Where(a => a.UserId == userId)
                        .OrderByDescending(a => a.IsDefault)
                        .ToListAsync();

                    // If has default address, pre-fill
                    var defaultAddress = model.SavedAddresses.FirstOrDefault(a => a.IsDefault);
                    if (defaultAddress != null)
                    {
                        model.SelectedAddressId = defaultAddress.Id;
                        model.Phone = defaultAddress.Phone ?? string.Empty;
                        model.Province = defaultAddress.Province;
                        model.District = defaultAddress.District;
                        model.Ward = defaultAddress.Ward;
                        model.DetailAddress = defaultAddress.Address ?? string.Empty;
                        if (!string.IsNullOrEmpty(defaultAddress.ReceiverName))
                        {
                            model.CustomerName = defaultAddress.ReceiverName;
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(cart))
            {
                TempData["Error"] = "Gi? hàng tr?ng!";
                return RedirectToAction("Index", "Cart");
            }

            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new List<CartItem>();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Gi? hàng tr?ng!";
                return RedirectToAction("Index", "Cart");
            }

            model.CartItems = cartItems;
            model.TotalAmount = cartItems.Sum(i => i.Price * i.Quantity);

            if (!ModelState.IsValid)
            {
                // Reload saved addresses if user is logged in
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        model.SavedAddresses = await _context.UserAddresses
                            .Where(a => a.UserId == userId)
                            .OrderByDescending(a => a.IsDefault)
                            .ToListAsync();
                    }
                }
                return View("Index", model);
            }

            // Không ki?m tra t?n kho - cho phép ??t hàng t? do (overselling/backorder)

            // Build full shipping address
            var addressParts = new List<string>();
            if (!string.IsNullOrEmpty(model.DetailAddress)) addressParts.Add(model.DetailAddress);
            if (!string.IsNullOrEmpty(model.Ward)) addressParts.Add(model.Ward);
            if (!string.IsNullOrEmpty(model.District)) addressParts.Add(model.District);
            if (!string.IsNullOrEmpty(model.Province)) addressParts.Add(model.Province);
            var fullAddress = string.Join(", ", addressParts);

            int? userId_nullable = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    userId_nullable = userId;

                    // Save address if requested
                    if (model.SaveAddress)
                    {
                        var existingAddresses = await _context.UserAddresses
                            .Where(a => a.UserId == userId)
                            .ToListAsync();

                        var newAddress = new UserAddress
                        {
                            UserId = userId,
                            ReceiverName = model.CustomerName,
                            Phone = model.Phone,
                            Address = model.DetailAddress,
                            Province = model.Province,
                            District = model.District,
                            Ward = model.Ward,
                            IsDefault = !existingAddresses.Any(),
                            CreatedAt = DateTime.Now
                        };

                        _context.UserAddresses.Add(newAddress);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // Xác ??nh payment status d?a trên ph??ng th?c thanh toán
            var isCOD = model.PaymentMethod == PaymentMethodConstants.COD;
            var initialPaymentStatus = isCOD ? PaymentStatusConstants.Pending : PaymentStatusConstants.Pending;
            var initialOrderStatus = isCOD ? OrderStatusConstants.Confirmed : OrderStatusConstants.Pending;

            // Create order
            var order = new Order
            {
                UserId = userId_nullable,
                CustomerName = model.CustomerName,
                Email = model.Email,
                Phone = model.Phone,
                ShippingAddress = fullAddress,
                Note = model.Note,
                TotalAmount = model.TotalAmount,
                ShippingFee = model.ShippingFee,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = initialPaymentStatus,
                OrderStatus = initialOrderStatus,
                CreatedAt = DateTime.Now
            };

            // Add order details
            foreach (var item in cartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    BookId = item.BookId,
                    BookTitle = item.Title,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Subtotal = item.Price * item.Quantity
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // ??t tr??c hàng trong kho (cho phép overselling - không rollback n?u h?t hàng)
            await _inventoryService.ReserveStockAsync(order.Id, order.OrderDetails.ToList());

            // X? lý theo ph??ng th?c thanh toán
            if (isCOD)
            {
                // COD: ??n hàng ???c xác nh?n ngay, ch? giao hàng
                _logger.LogInformation("??n hàng COD #{OrderId} ?ã ???c t?o thành công", order.Id);

                // Clear cart
                HttpContext.Session.Remove("cart");
                HttpContext.Session.Remove("pending_order_id");

                // Redirect ??n trang k?t qu?
                return RedirectToAction("OrderSuccess", new { orderId = order.Id });
            }
            else
            {
                // VNPay: Chuy?n ??n c?ng thanh toán
                HttpContext.Session.SetInt32("pending_order_id", order.Id);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                var orderInfo = $"Thanh toan don hang #{order.Id}";
                var paymentUrl = _vnPayService.CreatePaymentUrl(order.Id, order.TotalAmount, orderInfo, ipAddress);

                return Redirect(paymentUrl);
            }
        }

        /// <summary>
        /// Trang thành công cho ??n hàng COD
        /// </summary>
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var result = new PaymentResultViewModel
            {
                IsSuccess = true,
                OrderId = orderId,
                Amount = order.TotalAmount,
                Message = order.PaymentMethod == PaymentMethodConstants.COD 
                    ? "??t hàng thành công! B?n s? thanh toán khi nh?n hàng."
                    : "Thanh toán thành công!",
                PaymentMethod = order.PaymentMethod
            };

            return View("PaymentResult", result);
        }

        public async Task<IActionResult> PaymentCallback()
        {
            var isSuccess = _vnPayService.ValidateCallback(Request.Query, out string transactionId, out int orderId);

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return View("PaymentResult", new PaymentResultViewModel
                {
                    IsSuccess = false,
                    Message = "Không tìm th?y ??n hàng!"
                });
            }

            var result = new PaymentResultViewModel
            {
                OrderId = orderId,
                TransactionId = transactionId,
                Amount = order.TotalAmount,
                BankCode = Request.Query["vnp_BankCode"].ToString(),
                PaymentMethod = order.PaymentMethod
            };

            if (isSuccess)
            {
                // Update order status
                order.PaymentStatus = PaymentStatusConstants.Paid;
                order.OrderStatus = OrderStatusConstants.Confirmed;
                order.VnpayTransactionId = transactionId;
                order.PaidAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Clear cart
                HttpContext.Session.Remove("cart");
                HttpContext.Session.Remove("pending_order_id");

                result.IsSuccess = true;
                result.Message = "Thanh toán thành công!";
                result.PayDate = order.PaidAt;
            }
            else
            {
                // Update order status to failed
                order.PaymentStatus = PaymentStatusConstants.Failed;
                order.OrderStatus = OrderStatusConstants.Cancelled;
                order.CancelledAt = DateTime.Now;
                order.CancelReason = "Thanh toán VNPay th?t b?i";
                await _context.SaveChangesAsync();

                // Gi?i phóng hàng ?ã ??t tr??c
                await _inventoryService.ReleaseReservedStockAsync(order.Id);

                result.IsSuccess = false;
                result.Message = "Thanh toán th?t b?i. Vui lòng th? l?i!";
            }

            return View("PaymentResult", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadAddress(int addressId)
        {
            var address = await _context.UserAddresses.FindAsync(addressId);
            if (address == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                receiverName = address.ReceiverName,
                phone = address.Phone,
                province = address.Province,
                district = address.District,
                ward = address.Ward,
                detailAddress = address.Address
            });
        }
    }
}
