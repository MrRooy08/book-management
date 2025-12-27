-- ============================================
-- Script Insert D? Li?u M?u Nhanh (Quick Insert)
-- Ch?y sau khi ?ã Update-Database thành công
-- ============================================

-- 1. Publishers
INSERT INTO [Publishers] ([Name], [Address], [Phone], [Email], [Slug])
VALUES 
    (N'NXB Tr?', N'161B Lý Chính Th?ng, Q3, TP.HCM', '0283932343', 'nxbtre@gmail.com', 'nxb-tre'),
    (N'NXB Kim ??ng', N'55 Quang Trung, Hà N?i', '0243943344', 'kimdong@gmail.com', 'nxb-kim-dong'),
    (N'NXB V?n H?c', N'18 Nguy?n Tr??ng T?, Hà N?i', '0438222920', 'vanhoc@gmail.com', 'nxb-van-hoc'),
    (N'First News', N'195 ?i?n Biên Ph?, Bình Th?nh, HCM', '0283822896', 'firstnews@gmail.com', 'first-news'),
    (N'Alphabooks', N'67/1 Lý Chính Th?ng, Q3, HCM', '0283520794', 'alphabooks@gmail.com', 'alphabooks');

-- 2. Categories
INSERT INTO [Categories] ([Name], [Slug], [ParentId]) VALUES 
(N'V?n h?c', 'van-hoc', NULL),
(N'Kinh t?', 'kinh-te', NULL),
(N'K? n?ng s?ng', 'ky-nang-song', NULL),
(N'Thi?u nhi', 'thieu-nhi', NULL),
(N'Công ngh?', 'cong-nghe', NULL);

INSERT INTO [Categories] ([Name], [Slug], [ParentId]) VALUES 
(N'V?n h?c Vi?t Nam', 'van-hoc-viet-nam', 1),
(N'V?n h?c n??c ngoài', 'van-hoc-nuoc-ngoai', 1),
(N'Ti?u thuy?t', 'tieu-thuyet', 1),
(N'Qu?n tr?', 'quan-tri', 2),
(N'Marketing', 'marketing', 2),
(N'K? n?ng giao ti?p', 'ky-nang-giao-tiep', 3),
(N'T? duy', 'tu-duy', 3),
(N'Truy?n tranh', 'truyen-tranh', 4),
(N'L?p trình', 'lap-trinh', 5);

-- 3. Persons (Authors & Translators)
INSERT INTO [Persons] ([Name], [Description]) VALUES 
(N'Nguy?n Nh?t Ánh', N'Nhà v?n Vi?t Nam'),
(N'Tô Hoài', N'Nhà v?n Vi?t Nam'),
(N'Dale Carnegie', N'Tác gi? sách k? n?ng s?ng'),
(N'Robert Kiyosaki', N'Tác gi? D?y Con Làm Giàu'),
(N'Stephen Covey', N'Tác gi? 7 Thói Quen'),
(N'Haruki Murakami', N'Nhà v?n Nh?t B?n'),
(N'J.K. Rowling', N'Tác gi? Harry Potter'),
(N'Nguy?n H?u Trí', N'D?ch gi?'),
(N'Lê Minh Qu?c', N'D?ch gi?'),
(N'Paulo Coelho', N'Tác gi? Nhà Gi? Kim'),
(N'Yuval Harari', N'Tác gi? Sapiens'),
(N'Tony Bu?i Sáng', N'Tác gi? Vi?t Nam');

-- 4. Books
INSERT INTO [Books] ([ISBN], [Title], [Language], [PublishDate], [PageCount], [Weight], [ShortDescription], 
[Description], [ListPrice], [SalePrice], [CostPrice], [Format], 
[Dimensions_Height], [Dimensions_Width], [Dimensions_Length], [PublisherId])
VALUES 
('9786041138704', N'M?t Bi?c', 'vi', '2020-01-15', 368, 350, 
 N'Tác ph?m v?n h?c hay nh?t', N'M?t bi?c là truy?n dài c?a Nguy?n Nh?t Ánh',
 150000, 120000, 80000, 'paperback', 20.5, 14.5, 2.0, 1),

('9786041138711', N'Tôi Th?y Hoa Vàng Trên C? Xanh', 'vi', '2019-05-20', 400, 380,
 N'Câu chuy?n tu?i th?', N'Truy?n dài c?a Nguy?n Nh?t Ánh',
 160000, 135000, 90000, 'paperback', 20.5, 14.5, 2.2, 1),

('9780062837691', N'D?y Con Làm Giàu', 'vi', '2021-06-01', 336, 320,
 N'Sách v? tài chính cá nhân', N'Rich Dad Poor Dad - Robert Kiyosaki',
 180000, 145000, 95000, 'paperback', 20.0, 14.0, 1.8, 5),

('9780684858395', N'7 Thói Quen Hi?u Qu?', 'vi', '2020-08-15', 432, 400,
 N'K? n?ng làm vi?c hi?u qu?', N'The 7 Habits of Highly Effective People',
 200000, 165000, 110000, 'hardcover', 21.0, 14.5, 2.5, 4),

('9780671027032', N'??c Nhân Tâm', 'vi', '2019-01-10', 320, 300,
 N'K? n?ng giao ti?p', N'How to Win Friends and Influence People',
 120000, 95000, 60000, 'paperback', 20.0, 13.0, 1.6, 1),

('9786041183902', N'Trên ???ng B?ng', 'vi', '2021-11-20', 280, 260,
 N'Phát tri?n s? nghi?p', N'Sách c?a Tony Bu?i Sáng',
 135000, 108000, 70000, 'paperback', 19.0, 13.5, 1.5, 1),

('9780747532699', N'Harry Potter Và Hòn ?á Phù Th?y', 'vi', '2020-02-14', 368, 340,
 N'Ph?n 1 Harry Potter', N'Harry Potter and the Philosopher Stone',
 170000, 140000, 95000, 'paperback', 20.5, 14.0, 2.0, 2),

('9786041161191', N'D? Mèn Phiêu L?u Ký', 'vi', '2018-09-05', 196, 180,
 N'V?n h?c thi?u nhi VN', N'Tác ph?m c?a Tô Hoài',
 80000, 65000, 40000, 'paperback', 19.0, 13.0, 1.2, 2),

('9780062315007', N'Nhà Gi? Kim', 'vi', '2019-03-15', 227, 200,
 N'Ti?u thuy?t n?i ti?ng', N'The Alchemist - Paulo Coelho',
 110000, 88000, 55000, 'paperback', 19.5, 13.0, 1.3, 3),

('9780062316110', N'Sapiens: L??c S? Loài Ng??i', 'vi', '2020-07-20', 544, 520,
 N'L?ch s? loài ng??i', N'Sapiens - Yuval Noah Harari',
 220000, 185000, 125000, 'paperback', 22.0, 15.0, 3.0, 3);

-- 5. BookAuthors
INSERT INTO [BookAuthors] ([BookId], [AuthorId]) VALUES 
(1, 1), (2, 1), (3, 4), (4, 5), (5, 3), (6, 12), (7, 7), (8, 2), (9, 10), (10, 11);

-- 6. BookTranslators
INSERT INTO [BookTranslators] ([BookId], [TranslatorId]) VALUES 
(3, 8), (4, 9), (5, 8), (7, 9), (9, 8), (10, 9);

-- 7. BookCategory
INSERT INTO [BookCategory] ([BooksId], [CategoriesId]) VALUES 
(1, 6), (1, 8), (2, 6), (2, 8), (3, 9), (4, 9), (5, 11), (6, 12), (7, 13), (8, 4), (9, 7), (10, 12);

-- 8. BookImage
INSERT INTO [BookImage] ([ImageUrl], [Caption], [IsPrimary], [BookId]) VALUES 
('/images/books/mat-biec.jpg', N'M?t Bi?c', 1, 1),
('/images/books/hoa-vang.jpg', N'Hoa Vàng', 1, 2),
('/images/books/rich-dad.jpg', N'Rich Dad', 1, 3),
('/images/books/7-habits.jpg', N'7 Habits', 1, 4),
('/images/books/dac-nhan-tam.jpg', N'??c Nhân Tâm', 1, 5),
('/images/books/tren-duong-bang.jpg', N'Trên ???ng B?ng', 1, 6),
('/images/books/harry-potter.jpg', N'Harry Potter', 1, 7),
('/images/books/de-men.jpg', N'D? Mèn', 1, 8),
('/images/books/nha-gia-kim.jpg', N'Nhà Gi? Kim', 1, 9),
('/images/books/sapiens.jpg', N'Sapiens', 1, 10);

-- 9. Inventories
INSERT INTO [Inventories] ([BookId], [Quantity], [ReservedQuantity], [SoldQuantity], [LastUpdated]) VALUES 
(1, 150, 0, 45, GETDATE()),
(2, 200, 5, 80, GETDATE()),
(3, 180, 8, 120, GETDATE()),
(4, 120, 3, 65, GETDATE()),
(5, 300, 10, 250, GETDATE()),
(6, 100, 4, 35, GETDATE()),
(7, 250, 15, 180, GETDATE()),
(8, 90, 0, 40, GETDATE()),
(9, 220, 12, 160, GETDATE()),
(10, 140, 6, 95, GETDATE());

-- 10. Roles
INSERT INTO [Roles] ([RoleName], [RoleDescription]) VALUES 
(N'Admin', N'Qu?n tr? viên'),
(N'Customer', N'Khách hàng'),
(N'Staff', N'Nhân viên');

-- 11. Users
INSERT INTO [Users] ([Name], [Email], [BirthDay], [Password]) VALUES 
(N'Admin', 'admin@bookstore.com', '1990-01-15', 'admin123'),
(N'Tr?n Th? Lan', 'lan.tran@gmail.com', '1995-05-20', 'password123'),
(N'Lê Minh Tu?n', 'tuan.le@gmail.com', '1998-08-10', 'password123'),
(N'Ph?m Thu Hà', 'ha.pham@gmail.com', '2000-03-25', 'password123');

-- 12. RoleUser
INSERT INTO [RoleUser] ([RolesId], [UsersId]) VALUES 
(1, 1), (2, 2), (2, 3), (2, 4);

PRINT N'? ?ã insert d? li?u m?u thành công!';
