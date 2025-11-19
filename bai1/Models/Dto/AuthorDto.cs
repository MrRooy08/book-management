using System.Text.Json.Serialization;

namespace bai1.Models.Dto
{
    public class AuthorDto
    {
        [JsonPropertyName("authorId")]
        public int Id { get; set; }

        [JsonPropertyName("authorName")]
        public string Name { get; set; }
    }
}
