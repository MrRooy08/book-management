using System;
using Microsoft.EntityFrameworkCore;

namespace bai1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
            
        }

        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<Book> Books { get; set; } = default!;
        public DbSet<Person> Persons { get; set; } = default!;
        public DbSet<Publisher> Publishers { get; set; } = default!;
        public DbSet<Inventory> Inventories { get; set; } = default!;

        public DbSet<BookTranslators> BookTranslators { get; set; } = default!;
        public DbSet<BookAuthors> BookAuthors { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa Khóa Chính Tổ hợp cho bảng trung gian BookAuthors
            modelBuilder.Entity<BookAuthors>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId }); // Giả sử BookAuthors có thuộc tính BookId và AuthorId

            // (Tùy chọn) Định nghĩa mối quan hệ Khóa Ngoại rõ ràng hơn
            modelBuilder.Entity<BookAuthors>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.Author) // Giả sử thuộc tính điều hướng trong Book là Author
                .HasForeignKey(ba => ba.BookId);

            // Lặp lại logic tương tự cho BookTranslators nếu cần:
            modelBuilder.Entity<BookTranslators>()
                .HasKey(bt => new { bt.BookId, bt.TranslatorId });

            // ... và các Entity trung gian khác như BookSupplier (nếu có)
        }
    }
}
