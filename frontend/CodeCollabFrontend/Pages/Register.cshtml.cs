using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _context;

    public RegisterModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ErrorMessage { get; set; } = "";

    public class InputModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.Password != Input.ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            return Page();
        }

        // Проверяем, существует ли пользователь
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
        if (existingUser != null)
        {
            ErrorMessage = "Пользователь с таким email уже существует";
            return Page();
        }

        // Хэшируем пароль
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password);

        var user = new User
        {
            Name = Input.Name,
            Email = Input.Email,
            PasswordHash = passwordHash,
            Avatar = "👤",
            Cursor = "⬤"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Сохраняем пользователя в сессии
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);

        return RedirectToPage("/Dashboard");
    }
}