using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProtocol.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateArticles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    subtitle = table.Column<string>(type: "text", nullable: true),
                    author = table.Column<string>(type: "text", nullable: false),
                    main_image_url = table.Column<string>(type: "text", nullable: false),
                    sub_image_url = table.Column<string>(type: "text", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
