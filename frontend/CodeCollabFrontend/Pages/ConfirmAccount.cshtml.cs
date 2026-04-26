using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class ConfirmAccountModel : PageModel
{
    private readonly AppDbContext _context;

    public ConfirmAccountModel(AppDbContext context)
    {
        _context = context;
    }

    public string ErrorMessage { get; set; } = "";
    public string SuccessMessage { get; set; } = "";

    public async Task<IActionResult> OnPostAsync(string code)
    {
        var userId = HttpContext.Session.GetInt32("TempUserId");
        if (userId == null)
        {
            ErrorMessage = "Сессия истекла. Зарегистрируйтесь заново.";
            return RedirectToPage("/Register");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            ErrorMessage = "Пользователь не найден";
            return RedirectToPage("/Register");
        }

        if (user.ConfirmationCode == code)
        {
            user.IsConfirmed = true;
            user.ConfirmationCode = null;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.Remove("TempUserId");

            return RedirectToPage("/Dashboard");
        }

        ErrorMessage = "Неверный код. Попробуйте ещё раз.";
        return Page();
    }
}