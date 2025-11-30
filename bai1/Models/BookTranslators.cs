namespace bai1.Models
{
    public class BookTranslators
    {
        public int BookId { get; set; }
        public int TranslatorId { get; set; }
        public Book book { get; set; }

        public Person Translator { get; set; }
    }
}
