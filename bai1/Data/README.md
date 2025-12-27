# H??ng D?n S? D?ng SQL Scripts

## ?? C?u trúc Files

- **SampleData.sql**: Script ??y ?? v?i d? li?u chi ti?t và có xóa d? li?u c?
- **QuickInsert.sql**: Script nhanh, ch? insert d? li?u c? b?n (không xóa d? li?u c?)

## ?? Cách S? D?ng

### Ph??ng án 1: S? d?ng SQL Server Management Studio (SSMS)

1. M? **SQL Server Management Studio**
2. Connect ??n SQL Server c?a b?n
3. M? file `SampleData.sql` ho?c `QuickInsert.sql`
4. S?a dòng ??u tiên n?u c?n:
   ```sql
   USE [BookManagementDB]  -- Thay tên database c?a b?n
   ```
5. Nh?n **F5** ho?c **Execute** ?? ch?y

### Ph??ng án 2: S? d?ng Visual Studio

1. M? **SQL Server Object Explorer** trong Visual Studio
2. K?t n?i ??n database
3. Click ph?i vào database > **New Query**
4. Copy n?i dung t? file SQL và paste vào
5. Click **Execute** ho?c nh?n **Ctrl+Shift+E**

### Ph??ng án 3: S? d?ng Package Manager Console

```powershell
# Trong Package Manager Console c?a Visual Studio
Invoke-Sqlcmd -InputFile ".\bai1\Data\SampleData.sql" -ServerInstance "(localdb)\mssqllocaldb" -Database "BookManagementDB"
```

## ?? D? Li?u M?u Bao G?m

### SampleData.sql (??y ??)
- ? 5 Publishers (Nhà xu?t b?n)
- ? 14 Categories (5 danh m?c cha + 9 danh m?c con)
- ? 14 Persons (Tác gi? và D?ch gi?)
- ? 12 Books (Sách ?a d?ng th? lo?i)
- ? 12 Inventories (T?n kho cho m?i sách)
- ? Book-Author relationships
- ? Book-Translator relationships
- ? Book-Category relationships
- ? Book Images
- ? 3 Roles (Admin, Customer, Staff)
- ? 5 Users
- ? 4 User Addresses
- ? 4 Orders v?i 9 Order Details

### QuickInsert.sql (C? b?n)
- ? 5 Publishers
- ? 14 Categories
- ? 12 Persons
- ? 10 Books
- ? 10 Inventories
- ? Relationships c? b?n
- ? 3 Roles
- ? 4 Users

## ?? L?u Ý Quan Tr?ng

### SampleData.sql
- **Xóa toàn b? d? li?u c?** tr??c khi insert
- Reset IDENTITY v? 0
- Phù h?p cho **môi tr??ng Development/Testing**
- **KHÔNG nên** ch?y trên Production có d? li?u th?t

### QuickInsert.sql
- **KHÔNG xóa** d? li?u c?
- Ch? insert thêm d? li?u m?i
- Có th? gây l?i n?u ?ã có d? li?u trùng l?p
- An toàn h?n cho vi?c thêm d? li?u m?u

## ?? Ki?m Tra D? Li?u Sau Khi Insert

```sql
-- Ki?m tra s? l??ng records
SELECT 'Books' as TableName, COUNT(*) as Count FROM Books
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Publishers', COUNT(*) FROM Publishers
UNION ALL
SELECT 'Persons', COUNT(*) FROM Persons
UNION ALL
SELECT 'Inventories', COUNT(*) FROM Inventories
UNION ALL
SELECT 'Users', COUNT(*) FROM Users
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders;

-- Xem sách v?i ??y ?? thông tin
SELECT 
    b.Id, b.Title, b.SalePrice,
    p.Name as Publisher,
    i.Quantity as Stock,
    STRING_AGG(c.Name, ', ') as Categories
FROM Books b
LEFT JOIN Publishers p ON b.PublisherId = p.Id
LEFT JOIN Inventories i ON b.Id = i.BookId
LEFT JOIN BookCategory bc ON b.Id = bc.BooksId
LEFT JOIN Categories c ON bc.CategoriesId = c.Id
GROUP BY b.Id, b.Title, b.SalePrice, p.Name, i.Quantity;
```

## ?? Troubleshooting

### L?i: "Cannot insert duplicate key"
- ?ã có d? li?u trong database
- **Gi?i pháp**: S? d?ng `SampleData.sql` (xóa h?t d? li?u c?) ho?c xóa th? công

### L?i: "Foreign key constraint"
- Th? t? insert không ?úng
- **Gi?i pháp**: Ch?y l?i toàn b? script t? ??u

### L?i: "Invalid object name"
- Database ch?a có b?ng
- **Gi?i pháp**: Ch?y migration tr??c:
  ```powershell
  Update-Database
  ```

## ?? Tùy Ch?nh D? Li?u

B?n có th? s?a ??i các giá tr? trong script:

```sql
-- Ví d?: Thay ??i giá sách
UPDATE Books SET SalePrice = 99000 WHERE Id = 1;

-- Thêm sách m?i
INSERT INTO Books (...) VALUES (...);

-- T?ng t?n kho
UPDATE Inventories SET Quantity = Quantity + 100 WHERE BookId = 1;
```

## ?? M?c ?ích S? D?ng

- ? Testing ch?c n?ng CRUD
- ? Demo cho khách hàng
- ? Development và Debug
- ? H?c t?p và Training
- ? **KHÔNG** dùng cho Production

## ?? H? Tr?

N?u g?p v?n ??, ki?m tra:
1. Connection string trong `appsettings.json`
2. SQL Server ?ang ch?y
3. Database ?ã ???c t?o (migration)
4. Quy?n truy c?p database

---
**Chúc b?n làm vi?c hi?u qu?! ??**
