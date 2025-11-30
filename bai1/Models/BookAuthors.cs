namespace bai1.Models
{
    public class BookAuthors
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public Book book { get; set; }

        public Person Author { get; set; }
    }
}
