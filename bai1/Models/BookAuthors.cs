namespace bai1.Models
{
    public class BookAuthors
    {
        public int BookId { get; set; }
        public Book Book{ get; set; }

        public int AuthorId { get; set; }
        public Person Author { get; set; }

        public bool IsMainAuthor { get; set; }

    }
}
