using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;
using CodeCollabFrontend.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 10 * 1024 * 1024;
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Настройка DbContext с отключением автоматических миграций
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=codecollab.db");
    // Отключаем автоматическое создание и применение миграций
    options.EnableServiceProviderCaching(false);
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// ВАЖНО: НЕ вызываем DbInitializer и не применяем миграции
// Миграции полностью отключены, используем существующую БД

// Проверяем подключение к БД без миграций
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Просто проверяем, что БД существует и можно подключиться
        var canConnect = context.Database.CanConnect();
        if (!canConnect)
        {
            Console.WriteLine("WARNING: Cannot connect to database!");
        }
        else
        {
            Console.WriteLine("Database connection successful!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection error: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // 👈 ЭТО ДОБАВЛЕНО — для отдачи аватарок и статики
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapControllers();
app.UseStatusCodePagesWithReExecute("/Error404");

app.Run();