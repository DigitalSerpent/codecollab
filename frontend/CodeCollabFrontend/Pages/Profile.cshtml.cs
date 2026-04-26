using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class ProfileModel : PageModel
{
    private readonly AppDbContext _context;

    public ProfileModel(AppDbContext context)
    {
        _context = context;
    }

    public User? CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");

        CurrentUser = await _context.Users.FindAsync(userId);
        if (CurrentUser == null) return RedirectToPage("/Login");

        return Page();
    }

    public async Task<IActionResult> OnPostUploadAvatar(IFormFile avatarFile)
    {
        Console.WriteLine("=== OnPostUploadAvatar START ===");
        
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (avatarFile == null || avatarFile.Length == 0)
            return BadRequest(new { success = false, message = "No file" });

        Console.WriteLine($"File: {avatarFile.FileName}, size: {avatarFile.Length}");

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
        if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
        
        var fileName = $"{userId}_{DateTime.Now.Ticks}{Path.GetExtension(avatarFile.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatarFile.CopyToAsync(stream);
        }
        
        user.Avatar = $"/uploads/avatars/{fileName}";
        await _context.SaveChangesAsync();
        
        Console.WriteLine("Success!");
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostUploadCover(IFormFile coverFile)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (coverFile == null || coverFile.Length == 0)
            return BadRequest(new { success = false, message = "Файл не выбран" });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(coverFile.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            return BadRequest(new { success = false, message = "Можно загружать только картинки" });

        var fileName = $"cover_{userId}_{DateTime.Now.Ticks}{ext}";
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "covers");
        if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
        
        var filePath = Path.Combine(uploadsDir, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await coverFile.CopyToAsync(stream);
        }

        if (!string.IsNullOrEmpty(user.CoverUrl) && user.CoverUrl.Contains("/uploads/"))
        {
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.CoverUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
        }

        user.CoverUrl = $"/uploads/covers/{fileName}";
        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnGetGetSocials()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        var socials = new List<SocialItem>();
        if (!string.IsNullOrEmpty(user.SocialLinks))
        {
            try
            {
                socials = System.Text.Json.JsonSerializer.Deserialize<List<SocialItem>>(user.SocialLinks) ?? new List<SocialItem>();
            }
            catch { }
        }

        return new JsonResult(new { socials = socials });
    }

    public async Task<IActionResult> OnPostUpdateName([FromBody] UpdateNameRequest request)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name;
            HttpContext.Session.SetString("UserName", user.Name);
            await _context.SaveChangesAsync();
        }

        return new JsonResult(new { success = true });
    }

    public class UpdateNameRequest
    {
        public string Name { get; set; } = "";
    }

    public async Task<IActionResult> OnPostUpdateSocials([FromBody] UpdateSocialsRequest request)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.SocialLinks = System.Text.Json.JsonSerializer.Serialize(request.Socials);
        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    public class UpdateSocialsRequest
    {
        public List<SocialItem> Socials { get; set; } = new();
    }

    public class SocialItem
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
    }
}