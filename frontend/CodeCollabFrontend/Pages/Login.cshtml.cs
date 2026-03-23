using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class LoginModel : PageModel
{
    private readonly AppDbContext _context;

    public LoginModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ErrorMessage { get; set; } = "";

    public class InputModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
        if (user == null)
        {
            ErrorMessage = "Неверный email или пароль";
            return Page();
        }

        if (!BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
        {
            ErrorMessage = "Неверный email или пароль";
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);

        return RedirectToPage("/Dashboard");
    }
}