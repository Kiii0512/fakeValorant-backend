using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NexusProtocol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeaponsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;

        public WeaponsController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpFactory.CreateClient("supabase");
            var resp = await client.GetAsync("weapons?order=created_at.desc");
            if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode);

            var json = await resp.Content.ReadAsStringAsync();
            // Trả trực tiếp Content JSON từ Supabase ra để giữ nguyên vẹn cấu trúc mảng damage_tiers
            return Content(json, "application/json");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var client = _httpFactory.CreateClient("supabase");
            var resp = await client.GetAsync($"weapons?id=eq.{id}");
            if (!resp.IsSuccessStatusCode) return StatusCode((int)resp.StatusCode);

            var json = await resp.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
    }
}