-- ============================================
-- Script t?o d? li?u m?u cho Book Management System
-- ============================================

USE [BookManagementDB]
GO

-- ============================================
-- 1. Xóa d? li?u c? (n?u có) theo th? t? ph? thu?c
-- ============================================
DELETE FROM [OrderDetails];
DELETE FROM [Orders];
DELETE FROM [UserAddresses];
DELETE FROM [RoleUser];
DELETE FROM [BookAuthors];
DELETE FROM [BookTranslators];
DELETE FROM [BookCategory];
DELETE FROM [BookImage];
DELETE FROM [Inventories];
DELETE FROM [Books];
DELETE FROM [Persons];
DELETE FROM [Categories];
DELETE FROM [Publishers];
DELETE FROM [Users];
DELETE FROM [Roles];

-- Reset IDENTITY
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('UserAddresses', RESEED, 0);
DBCC CHECKIDENT ('Books', RESEED, 0);
DBCC CHECKIDENT ('Persons', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Publishers', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('Inventories', RESEED, 0);
DBCC CHECKIDENT ('BookImage', RESEED, 0);

-- ============================================
-- 2. Thêm Roles
-- ============================================
INSERT INTO [Roles] ([RoleName], [RoleDescription])
VALUES 
    (N'Admin', N'Qu?n tr? viên h? th?ng'),
    (N'Customer', N'Khách hàng'),
    (N'Staff', N'Nhân viên');
GO

-- ============================================
-- 3. Thêm Users
-- ============================================
INSERT INTO [Users] ([Name], [Email], [BirthDay], [Password])
VALUES 
    (N'Nguy?n V?n Admin', 'admin@bookstore.com', '1990-01-15', 'admin123'),
    (N'Tr?n Th? Lan', 'lan.tran@gmail.com', '1995-05-20', 'password123'),
    (N'Lê Minh Tu?n', 'tuan.le@gmail.com', '1998-08-10', 'password123'),
    (N'Ph?m Thu Hà', 'ha.pham@gmail.com', '2000-03-25', 'password123'),
    (N'Hoàng V?n Long', 'long.hoang@gmail.com', '1997-11-30', 'password123');
GO

-- ============================================
-- 4. Gán Roles cho Users
-- ============================================
INSERT INTO [RoleUser] ([RolesId], [UsersId])
VALUES 
    (1, 1), -- Admin
    (2, 2), -- Customer
    (2, 3), -- Customer
    (2, 4), -- Customer
    (3, 5); -- Staff
GO

-- ============================================
-- 5. Thêm Publishers (Nhà xu?t b?n)
-- ============================================
INSERT INTO [Publishers] ([Name], [Address], [Phone], [Email], [Slug])
VALUES 
    (N'NXB Tr?', N'161B Lý Chính Th?ng, Ph??ng 7, Qu?n 3, TP.HCM', '0283932343', 'hopthubandoc@nxbtre.com.vn', 'nxb-tre'),
    (N'NXB Kim ??ng', N'55 Quang Trung, Nguy?n Du, Hai Bà Tr?ng, Hà N?i', '0243943344', 'info@nxbkimdong.com.vn', 'nxb-kim-dong'),
    (N'NXB V?n H?c', N'18 Nguy?n Tr??ng T?, Ba ?ình, Hà N?i', '0438222920', 'vanhocinfor@gmail.com', 'nxb-van-hoc'),
    (N'First News', N'195 ?i?n Biên Ph?, Ph??ng 15, Bình Th?nh, TP.HCM', '0283822896', 'info@firstnews.com.vn', 'first-news'),
    (N'Alphabooks', N'67/1 Lý Chính Th?ng, Ph??ng 8, Qu?n 3, TP.HCM', '0283520794', 'alphabooks@gmail.com', 'alphabooks');
GO

-- ============================================
-- 6. Thêm Categories (Danh m?c)
-- ============================================
-- Danh m?c cha
INSERT INTO [Categories] ([Name], [Slug], [ParentId])
VALUES 
    (N'V?n h?c', 'van-hoc', NULL),
    (N'Kinh t?', 'kinh-te', NULL),
    (N'K? n?ng s?ng', 'ky-nang-song', NULL),
    (N'Thi?u nhi', 'thieu-nhi', NULL),
    (N'Công ngh?', 'cong-nghe', NULL);
GO

-- Danh m?c con
INSERT INTO [Categories] ([Name], [Slug], [ParentId])
VALUES 
    (N'V?n h?c Vi?t Nam', 'van-hoc-viet-nam', 1),
    (N'V?n h?c n??c ngoài', 'van-hoc-nuoc-ngoai', 1),
    (N'Ti?u thuy?t', 'tieu-thuyet', 1),
    (N'Qu?n tr? - Lãnh ??o', 'quan-tri-lanh-dao', 2),
    (N'Marketing - Bán hàng', 'marketing-ban-hang', 2),
    (N'K? n?ng giao ti?p', 'ky-nang-giao-tiep', 3),
    (N'T? duy - H?c t?p', 'tu-duy-hoc-tap', 3),
    (N'Truy?n tranh', 'truyen-tranh', 4),
    (N'L?p trình', 'lap-trinh', 5),
    (N'AI - Machine Learning', 'ai-machine-learning', 5);
GO

-- ============================================
-- 7. Thêm Persons (Tác gi?, D?ch gi?)
-- ============================================
INSERT INTO [Persons] ([Name], [Description])
VALUES 
    (N'Nguy?n Nh?t Ánh', N'Nhà v?n n?i ti?ng Vi?t Nam'),
    (N'Tô Hoài', N'Nhà v?n Vi?t Nam'),
    (N'Dale Carnegie', N'Tác gi? sách k? n?ng s?ng n?i ti?ng'),
    (N'Robert Kiyosaki', N'Tác gi? sách D?y Con Làm Giàu'),
    (N'Stephen R. Covey', N'Tác gi? 7 Thói Quen Hi?u Qu?'),
    (N'Haruki Murakami', N'Nhà v?n Nh?t B?n'),
    (N'J.K. Rowling', N'Tác gi? Harry Potter'),
    (N'Nguy?n H?u Trí', N'D?ch gi?'),
    (N'Lê Minh Qu?c', N'D?ch gi?'),
    (N'Hoàng Anh D?ng', N'D?ch gi?'),
    (N'Paulo Coelho', N'Tác gi? Nhà Gi? Kim'),
    (N'Yuval Noah Harari', N'Tác gi? Sapiens'),
    (N'Tony Bu?i Sáng', N'Tác gi?, di?n gi? Vi?t Nam'),
    (N'Rosie Nguy?n', N'Tác gi? sách k? n?ng s?ng');
GO

-- ============================================
-- 8. Thêm Books
-- ============================================
INSERT INTO [Books] ([ISBN], [Title], [Language], [PublishDate], [PageCount], [Weight], 
                      [ShortDescription], [Description], [ListPrice], [SalePrice], [CostPrice], 
                      [Format], [Dimensions_Height], [Dimensions_Width], [Dimensions_Length], [PublisherId])
VALUES 
    -- V?n h?c
    ('9786041138704', N'M?t Bi?c', 'vi', '2020-01-15', 368, 350, 
     N'Tác ph?m v?n h?c hay nh?t c?a Nguy?n Nh?t Ánh', 
     N'M?t bi?c là m?t truy?n dài c?a nhà v?n Nguy?n Nh?t Ánh. Cu?n sách ?ã ???c chuy?n th? thành phim ?i?n ?nh cùng tên.',
     150000, 120000, 80000, 'paperback', 20.5, 14.5, 2.0, 1),
    
    ('9786041138711', N'Tôi Th?y Hoa Vàng Trên C? Xanh', 'vi', '2019-05-20', 400, 380,
     N'Câu chuy?n v? tu?i th? d? d?i và tinh khôi', 
     N'Tôi th?y hoa vàng trên c? xanh là m?t truy?n dài c?a nhà v?n Nguy?n Nh?t Ánh.',
     160000, 135000, 90000, 'paperback', 20.5, 14.5, 2.2, 1),
    
    ('9780345803481', N'1Q84 (B?n ti?ng Anh)', 'en', '2018-03-10', 928, 800,
     N'Masterpiece c?a Haruki Murakami',
     N'1Q84 là m?t ti?u thuy?t c?a nhà v?n Nh?t B?n Haruki Murakami, ???c xu?t b?n l?n ??u b?ng ti?ng Nh?t vào n?m 2009-2010.',
     450000, 380000, 250000, 'paperback', 23.0, 15.5, 5.0, 4),
    
    -- Kinh t?
    ('9780062837691', N'D?y Con Làm Giàu (Rich Dad Poor Dad)', 'vi', '2021-06-01', 336, 320,
     N'Sách v? tài chính cá nhân bán ch?y nh?t',
     N'Cha Giàu, Cha Nghèo s? giúp b?n phá v? quan ni?m l?i th?i cho r?ng ch? c?n h?c th?t gi?i.',
     180000, 145000, 95000, 'paperback', 20.0, 14.0, 1.8, 5),
    
    ('9780684858395', N'7 Thói Quen Hi?u Qu?', 'vi', '2020-08-15', 432, 400,
     N'Cu?n sách v? k? n?ng s?ng và làm vi?c hi?u qu?',
     N'Cu?n sách ?ã giúp hàng tri?u ng??i trên th? gi?i c?i thi?n hi?u su?t làm vi?c và cu?c s?ng.',
     200000, 165000, 110000, 'hardcover', 21.0, 14.5, 2.5, 4),
    
    -- K? n?ng s?ng
    ('9780671027032', N'??c Nhân Tâm', 'vi', '2019-01-10', 320, 300,
     N'Sách k? n?ng giao ti?p kinh ?i?n',
     N'??c nhân tâm c?a Dale Carnegie là quy?n sách n?i ti?ng nh?t, bán ch?y nh?t và có t?m ?nh h??ng nh?t.',
     120000, 95000, 60000, 'paperback', 20.0, 13.0, 1.6, 1),
    
    ('9786041183902', N'Trên ???ng B?ng', 'vi', '2021-11-20', 280, 260,
     N'Sách c?a Tony Bu?i Sáng v? ??nh h??ng và phát tri?n s? nghi?p',
     N'Trên ???ng B?ng là cu?n sách t?ng h?p nh?ng bài vi?t truy?n c?m h?ng c?a Tony Bu?i Sáng.',
     135000, 108000, 70000, 'paperback', 19.0, 13.5, 1.5, 1),
    
    -- Thi?u nhi
    ('9780747532699', N'Harry Potter Và Hòn ?á Phù Th?y', 'vi', '2020-02-14', 368, 340,
     N'Ph?n ??u tiên c?a series Harry Potter',
     N'Harry Potter là m?t lo?t ti?u thuy?t k? ?o g?m b?y ph?n c?a n? nhà v?n ng??i Anh J. K. Rowling.',
     170000, 140000, 95000, 'paperback', 20.5, 14.0, 2.0, 2),
    
    ('9786041161191', N'D? Mène Phiêu L?u Ký', 'vi', '2018-09-05', 196, 180,
     N'Tác ph?m kinh ?i?n v?n h?c thi?u nhi Vi?t Nam',
     N'D? Mèn phiêu l?u ký là m?t truy?n dài c?a nhà v?n Tô Hoài vi?t v? m?t chú d? mèn t?t b?ng.',
     80000, 65000, 40000, 'paperback', 19.0, 13.0, 1.2, 2),
    
    -- Công ngh?
    ('9780262033848', N'Introduction to Algorithms', 'en', '2021-04-01', 1312, 1400,
     N'Sách giáo trình thu?t toán kinh ?i?n',
     N'Cu?n sách v? thu?t toán ???c s? d?ng r?ng rãi trong các tr??ng ??i h?c trên th? gi?i.',
     850000, 720000, 500000, 'hardcover', 24.0, 18.0, 6.0, 4),
    
    ('9780062316110', N'Sapiens: L??c S? Loài Ng??i', 'vi', '2020-07-20', 544, 520,
     N'Cu?n sách v? l?ch s? loài ng??i gây s?t toàn c?u',
     N'Sapiens là m?t tác ph?m phi h? c?u c?a tác gi? ng??i Israel Yuval Noah Harari.',
     220000, 185000, 125000, 'paperback', 22.0, 15.0, 3.0, 3),
    
    ('9780062315007', N'Nhà Gi? Kim', 'vi', '2019-03-15', 227, 200,
     N'Tác ph?m v?n h?c n?i ti?ng c?a Paulo Coelho',
     N'Nhà gi? kim là m?t ti?u thuy?t c?a nhà v?n Brazil Paulo Coelho xu?t b?n l?n ??u n?m 1988.',
     110000, 88000, 55000, 'paperback', 19.5, 13.0, 1.3, 3);
GO

-- ============================================
-- 9. Thêm BookAuthors (Liên k?t Sách - Tác gi?)
-- ============================================
INSERT INTO [BookAuthors] ([BookId], [AuthorId])
VALUES 
    (1, 1),  -- M?t Bi?c - Nguy?n Nh?t Ánh
    (2, 1),  -- Tôi Th?y Hoa Vàng - Nguy?n Nh?t Ánh
    (3, 6),  -- 1Q84 - Haruki Murakami
    (4, 4),  -- D?y Con Làm Giàu - Robert Kiyosaki
    (5, 5),  -- 7 Thói Quen - Stephen R. Covey
    (6, 3),  -- ??c Nhân Tâm - Dale Carnegie
    (7, 13), -- Trên ???ng B?ng - Tony Bu?i Sáng
    (8, 7),  -- Harry Potter - J.K. Rowling
    (9, 2),  -- D? Mèn - Tô Hoài
    (11, 12), -- Sapiens - Yuval Noah Harari
    (12, 11); -- Nhà Gi? Kim - Paulo Coelho
GO

-- ============================================
-- 10. Thêm BookTranslators (D?ch gi?)
-- ============================================
INSERT INTO [BookTranslators] ([BookId], [TranslatorId])
VALUES 
    (3, 8),  -- 1Q84 - Nguy?n H?u Trí
    (4, 9),  -- Rich Dad - Lê Minh Qu?c
    (5, 10), -- 7 Habits - Hoàng Anh D?ng
    (6, 8),  -- ??c Nhân Tâm - Nguy?n H?u Trí
    (8, 9),  -- Harry Potter - Lê Minh Qu?c
    (11, 10), -- Sapiens - Hoàng Anh D?ng
    (12, 8);  -- Nhà Gi? Kim - Nguy?n H?u Trí
GO

-- ============================================
-- 11. Thêm BookCategory (Liên k?t Sách - Danh m?c)
-- ============================================
INSERT INTO [BookCategory] ([BooksId], [CategoriesId])
VALUES 
    (1, 6),  -- M?t Bi?c - V?n h?c VN
    (1, 8),  -- M?t Bi?c - Ti?u thuy?t
    (2, 6),  -- Tôi Th?y Hoa Vàng - V?n h?c VN
    (2, 8),  -- Tôi Th?y Hoa Vàng - Ti?u thuy?t
    (3, 7),  -- 1Q84 - V?n h?c n??c ngoài
    (3, 8),  -- 1Q84 - Ti?u thuy?t
    (4, 9),  -- Rich Dad - Qu?n tr?
    (5, 9),  -- 7 Habits - Qu?n tr?
    (6, 11), -- ??c Nhân Tâm - K? n?ng giao ti?p
    (7, 12), -- Trên ???ng B?ng - T? duy
    (8, 13), -- Harry Potter - Truy?n tranh/Thi?u nhi
    (9, 4),  -- D? Mèn - Thi?u nhi
    (10, 14), -- Algorithms - L?p trình
    (11, 12), -- Sapiens - T? duy
    (12, 7);  -- Nhà Gi? Kim - V?n h?c n??c ngoài
GO

-- ============================================
-- 12. Thêm BookImage (Hình ?nh sách)
-- ============================================
INSERT INTO [BookImage] ([ImageUrl], [Caption], [IsPrimary], [BookId])
VALUES 
    ('images/books/mat-biec.jpg', N'Bìa sách M?t Bi?c', 1, 1),
    ('images/books/hoa-vang.jpg', N'Bìa sách Tôi Th?y Hoa Vàng', 1, 2),
    ('images/books/1q84.jpg', N'Bìa sách 1Q84', 1, 3),
    ('images/books/rich-dad.jpg', N'Bìa sách D?y Con Làm Giàu', 1, 4),
    ('images/books/7-habits.jpg', N'Bìa sách 7 Thói Quen', 1, 5),
    ('images/books/dac-nhan-tam.jpg', N'Bìa sách ??c Nhân Tâm', 1, 6),
    ('images/books/tren-duong-bang.jpg', N'Bìa sách Trên ???ng B?ng', 1, 7),
    ('images/books/harry-potter.jpg', N'Bìa sách Harry Potter', 1, 8),
    ('images/books/de-men.jpg', N'Bìa sách D? Mèn', 1, 9),
    ('images/books/algorithms.jpg', N'Bìa sách Algorithms', 1, 10),
    ('images/books/sapiens.jpg', N'Bìa sách Sapiens', 1, 11),
    ('images/books/nha-gia-kim.jpg', N'Bìa sách Nhà Gi? Kim', 1, 12);
GO

-- ============================================
-- 13. Thêm Inventories (T?n kho)
-- ============================================
INSERT INTO [Inventories] ([BookId], [Quantity], [ReservedQuantity], [SoldQuantity], [LastUpdated])
VALUES 
    (1, 150, 0, 45, GETDATE()),
    (2, 200, 5, 80, GETDATE()),
    (3, 50, 2, 15, GETDATE()),
    (4, 180, 8, 120, GETDATE()),
    (5, 120, 3, 65, GETDATE()),
    (6, 300, 10, 250, GETDATE()),
    (7, 100, 4, 35, GETDATE()),
    (8, 250, 15, 180, GETDATE()),
    (9, 90, 0, 40, GETDATE()),
    (10, 30, 1, 8, GETDATE()),
    (11, 140, 6, 95, GETDATE()),
    (12, 220, 12, 160, GETDATE());
GO

-- ============================================
-- 14. Thêm UserAddresses (??a ch? ng??i dùng)
-- ============================================
INSERT INTO [UserAddresses] ([UserId], [ReceiverName], [Phone], [Address], [Province], [District], [Ward], [IsDefault], [CreatedAt])
VALUES 
    (2, N'Tr?n Th? Lan', '0901234567', N'123 Nguy?n Hu?', N'TP. H? Chí Minh', N'Qu?n 1', N'Ph??ng B?n Nghé', 1, GETDATE()),
    (3, N'Lê Minh Tu?n', '0912345678', N'456 Lê L?i', N'Hà N?i', N'Qu?n Hoàn Ki?m', N'Ph??ng Tràng Ti?n', 1, GETDATE()),
    (4, N'Ph?m Thu Hà', '0923456789', N'789 Tr?n Phú', N'?à N?ng', N'Qu?n H?i Châu', N'Ph??ng H?i Châu 1', 1, GETDATE()),
    (5, N'Hoàng V?n Long', '0934567890', N'321 Lý Th??ng Ki?t', N'TP. H? Chí Minh', N'Qu?n 10', N'Ph??ng 15', 1, GETDATE());
GO

-- ============================================
-- 15. Thêm Orders (??n hàng)
-- ============================================
INSERT INTO [Orders] ([UserId], [CustomerName], [Email], [Phone], [ShippingAddress], [Note], 
                       [TotalAmount], [PaymentMethod], [PaymentStatus], [VnpayTransactionId], 
                       [OrderStatus], [CreatedAt], [PaidAt])
VALUES 
    (2, N'Tr?n Th? Lan', 'lan.tran@gmail.com', '0901234567', 
     N'123 Nguy?n Hu?, Ph??ng B?n Nghé, Qu?n 1, TP. H? Chí Minh', N'Giao gi? hành chính',
     255000, 'COD', 'Unpaid', NULL, 'Processing', DATEADD(day, -5, GETDATE()), NULL),
    
    (3, N'Lê Minh Tu?n', 'tuan.le@gmail.com', '0912345678',
     N'456 Lê L?i, Ph??ng Tràng Ti?n, Qu?n Hoàn Ki?m, Hà N?i', N'G?i tr??c khi giao',
     433000, 'VNPay', 'Paid', 'VNP20231215001', 'Delivered', DATEADD(day, -10, GETDATE()), DATEADD(day, -10, GETDATE())),
    
    (4, N'Ph?m Thu Hà', 'ha.pham@gmail.com', '0923456789',
     N'789 Tr?n Phú, Ph??ng H?i Châu 1, Qu?n H?i Châu, ?à N?ng', NULL,
     295000, 'VNPay', 'Paid', 'VNP20231218002', 'Shipping', DATEADD(day, -3, GETDATE()), DATEADD(day, -3, GETDATE())),
    
    (2, N'Tr?n Th? Lan', 'lan.tran@gmail.com', '0901234567',
     N'123 Nguy?n Hu?, Ph??ng B?n Nghé, Qu?n 1, TP. H? Chí Minh', N'Hàng d? v?, nh? tay',
     720000, 'VNPay', 'Paid', 'VNP20231220003', 'Delivered', DATEADD(day, -7, GETDATE()), DATEADD(day, -7, GETDATE()));
GO

-- ============================================
-- 16. Thêm OrderDetails (Chi ti?t ??n hàng)
-- ============================================
INSERT INTO [OrderDetails] ([OrderId], [BookId], [BookTitle], [Price], [Quantity], [Subtotal])
VALUES 
    -- Order 1
    (1, 1, N'M?t Bi?c', 120000, 1, 120000),
    (1, 2, N'Tôi Th?y Hoa Vàng Trên C? Xanh', 135000, 1, 135000),
    
    -- Order 2
    (2, 4, N'D?y Con Làm Giàu', 145000, 2, 290000),
    (2, 7, N'Trên ???ng B?ng', 108000, 1, 108000),
    (2, 9, N'D? Mèn Phiêu L?u Ký', 65000, 1, 65000),
    
    -- Order 3
    (3, 6, N'??c Nhân Tâm', 95000, 2, 190000),
    (3, 12, N'Nhà Gi? Kim', 88000, 1, 88000),
    
    -- Order 4
    (4, 5, N'7 Thói Quen Hi?u Qu?', 165000, 2, 330000),
    (4, 11, N'Sapiens: L??c S? Loài Ng??i', 185000, 2, 370000);
GO

-- ============================================
-- K?T THÚC SCRIPT
-- ============================================

PRINT N'========================================';
PRINT N'?ã t?o d? li?u m?u thành công!';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Users) AS VARCHAR) + N' Users';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Roles) AS VARCHAR) + N' Roles';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Publishers) AS VARCHAR) + N' Publishers';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Categories) AS VARCHAR) + N' Categories';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Persons) AS VARCHAR) + N' Persons (Authors/Translators)';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Books) AS VARCHAR) + N' Books';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Inventories) AS VARCHAR) + N' Inventory Records';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM Orders) AS VARCHAR) + N' Orders';
PRINT N'- ' + CAST((SELECT COUNT(*) FROM OrderDetails) AS VARCHAR) + N' Order Details';
PRINT N'========================================';
GO
