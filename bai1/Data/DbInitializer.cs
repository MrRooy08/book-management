using bai1.Models;
using Microsoft.EntityFrameworkCore;

namespace bai1.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Đảm bảo database đã được tạo
            context.Database.EnsureCreated();

            // Seed Roles trước (luôn kiểm tra và tạo nếu chưa có)
            SeedRoles(context);

            // Seed Admin user (luôn kiểm tra và tạo nếu chưa có)
            SeedAdminUser(context);

            // Kiểm tra xem đã có dữ liệu sách chưa
            if (context.Books.Any())
            {
                return; // Database đã được seed
            }

            // Seed Publishers
            var publishers = new Publisher[]
            {
                new Publisher
                {
                    Name = "Nhà Xuất Bản Trẻ",
                    Address = "161B Lý Chính Thắng, Phường 7, Quận 3, TP.HCM",
                    Phone = "02839316211",
                    Email = "info@nxbtrẻ.com.vn"
                },
                new Publisher
                {
                    Name = "Nhà Xuất Bản Kim Đồng",
                    Address = "55 Quang Trung, Hai Bà Trưng, Hà Nội",
                    Phone = "02439434730",
                    Email = "info@nxbkimdong.com.vn"
                },
                new Publisher
                {
                    Name = "Nhà Xuất Bản Văn Học",
                    Address = "18 Nguyễn Trường Tộ, Ba Đình, Hà Nội",
                    Phone = "02438221347",
                    Email = "nxbvanhoc@vnn.vn"
                },
                new Publisher
                {
                    Name = "Alpha Books",
                    Address = "59B Nguyễn Khắc Hiếu, Trúc Bạch, Ba Đình, Hà Nội",
                    Phone = "02437155555",
                    Email = "info@alphabooks.vn"
                }
            };

            context.Publishers.AddRange(publishers);
            context.SaveChanges();

            // Seed Categories
            var categories = new Category[]
            {
                new Category { Name = "Văn Học", Slug = "van-hoc" },
                new Category { Name = "Khoa Học", Slug = "khoa-hoc" },
                new Category { Name = "Kinh Tế", Slug = "kinh-te" },
                new Category { Name = "Lịch Sử", Slug = "lich-su" },
                new Category { Name = "Tiểu Thuyết", Slug = "tieu-thuyet", ParentId = 1 },
                new Category { Name = "Truyện Ngắn", Slug = "truyen-ngan", ParentId = 1 },
                new Category { Name = "Thơ", Slug = "tho", ParentId = 1 },
                new Category { Name = "Khoa Học Tự Nhiên", Slug = "khoa-hoc-tu-nhien", ParentId = 2 },
                new Category { Name = "Khoa Học Xã Hội", Slug = "khoa-hoc-xa-hoi", ParentId = 2 }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Reload categories to ensure they have proper IDs
            var savedCategories = context.Categories.ToList();

            // Seed Authors
            var authors = new Person[]
            {
                new Person { Name = "Nguyễn Nhật Ánh", Description = "Nhà văn Việt Nam chuyên viết cho tuổi mới lớn" },
                new Person { Name = "Haruki Murakami", Description = "Nhà văn Nhật Bản nổi tiếng" },
                new Person { Name = "Paulo Coelho", Description = "Nhà văn Brazil, tác giả của Nhà giả kim" },
                new Person { Name = "Nguyễn Du", Description = "Đại thi hào dân tộc Việt Nam" },
                new Person { Name = "Ngô Tất Tố", Description = "Nhà văn hiện thực phê phán Việt Nam" },
                new Person { Name = "Nam Cao", Description = "Nhà văn hiện thực Việt Nam" }
            };

            context.Persons.AddRange(authors);
            context.SaveChanges();

            // Seed Books
            var books = new Book[]
            {
                new Book
                {
                    ISBN = "9786041001234",
                    Title = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh",
                    ShortDescription = "Câu chuyện về tuổi thơ ở miền quê Việt Nam",
                    Description = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh là một tiểu thuyết dành cho thanh thiếu niên của nhà văn Nguyễn Nhật Ánh, xuất bản lần đầu tại Việt Nam vào năm 2010 bởi Nhà xuất bản Trẻ, với phần tranh minh họa do Đỗ Hoàng Tường thực hiện.",
                    Language = "vi",
                    PublishDate = new DateTime(2010, 1, 1),
                    PageCount = 378,
                    Weight = 350,
                    Format = "paperback",
                    Dimensions = new BookDimensions
                    {
                        Length = 20.5f,
                        Width = 14.5f,
                        Height = 2.0f
                    },
                    ListPrice = 120000,
                    SalePrice = 96000,
                    CostPrice = 60000,
                    PublisherId = publishers[0].Id,
                    Authors = new List<BookAuthors>
                    {
                        new BookAuthors { AuthorId = authors[0].Id }
                    },
                    Images = new List<BookImage>
                    {
                        new BookImage
                        {
                            ImageUrl = "image.jpg",
                            IsPrimary = true
                        }
                    },
                    Inventory = new Inventory { Quantity = 50 }
                },
                new Book
                {
                    ISBN = "9786041001235",
                    Title = "Norwegian Wood",
                    ShortDescription = "Tiểu thuyết tình yêu nổi tiếng của Haruki Murakami",
                    Description = "Norwegian Wood là một tiểu thuyết của nhà văn Nhật Bản Haruki Murakami, xuất bản lần đầu năm 1987. Cuốn sách kể về câu chuyện tình yêu của Toru Watanabe và Naoko.",
                    Language = "vi",
                    PublishDate = new DateTime(1987, 9, 4),
                    PageCount = 296,
                    Weight = 280,
                    Format = "paperback",
                    Dimensions = new BookDimensions
                    {
                        Length = 20.0f,
                        Width = 13.0f,
                        Height = 1.8f
                    },
                    ListPrice = 150000,
                    SalePrice = 120000,
                    CostPrice = 80000,
                    PublisherId = publishers[3].Id,
                    Authors = new List<BookAuthors>
                    {
                        new BookAuthors { AuthorId = authors[1].Id }
                    },
                    Images = new List<BookImage>
                    {
                        new BookImage
                        {
                            ImageUrl = "image.jpg",
                            IsPrimary = true
                        }
                    },
                    Inventory = new Inventory { Quantity = 30 }
                },
                new Book
                {
                    ISBN = "9786041001236",
                    Title = "Nhà Giả Kim",
                    ShortDescription = "Cuốn sách truyền cảm hứng về hành trình tìm kiếm vận mệnh",
                    Description = "Nhà Giả Kim là một cuốn tiểu thuyết của nhà văn Paulo Coelho, xuất bản lần đầu năm 1988. Câu chuyện kể về Santiago, một cậu bé chăn cừu người Andalusia, trong chuyến phiêu lưu đến Ai Cập để tìm kho báu.",
                    Language = "vi",
                    PublishDate = new DateTime(1988, 4, 25),
                    PageCount = 163,
                    Weight = 200,
                    Format = "paperback",
                    Dimensions = new BookDimensions
                    {
                        Length = 19.0f,
                        Width = 12.5f,
                        Height = 1.5f
                    },
                    ListPrice = 80000,
                    SalePrice = 64000,
                    CostPrice = 40000,
                    PublisherId = publishers[3].Id,
                    Authors = new List<BookAuthors>
                    {
                        new BookAuthors { AuthorId = authors[2].Id }
                    },
                    Images = new List<BookImage>
                    {
                        new BookImage
                        {
                            ImageUrl = "image.jpg",
                            IsPrimary = true
                        }
                    },
                    Inventory = new Inventory { Quantity = 75 }
                },
                new Book
                {
                    ISBN = "9786041001237",
                    Title = "Truyện Kiều",
                    ShortDescription = "Kiệt tác văn học Việt Nam của đại thi hào Nguyễn Du",
                    Description = "Truyện Kiều là một truyện thơ Nôm của đại thi hào Nguyễn Du. Đây được xem là truyện thơ nổi tiếng nhất và xét vào hàng kinh điển trong văn học Việt Nam.",
                    Language = "vi",
                    PublishDate = new DateTime(1820, 1, 1),
                    PageCount = 3254,
                    Weight = 500,
                    Format = "hardcover",
                    Dimensions = new BookDimensions
                    {
                        Length = 24.0f,
                        Width = 16.0f,
                        Height = 3.5f
                    },
                    ListPrice = 200000,
                    SalePrice = 160000,
                    CostPrice = 100000,
                    PublisherId = publishers[2].Id,
                    Authors = new List<BookAuthors>
                    {
                        new BookAuthors { AuthorId = authors[3].Id }
                    },
                    Images = new List<BookImage>
                    {
                        new BookImage
                        {
                            ImageUrl = "image.jpg",
                            IsPrimary = true
                        }
                    },
                    Inventory = new Inventory { Quantity = 25 }
                },
                new Book
                {
                    ISBN = "9786041001238",
                    Title = "Tắt Đèn",
                    ShortDescription = "Tiểu thuyết hiện thực phê phán của Ngô Tất Tố",
                    Description = "Tắt Đèn là một tiểu thuyết của nhà văn Ngô Tất Tố, xuất bản lần đầu năm 1939. Tác phẩm phản ánh hiện thực xã hội Việt Nam đầu thế kỷ 20.",
                    Language = "vi",
                    PublishDate = new DateTime(1939, 1, 1),
                    PageCount = 232,
                    Weight = 250,
                    Format = "paperback",
                    Dimensions = new BookDimensions
                    {
                        Length = 20.0f,
                        Width = 14.0f,
                        Height = 1.5f
                    },
                    ListPrice = 90000,
                    SalePrice = 72000,
                    CostPrice = 45000,
                    PublisherId = publishers[2].Id,
                    Authors = new List<BookAuthors>
                    {
                        new BookAuthors { AuthorId = authors[4].Id }
                    },
                    Images = new List<BookImage>
                    {
                        new BookImage
                        {
                            ImageUrl = "image.jpg",
                            IsPrimary = true
                        }
                    },
                    Inventory = new Inventory { Quantity = 40 }
                },
                new Book
                {
                    ISBN = "9786041001239",
                    Title = "Chí Phèo",
                    ShortDescription = "Tập truyện ngắn nổi tiếng của Nam Cao",
                    Description = "Chí Phèo là một truyện ngắn của nhà văn Nam Cao, được coi là một trong những tác phẩm văn học hiện thực xuất sắc nhất của văn học Việt Nam thế kỷ 20.",
                    Language = "vi",
                    PublishDate = new DateTime(1941, 1, 1),
                    PageCount = 180,
                    Weight = 200,
                    Format = "paperback",
                    Dimensions = new BookDimensions
                    {
                        Length = 19.5f,
                        Width = 13.5f,
                        Height = 1.2f
                    },
                    ListPrice = 70000,
                    SalePrice = 56000,
                    CostPrice = 35000,
                    PublisherId = publishers[2].Id,
                    Authors = new List<BookAuthors>
                    {
                        new BookAuthors { AuthorId = authors[5].Id }
                    },
                    Images = new List<BookImage>
                    {
                        new BookImage
                        {
                            ImageUrl = "image.jpg",
                            IsPrimary = true
                        }
                    },
                    Inventory = new Inventory { Quantity = 60 }
                }
            };

            // Gán categories cho books (sử dụng savedCategories để đảm bảo có ID)
            books[0].Categories.Add(savedCategories[4]); // Tiểu Thuyết
            books[1].Categories.Add(savedCategories[4]); // Tiểu Thuyết
            books[2].Categories.Add(savedCategories[4]); // Tiểu Thuyết
            books[3].Categories.Add(savedCategories[6]); // Thơ
            books[4].Categories.Add(savedCategories[4]); // Tiểu Thuyết
            books[5].Categories.Add(savedCategories[5]); // Truyện Ngắn

            context.Books.AddRange(books);
            context.SaveChanges();
        }

        /// <summary>
        /// Tạo các Roles mặc định nếu chưa có
        /// </summary>
        private static void SeedRoles(ApplicationDbContext context)
        {
            var roles = new[]
            {
                new { Name = "Admin", Description = "Quản trị viên hệ thống - Toàn quyền" },
                new { Name = "Staff", Description = "Nhân viên - Quản lý đơn hàng và sách" },
                new { Name = "User", Description = "Người dùng thông thường - Mua hàng" }
            };

            foreach (var roleInfo in roles)
            {
                if (!context.Roles.Any(r => r.RoleName == roleInfo.Name))
                {
                    context.Roles.Add(new Role
                    {
                        RoleName = roleInfo.Name,
                        RoleDescription = roleInfo.Description
                    });
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Tạo tài khoản Admin mặc định nếu chưa có
        /// </summary>
        private static void SeedAdminUser(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có admin chưa
            var adminEmail = "admin@bookstore.com";
            var existingAdmin = context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Email == adminEmail);

            if (existingAdmin == null)
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
                if (adminRole != null)
                {
                    var adminUser = new User
                    {
                        Name = "Administrator",
                        Email = adminEmail,
                        Password = "admin123", // Trong thực tế nên hash password
                        BirthDay = new DateTime(1990, 1, 1),
                        Roles = new List<Role> { adminRole }
                    };

                    context.Users.Add(adminUser);
                    context.SaveChanges();

                    Console.WriteLine("===========================================");
                    Console.WriteLine("  TÀI KHOẢN ADMIN MẶC ĐỊNH ĐÃ ĐƯỢC TẠO");
                    Console.WriteLine("  Email: admin@bookstore.com");
                    Console.WriteLine("  Password: admin123");
                    Console.WriteLine("===========================================");
                }
            }
        }
    }
}

