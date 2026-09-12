using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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

// 2. Đăng ký Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 3. Cấu hình CORS
builder.Services.AddCors(p => p.AddDefaultPolicy(b =>
    b.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod()));

// 4. Đọc thông số Supabase
var supabaseUrl = (builder.Configuration.GetSection("Supabase")["Url"] ?? string.Empty).TrimEnd('/');
var supabaseKey = builder.Configuration.GetSection("Supabase")["Key"] ?? string.Empty;

// 5. Đăng ký Supabase Client
var supabaseOptions = new Supabase.SupabaseOptions { AutoRefreshToken = true, AutoConnectRealtime = true };
builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions));

// 6. Đăng ký Named HttpClient kết nối Supabase REST API
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

// 7. Pipeline cấu hình
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthorization();

// 8. Ánh xạ Controller routes
app.MapControllers();

app.Run();