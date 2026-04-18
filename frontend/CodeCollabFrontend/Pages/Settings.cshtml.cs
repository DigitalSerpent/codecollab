using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class SettingsModel : PageModel
{
    private readonly AppDbContext _context;

    public SettingsModel(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Settings/ChangePassword?old=xxx&new=yyy
    public IActionResult OnGetChangePassword(string old, string @new)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Redirect("/Login");

        var user = _context.Users.Find(userId);
        if (user == null) return Redirect("/Login");

        if (!BCrypt.Net.BCrypt.Verify(old, user.PasswordHash))
        {
            TempData["Error"] = "Неверный старый пароль";
            return Redirect("/Settings");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(@new);
        _context.SaveChanges();
        HttpContext.Session.Clear();
        return Redirect("/Login");
    }

    // GET: /Settings?handler=Delete
    public IActionResult OnGetDelete()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Redirect("/Login");

        var user = _context.Users.Find(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        
        HttpContext.Session.Clear();
        return Redirect("/Login");
    }
}