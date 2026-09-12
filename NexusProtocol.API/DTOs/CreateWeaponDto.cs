using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NexusProtocol.API.DTOs
{
    public class DamageTierDto
    {
        [JsonPropertyName("range")]
        public string Range { get; set; } = string.Empty;

        [JsonPropertyName("head")]
        public int Head { get; set; }

        [JsonPropertyName("body")]
        public int Body { get; set; }

        [JsonPropertyName("leg")]
        public int Leg { get; set; }
    }

    public class CreateWeaponDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("creds")]
        public int Creds { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("killfeed_icon")]
        public string? KillfeedIcon { get; set; }

        [JsonPropertyName("wall_penetration")]
        public string? WallPenetration { get; set; }

        [JsonPropertyName("fire_mode")]
        public string? FireMode { get; set; }

        [JsonPropertyName("fire_rate")]
        public double FireRate { get; set; }

        [JsonPropertyName("run_speed")]
        public string? RunSpeed { get; set; }

        [JsonPropertyName("equip_speed")]
        public string? EquipSpeed { get; set; }

        [JsonPropertyName("reload_speed")]
        public string? ReloadSpeed { get; set; }

        [JsonPropertyName("magazine_size")]
        public int MagazineSize { get; set; }

        [JsonPropertyName("reserve_ammo")]
        public string? ReserveAmmo { get; set; }

        // Danh sách nhiều cự ly sát thương
        [JsonPropertyName("damage_tiers")]
        public List<DamageTierDto>? DamageTiers { get; set; }

        [JsonPropertyName("alt_fire_function")]
        public string? AltFireFunction { get; set; }

        [JsonPropertyName("alt_fire_zoom")]
        public string? AltFireZoom { get; set; }

        [JsonPropertyName("is_featured")]
        public bool IsFeatured { get; set; }
    }
}