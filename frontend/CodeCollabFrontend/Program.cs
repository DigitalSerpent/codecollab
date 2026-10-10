using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;
using CodeCollabFrontend.Services;
using Microsoft.AspNetCore.Http.Features;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.Configure<FormOptions>(o => {
    o.ValueLengthLimit = 10 * 1024 * 1024;
    o.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromDays(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<AppDbContext>(o => {
    o.UseSqlite("Data Source=codecollab.db");
    o.EnableServiceProviderCaching(false);
});
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<EmailService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try { if (!context.Database.CanConnect()) Console.WriteLine("DB Connection fail"); }
    catch (Exception ex) { Console.WriteLine($"DB error: {ex.Message}"); }
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapControllers();
app.UseStatusCodePagesWithReExecute("/Error404");
app.Run();
