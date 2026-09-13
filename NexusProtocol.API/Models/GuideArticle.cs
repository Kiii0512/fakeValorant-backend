using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusProtocol.API.Models
{
    [Table("guide_articles")]
    public class GuideArticle
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("subtitle")]
        public string? Subtitle { get; set; }

        [Column("banner_image_url")]
        public string BannerImageUrl { get; set; } = string.Empty;

        [Column("author")]
        public string Author { get; set; } = "VALORANT ESPORTS STAFF";

        [Column("published_at")]
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        [Column("chapters", TypeName = "jsonb")]
        public string ChaptersJson { get; set; } = "[]";
    }
}