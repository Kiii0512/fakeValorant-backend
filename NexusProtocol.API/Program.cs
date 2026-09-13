using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using NexusProtocol.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Mở giới hạn kích thước nhận file cho Server (100 MB)
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

// 2. Cấu hình CORS - Cho phép mọi Origin, Method, Header
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Đăng ký EF Core Npgsql DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabasePostgres")));

// 4. Đăng ký Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 5. Đọc thông số Supabase
var supabaseUrl = (builder.Configuration.GetSection("Supabase")["Url"] ?? string.Empty).TrimEnd('/');
var supabaseKey = builder.Configuration.GetSection("Supabase")["Key"] ?? string.Empty;

// 6. Đăng ký Supabase Client
var supabaseOptions = new Supabase.SupabaseOptions { AutoRefreshToken = true, AutoConnectRealtime = true };
builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions));

// 7. Đăng ký Named HttpClient kết nối Supabase REST API
builder.Services.AddHttpClient("supabase", client =>
{
    if (!string.IsNullOrWhiteSpace(supabaseUrl))
    {
        client.BaseAddress = new Uri($"{supabaseUrl}/rest/v1/");
    }
    if (!string.IsNullOrWhiteSpace(supabaseKey))
    {
        client.DefaultRequestHeaders.Add("apikey", supabaseKey);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supabaseKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
});

var app = builder.Build();

// 8. Tự động áp dụng Migration khi server khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[EF Core Migration Warning]: {ex.Message}");
    }
}

// 9. Pipeline Middleware sắp xếp đúng chuẩn ASP.NET Core
app.UseRouting();

// CORS bắt buộc phải nằm giữa UseRouting và UseAuthorization
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapOpenApi();

// 10. Ánh xạ Controller và áp dụng CORS policy
app.MapControllers().RequireCors("AllowAll");

app.Run();