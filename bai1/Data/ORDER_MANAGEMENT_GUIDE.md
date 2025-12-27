# H??ng D?n S? D?ng H? Th?ng Qu?n Lý ??n Hàng

## ?? T?ng Quan

H? th?ng qu?n lý ??n hàng bao g?m 2 ph?n:
- **Admin Order Management**: Dành cho qu?n tr? viên duy?t và qu?n lý ??n hàng
- **My Order**: Dành cho khách hàng xem tr?ng thái ??n hàng

## ?? Cài ??t

### 1. C?p nh?t Database (n?u c?n)

Model `Order` ?ã có ??y ?? các tr??ng c?n thi?t. N?u c?n migration m?i:

```powershell
# Trong Package Manager Console
Add-Migration AddOrderManagementFields
Update-Database
```

### 2. Ki?m tra Program.cs

??m b?o `OrderService` ?ã ???c ??ng ký:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
```

## ?? Lu?ng X? Lý ??n Hàng

### Tr?ng Thái ??n Hàng

1. **Pending** (Ch? xác nh?n)
   - ??n hàng m?i ???c t?o
   - COD: Ch?a thanh toán
   - VNPay: ?ã thanh toán ho?c ch?a

2. **Confirmed** (?ã xác nh?n)
   - Admin xác nh?n ??n hàng
   - B?t ??u chu?n b? hàng

3. **Processing** (?ang chu?n b?)
   - Admin ?ang ?óng gói s?n ph?m
   - Chu?n b? giao cho ??n v? v?n chuy?n

4. **Shipping** (?ang giao hàng)
   - ??n hàng ?ang ???c v?n chuy?n
   - Khách hàng có th? theo dõi

5. **Delivered** (?ã giao hàng)
   - Giao hàng thành công
   - COD: T? ??ng c?p nh?t "?ã thanh toán"

6. **Cancelled** (?ã h?y)
   - Admin ho?c khách hàng h?y ??n
   - Yêu c?u lý do h?y

## ?? Ch?c N?ng Admin

### Truy c?p Admin Order Management

```
URL: /AdminOrder/Index
```

### Xem danh sách ??n hàng

- L?c theo tr?ng thái
- Th?ng kê t?ng quan
- Tìm ki?m ??n hàng

### Duy?t ??n hàng COD

1. Vào **AdminOrder/Details/{orderId}**
2. Ki?m tra thông tin ??n hàng
3. Click **"Xác nh?n ??n hàng"** (n?u tr?ng thái Pending)
4. C?p nh?t tr?ng thái theo lu?ng:
   - Confirmed ? Processing ? Shipping ? Delivered

### H?y ??n hàng

1. Click **"H?y ??n hàng"**
2. Nh?p lý do h?y (b?t bu?c)
3. Xác nh?n

## ?? Ch?c N?ng User

### Truy c?p My Order

```
URL: /MyOrder/Index
```

### Xem ??n hàng

- Danh sách t?t c? ??n hàng c?a user
- Click "Xem chi ti?t" ?? xem timeline

### H?y ??n hàng

- Ch? h?y ???c ? tr?ng thái **Pending** ho?c **Confirmed**
- Ch?n lý do h?y t? dropdown

## ?? Giao Di?n

### Admin Order List
- Th?ng kê theo tr?ng thái (Cards)
- Filter tabs
- B?ng danh sách v?i actions

### Admin Order Details
- Timeline tr?ng thái
- Thông tin khách hàng
- Chi ti?t s?n ph?m
- Nút c?p nh?t tr?ng thái

### User Order List
- Card view cho m?i ??n hàng
- Hi?n th? 3 s?n ph?m ??u tiên
- Badge tr?ng thái

### User Order Details
- Timeline theo dõi ??n hàng
- Thông tin giao hàng
- Thông tin thanh toán
- Nút h?y ??n (n?u ???c phép)

## ?? Phân Quy?n (TODO)

Hi?n t?i c?n implement thêm:

```csharp
// Thêm vào Controller
[Authorize(Roles = "Admin")]
public class AdminOrderController : Controller
{ ... }

[Authorize]
public class MyOrderController : Controller
{ ... }
```

## ?? API Endpoints

### Admin
- `GET /AdminOrder/Index?status={status}` - Danh sách ??n hàng
- `GET /AdminOrder/Details/{id}` - Chi ti?t ??n hàng
- `POST /AdminOrder/ConfirmOrder` - Xác nh?n ??n hàng
- `POST /AdminOrder/UpdateStatus` - C?p nh?t tr?ng thái
- `POST /AdminOrder/CancelOrder` - H?y ??n hàng

### User
- `GET /MyOrder/Index` - ??n hàng c?a tôi
- `GET /MyOrder/Details/{id}` - Chi ti?t ??n hàng
- `POST /MyOrder/CancelOrder` - H?y ??n hàng

## ?? Tích H?p v?i Checkout

Khi khách hàng ??t hàng COD, trong `CheckoutController`:

```csharp
var order = new Order
{
    UserId = userId,
    PaymentMethod = "COD",
    PaymentStatus = "Pending",  // Ch?a thanh toán
    OrderStatus = "Pending",     // Ch? admin xác nh?n
    // ... other fields
};
```

## ?? Th?ng Kê

OrderService cung c?p:

```csharp
var stats = await _orderService.GetOrderStatisticsAsync();
// Returns: Dictionary<string, int>
// { "Pending": 5, "Confirmed": 3, "Processing": 2, ... }
```

## ?? Customization

### Thay ??i màu s?c tr?ng thái

Edit CSS trong Views ho?c `site.css`:

```css
.bg-purple {
    background-color: #6f42c1 !important;
}

.text-purple {
    color: #6f42c1 !important;
}
```

### Thêm tr?ng thái m?i

1. Thêm vào `OrderStatusConstants` trong `Order.cs`
2. C?p nh?t `OrderStatusHelper.cs`
3. C?p nh?t Views t??ng ?ng

## ?? L?u Ý

1. **Session UserId**: Hi?n t?i dùng Session ?? l?u UserId, c?n chuy?n sang Authentication th?c t?
2. **Authorization**: C?n thêm `[Authorize]` attributes
3. **Notification**: Có th? thêm email/SMS thông báo khi ??n hàng thay ??i tr?ng thái
4. **Inventory**: C?p nh?t t?n kho khi ??n hàng Delivered/Cancelled

## ?? Demo

### Test v?i d? li?u m?u

D? li?u m?u trong `SampleData.sql` ?ã có s?n 4 ??n hàng v?i các tr?ng thái khác nhau ?? test.

### Workflow Test

1. Login as Customer
2. ??t hàng COD ? Tr?ng thái "Pending"
3. Login as Admin ? `/AdminOrder/Index`
4. Xác nh?n ??n hàng ? "Confirmed"
5. Chu?n b? hàng ? "Processing"
6. Giao hàng ? "Shipping"
7. Hoàn thành ? "Delivered" + "Paid"

---

**Chúc b?n tri?n khai thành công! ??**
