using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Pages;

public class ProfileModel : PageModel
{
    private readonly AppDbContext _context;

    public ProfileModel(AppDbContext context)
    {
        _context = context;
    }

    public class UpdateData
    {
        public string Name { get; set; } = "";
        public string Cursor { get; set; } = "";
        public string Avatar { get; set; } = "";
    }

    public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateData data)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return StatusCode(401, new { success = false, message = "Не авторизован" });
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { success = false, message = "Пользователь не найден" });
        }

        user.Name = data.Name;
        user.Cursor = data.Cursor;
        user.Avatar = data.Avatar;

        await _context.SaveChangesAsync();

        HttpContext.Session.SetString("UserName", user.Name);

        return new JsonResult(new { success = true });
    }
}