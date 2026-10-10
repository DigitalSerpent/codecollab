using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Pages;

public class SetPasswordModel : PageModel
{
    private readonly AppDbContext _context;

    public SetPasswordModel(AppDbContext context)
    {
        _context = context;
    }

    public string ErrorMessage { get; set; } = "";

    public async Task<IActionResult> OnPostAsync(string password, string confirmPassword)
    {
        var userId = HttpContext.Session.GetInt32("TempUserId");
        if (userId == null)
        {
            return RedirectToPage("/RegisterChoice");
        }

        if (string.IsNullOrEmpty(password) || password != confirmPassword)
        {
            ErrorMessage = "Пароли не совпадают или пустые";
            return Page();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return RedirectToPage("/RegisterChoice");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.IsConfirmed = true;
        user.ConfirmationCode = null;
        await _context.SaveChangesAsync();

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);
        HttpContext.Session.Remove("TempUserId");

        return RedirectToPage("/Dashboard");
    }
}