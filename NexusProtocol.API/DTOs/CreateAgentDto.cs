using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NexusProtocol.API.DTOs
{
    public class CreateAgentDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("agent_number")]
        public string? AgentNumber { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("is_featured")]
        public bool IsFeatured { get; set; }

        [JsonPropertyName("abilities")]
        public List<CreateAbilityDto>? Abilities { get; set; }
    }
}