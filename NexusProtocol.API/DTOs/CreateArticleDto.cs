namespace NexusProtocol.API.DTOs
{
    public class CreateArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Author { get; set; } = "VALORANT ESPORTS STAFF";
        public string MainImageUrl { get; set; } = string.Empty;
        public string? SubImageUrl { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}