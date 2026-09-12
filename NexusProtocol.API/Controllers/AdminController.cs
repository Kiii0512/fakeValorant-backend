using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexusProtocol.API.DTOs;

namespace NexusProtocol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;

        public AdminController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile? file, [FromQuery] string bucket = "agent-media")
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có file nào được chọn.");

            var client = _httpFactory.CreateClient("supabase");

            // Lấy host gốc (loại bỏ /rest/v1 nếu có trong BaseAddress)
            var rawBase = client.BaseAddress?.ToString().TrimEnd('/') ?? "";
            var rootOrigin = rawBase.Replace("/rest/v1", "").TrimEnd('/');

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".png";
            var fileName = $"{Guid.NewGuid():N}{ext}";

            using var stream = file.OpenReadStream();
            using var content = new StreamContent(stream);
            var mediaType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
            content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            // Gửi chính xác đến endpoint Storage của Supabase
            var storageEndpoint = $"{rootOrigin}/storage/v1/object/{bucket}/{fileName}";
            var uploadResp = await client.PostAsync(storageEndpoint, content);

            if (!uploadResp.IsSuccessStatusCode)
            {
                var err = await uploadResp.Content.ReadAsStringAsync();
                return StatusCode((int)uploadResp.StatusCode, err);
            }

            var publicUrl = $"{rootOrigin}/storage/v1/object/public/{bucket}/{fileName}";
            return Ok(new { Url = publicUrl });
        }

        [HttpPost("agents")]
        public async Task<IActionResult> CreateAgent([FromBody] CreateAgentDto dto)
        {
            if (dto == null) return BadRequest();
            var client = _httpFactory.CreateClient("supabase");

            var agentId = string.IsNullOrWhiteSpace(dto.Id) ? Guid.NewGuid().ToString() : dto.Id.Trim().ToLower();

            var agentPayload = new
            {
                id = agentId,
                name = dto.Name,
                agent_number = dto.AgentNumber,
                role = dto.Role,
                bio = dto.Bio,
                avatar_url = dto.AvatarUrl,
                is_featured = dto.IsFeatured
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "agents")
            {
                Content = new StringContent(JsonSerializer.Serialize(agentPayload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Prefer", "resolution=merge-duplicates,return=representation");

            var agentResp = await client.SendAsync(request);
            if (!agentResp.IsSuccessStatusCode)
            {
                var err = await agentResp.Content.ReadAsStringAsync();
                return StatusCode((int)agentResp.StatusCode, err);
            }

            if (dto.Abilities != null && dto.Abilities.Count > 0)
            {
                await client.DeleteAsync($"abilities?agent_id=eq.{agentId}");

                foreach (var a in dto.Abilities)
                {
                    var abilityPayload = new
                    {
                        agent_id = agentId,
                        slot_key = a.SlotKey,
                        name = a.Name,
                        description = a.Description,
                        icon_url = a.IconUrl,
                        video_url = a.VideoUrl
                    };

                    await client.PostAsync("abilities", new StringContent(JsonSerializer.Serialize(abilityPayload), Encoding.UTF8, "application/json"));
                }
            }

            return Ok(new { AgentId = agentId, Message = "Lưu đặc vụ thành công" });
        }

        [HttpPost("maps")]
        public async Task<IActionResult> CreateMap([FromBody] CreateMapDto dto)
        {
            if (dto == null) return BadRequest();
            var client = _httpFactory.CreateClient("supabase");

            var mapId = string.IsNullOrWhiteSpace(dto.Id) ? Guid.NewGuid().ToString() : dto.Id.Trim().ToLower();

            var payload = new
            {
                id = mapId,
                name = dto.Name,
                location = dto.Location,
                coordinates = dto.Coordinates,
                image_url = dto.ImageUrl,
                notes = dto.Notes,
                is_featured = dto.IsFeatured
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "maps")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Prefer", "resolution=merge-duplicates,return=representation");

            var resp = await client.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { error = err });
            }

            if (dto.Gallery != null && dto.Gallery.Count > 0)
            {
                await client.DeleteAsync($"map_images?map_id=eq.{mapId}");

                int order = 0;
                foreach (var imgUrl in dto.Gallery)
                {
                    if (string.IsNullOrWhiteSpace(imgUrl)) continue;

                    var imgPayload = new
                    {
                        map_id = mapId,
                        image_url = imgUrl,
                        display_order = order++
                    };

                    await client.PostAsync("map_images", new StringContent(JsonSerializer.Serialize(imgPayload), Encoding.UTF8, "application/json"));
                }
            }

            return Ok(new { MapId = mapId, Message = "Lưu bản đồ và thư viện ảnh thành công" });
        }

        [HttpPost("weapons")]
        public async Task<IActionResult> CreateWeapon([FromBody] CreateWeaponDto dto)
        {
            if (dto == null) return BadRequest();
            var client = _httpFactory.CreateClient("supabase");

            var weaponId = string.IsNullOrWhiteSpace(dto.Id) ? Guid.NewGuid().ToString() : dto.Id.Trim().ToLower();

            var payload = new
            {
                id = weaponId,
                name = dto.Name,
                category = dto.Category,
                creds = dto.Creds,
                image_url = dto.ImageUrl,
                killfeed_icon = dto.KillfeedIcon,
                wall_penetration = dto.WallPenetration,
                fire_mode = dto.FireMode,
                fire_rate = dto.FireRate,
                run_speed = dto.RunSpeed,
                equip_speed = dto.EquipSpeed,
                reload_speed = dto.ReloadSpeed,
                magazine_size = dto.MagazineSize,
                reserve_ammo = dto.ReserveAmmo,
                damage_tiers = dto.DamageTiers ?? new List<DamageTierDto>(),
                alt_fire_function = dto.AltFireFunction,
                alt_fire_zoom = dto.AltFireZoom,
                is_featured = dto.IsFeatured
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "weapons")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Prefer", "resolution=merge-duplicates,return=representation");

            var resp = await client.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { error = err });
            }

            return Ok(new { WeaponId = weaponId, Message = "Lưu thông tin vũ khí thành công" });
        }
    }
}