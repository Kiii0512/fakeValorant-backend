using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NexusProtocol.API.DTOs
{
    public class CreateMapDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("coordinates")]
        public string? Coordinates { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("is_featured")]
        public bool IsFeatured { get; set; }

        // Danh sách URL ảnh album (bao nhiêu ảnh tùy thích)
        [JsonPropertyName("gallery")]
        public List<string>? Gallery { get; set; }
    }
}