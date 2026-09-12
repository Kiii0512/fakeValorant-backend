using System.Text.Json.Serialization;

namespace NexusProtocol.API.DTOs
{
    public class CreateAbilityDto
    {
        [JsonPropertyName("slot_key")]
        public string? SlotKey { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon_url")]
        public string? IconUrl { get; set; }

        [JsonPropertyName("video_url")]
        public string? VideoUrl { get; set; }
    }
}
