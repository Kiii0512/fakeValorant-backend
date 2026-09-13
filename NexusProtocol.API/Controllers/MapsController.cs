using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexusProtocol.API.Models;

namespace NexusProtocol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;

        public MapsController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpFactory.CreateClient("supabase");
            var mapsResp = await client.GetAsync("maps?order=created_at.desc");
            if (!mapsResp.IsSuccessStatusCode) return StatusCode((int)mapsResp.StatusCode);

            var mapsJson = await mapsResp.Content.ReadAsStringAsync();
            var maps = JsonSerializer.Deserialize<List<MapModel>>(mapsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MapModel>();

            var imagesResp = await client.GetAsync("map_images?order=display_order.asc");
            var images = new List<JsonElement>();
            if (imagesResp.IsSuccessStatusCode)
            {
                var imagesJson = await imagesResp.Content.ReadAsStringAsync();
                images = JsonSerializer.Deserialize<List<JsonElement>>(imagesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<JsonElement>();
            }

            var result = maps.Select(m =>
            {
                var mapGallery = images
                    .Where(img => img.TryGetProperty("map_id", out var mid) && string.Equals(mid.GetString()?.Trim(), m.Id?.Trim(), StringComparison.OrdinalIgnoreCase))
                    .Select(img => img.TryGetProperty("image_url", out var url) ? url.GetString() : null)
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .ToList();

                if (mapGallery.Count == 0 && !string.IsNullOrEmpty(m.ImageUrl))
                    mapGallery.Add(m.ImageUrl);

                return new
                {
                    m.Id,
                    m.Name,
                    m.Location,
                    m.Coordinates,
                    m.Notes,
                    m.ImageUrl,
                    m.IsFeatured,
                    m.CreatedAt,
                    Gallery = mapGallery
                };
            });

            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedRotation()
        {
            var client = _httpFactory.CreateClient("supabase");
            // Lấy danh sách map đang bật cờ xoay tua
            var mapsResp = await client.GetAsync("maps?is_featured=eq.true");
            if (!mapsResp.IsSuccessStatusCode) return StatusCode((int)mapsResp.StatusCode);

            var mapsJson = await mapsResp.Content.ReadAsStringAsync();
            var maps = JsonSerializer.Deserialize<List<MapModel>>(mapsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MapModel>();

            // Random 3 bản đồ bất kỳ mỗi lượt request
            var randomThree = maps.OrderBy(_ => Guid.NewGuid()).Take(3).ToList();
            return Ok(randomThree);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var client = _httpFactory.CreateClient("supabase");
            var mapResp = await client.GetAsync($"maps?id=eq.{id}");
            if (!mapResp.IsSuccessStatusCode) return StatusCode((int)mapResp.StatusCode);

            var mapJson = await mapResp.Content.ReadAsStringAsync();
            var maps = JsonSerializer.Deserialize<List<MapModel>>(mapJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MapModel>();
            var map = maps.FirstOrDefault();
            if (map == null) return NotFound();

            var imgResp = await client.GetAsync($"map_images?map_id=eq.{id}&order=display_order.asc");
            var imgJson = await imgResp.Content.ReadAsStringAsync();
            var images = imgResp.IsSuccessStatusCode ? JsonSerializer.Deserialize<List<JsonElement>>(imgJson) : new List<JsonElement>();

            var gallery = images?
                .Select(x => x.TryGetProperty("image_url", out var u) ? u.GetString() : null)
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .ToList() ?? new List<string?>();

            if (gallery.Count == 0 && !string.IsNullOrEmpty(map.ImageUrl))
                gallery.Add(map.ImageUrl);

            return Ok(new { Map = map, Gallery = gallery });
        }
    }
}