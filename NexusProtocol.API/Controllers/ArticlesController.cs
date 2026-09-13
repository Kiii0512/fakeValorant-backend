using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusProtocol.API.Data;
using NexusProtocol.API.DTOs;
using NexusProtocol.API.Models;

namespace NexusProtocol.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _context.Articles
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync();
            return Ok(articles);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var featured = await _context.Articles
                .Where(a => a.IsFeatured)
                .OrderByDescending(a => a.PublishedAt)
                .Take(3)
                .ToListAsync();
            return Ok(featured);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound(new { message = "Không tìm thấy bài viết" });
            return Ok(article);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleDto dto)
        {
            var article = new Article
            {
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Author = string.IsNullOrWhiteSpace(dto.Author) ? "VALORANT ESPORTS STAFF" : dto.Author,
                MainImageUrl = dto.MainImageUrl,
                SubImageUrl = dto.SubImageUrl,
                Content = dto.Content,
                IsFeatured = false,
                PublishedAt = DateTime.UtcNow
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateArticleDto dto)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound(new { message = "Không tìm thấy bài viết" });

            article.Title = dto.Title;
            article.Subtitle = dto.Subtitle;
            article.Author = string.IsNullOrWhiteSpace(dto.Author) ? "VALORANT ESPORTS STAFF" : dto.Author;
            if (!string.IsNullOrWhiteSpace(dto.MainImageUrl)) article.MainImageUrl = dto.MainImageUrl;
            if (dto.SubImageUrl != null) article.SubImageUrl = dto.SubImageUrl;
            article.Content = dto.Content;

            await _context.SaveChangesAsync();
            return Ok(article);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound(new { message = "Không tìm thấy bài viết" });

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa bài viết thành công" });
        }

        [HttpPatch("{id:guid}/toggle-featured")]
        public async Task<IActionResult> ToggleFeatured(Guid id, [FromBody] JsonElement body)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound(new { message = "Không tìm thấy bài viết" });

            bool nextState = false;
            if (body.TryGetProperty("is_featured", out var valSnake))
            {
                nextState = valSnake.GetBoolean();
            }
            else if (body.TryGetProperty("isFeatured", out var valCamel))
            {
                nextState = valCamel.GetBoolean();
            }

            if (nextState)
            {
                var currentFeaturedCount = await _context.Articles.CountAsync(a => a.IsFeatured && a.Id != id);
                if (currentFeaturedCount >= 3)
                {
                    return BadRequest(new { message = "Chỉ được phép chọn tối đa 3 bài viết nổi bật. Vui lòng bỏ bớt bài khác trước khi chọn bài này." });
                }
            }

            article.IsFeatured = nextState;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, isFeatured = article.IsFeatured });
        }
    }
}