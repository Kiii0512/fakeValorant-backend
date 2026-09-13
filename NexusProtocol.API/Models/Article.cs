using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusProtocol.API.Models
{
    [Table("articles")]
    public class Article
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("subtitle")]
        public string? Subtitle { get; set; }

        [Column("author")]
        public string Author { get; set; } = "VALORANT ESPORTS STAFF";

        [Required]
        [Column("main_image_url")]
        public string MainImageUrl { get; set; } = string.Empty;

        [Column("sub_image_url")]
        public string? SubImageUrl { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("is_featured")]
        public bool IsFeatured { get; set; } = false;

        [Column("published_at")]
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}