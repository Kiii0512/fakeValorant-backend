using System.Text.Json.Serialization;
using Postgrest.Attributes;
using Postgrest.Models;

namespace NexusProtocol.API.Models
{
    [Table("abilities")]
    public class AbilityModel : BaseModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("agent_id")]
        public int AgentId { get; set; }

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
