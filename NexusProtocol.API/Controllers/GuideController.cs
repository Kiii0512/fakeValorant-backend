using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusProtocol.API.Data;
using NexusProtocol.API.Models;

namespace NexusProtocol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuideController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuideController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestGuide()
        {
            var guide = await _context.GuideArticles
                .OrderByDescending(g => g.PublishedAt)
                .FirstOrDefaultAsync();

            if (guide == null) return NotFound(new { message = "Chưa có bài hướng dẫn nào" });
            return Ok(guide);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateGuide([FromBody] JsonElement body)
        {
            var guide = await _context.GuideArticles
                .OrderByDescending(g => g.PublishedAt)
                .FirstOrDefaultAsync();

            if (guide == null)
            {
                guide = new GuideArticle();
                _context.GuideArticles.Add(guide);
            }

            if (body.TryGetProperty("title", out var titleProp)) guide.Title = titleProp.GetString() ?? "HƯỚNG DẪN TÂN THỦ";
            if (body.TryGetProperty("subtitle", out var subProp)) guide.Subtitle = subProp.GetString();
            if (body.TryGetProperty("bannerImageUrl", out var bannerProp)) guide.BannerImageUrl = bannerProp.GetString() ?? "";
            if (body.TryGetProperty("author", out var authorProp)) guide.Author = authorProp.GetString() ?? "VALORANT ESPORTS STAFF";
            if (body.TryGetProperty("chapters", out var chapsProp)) guide.ChaptersJson = chapsProp.GetRawText();

            guide.PublishedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, guide });
        }
    }
}