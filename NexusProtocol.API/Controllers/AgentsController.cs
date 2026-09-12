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
    public class AgentsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;

        public AgentsController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpFactory.CreateClient("supabase");

            var agentsResp = await client.GetAsync("agents");
            if (!agentsResp.IsSuccessStatusCode) return StatusCode((int)agentsResp.StatusCode);

            var agentsJson = await agentsResp.Content.ReadAsStringAsync();
            var agents = JsonSerializer.Deserialize<List<AgentModel>>(agentsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AgentModel>();

            var abilitiesResp = await client.GetAsync("abilities");
            var abilities = new List<JsonElement>();
            if (abilitiesResp.IsSuccessStatusCode)
            {
                var abilitiesJson = await abilitiesResp.Content.ReadAsStringAsync();
                abilities = JsonSerializer.Deserialize<List<JsonElement>>(abilitiesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<JsonElement>();
            }

            var result = agents.Select(a => new
            {
                a.Id,
                a.Name,
                a.AgentNumber,
                a.Role,
                a.Bio,
                a.AvatarUrl,
                a.IsFeatured,
                a.CreatedAt,
                Abilities = abilities.Where(x =>
                {
                    if (x.TryGetProperty("agent_id", out var v))
                    {
                        return v.GetString() == a.Id;
                    }
                    return false;
                })
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var client = _httpFactory.CreateClient("supabase");

            var agentResp = await client.GetAsync($"agents?id=eq.{id}");
            if (!agentResp.IsSuccessStatusCode) return StatusCode((int)agentResp.StatusCode);

            var agentJson = await agentResp.Content.ReadAsStringAsync();
            var agents = JsonSerializer.Deserialize<List<AgentModel>>(agentJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AgentModel>();
            var agent = agents.FirstOrDefault();
            if (agent == null) return NotFound();

            var abilitiesResp = await client.GetAsync($"abilities?agent_id=eq.{agent.Id}&order=slot_key.asc");
            var abilitiesJson = await abilitiesResp.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var abilities = abilitiesResp.IsSuccessStatusCode ? JsonSerializer.Deserialize<List<object>>(abilitiesJson, options) : new List<object>();

            return Ok(new { Agent = agent, Abilities = abilities });
        }
    }
}