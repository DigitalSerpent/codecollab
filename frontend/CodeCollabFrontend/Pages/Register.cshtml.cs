using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using CodeCollabFrontend.Services;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public RegisterModel(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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
            return Page();

        if (Input.Password != Input.ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            return Page();
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
        if (existingUser != null)
        {
            ErrorMessage = "Пользователь с таким email уже существует";
            return Page();
        }

        var random = new Random();
        var confirmationCode = random.Next(100000, 999999).ToString();

        await _emailService.SendCodeAsync(Input.Email, confirmationCode);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password);

        var user = new User
        {
            Name = Input.Name,
            Email = Input.Email,
            PasswordHash = passwordHash,
            ConfirmationCode = confirmationCode,
            IsConfirmed = false,
            Avatar = "👤",
            Cursor = "⬤"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        HttpContext.Session.SetInt32("TempUserId", user.Id);
        HttpContext.Session.SetString("TempUserEmail", user.Email);

        return RedirectToPage("/ConfirmAccount");
    }
}