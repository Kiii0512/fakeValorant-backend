using Microsoft.EntityFrameworkCore;
using NexusProtocol.API.Models;

namespace NexusProtocol.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles => Set<Article>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
        public DbSet<GuideArticle> GuideArticles => Set<GuideArticle>();
    }
}