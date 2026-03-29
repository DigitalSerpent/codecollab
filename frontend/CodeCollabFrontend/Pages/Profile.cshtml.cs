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

    public class SocialsData
    {
        public string Github { get; set; } = "";
        public string Telegram { get; set; } = "";
        public string Instagram { get; set; } = "";
        public string LinkedIn { get; set; } = "";
    }

    public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateData data)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return StatusCode(401, new { success = false });

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.Name = data.Name;
        user.Cursor = data.Cursor;
        user.Avatar = data.Avatar;

        await _context.SaveChangesAsync();
        HttpContext.Session.SetString("UserName", user.Name);

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostUpdateSocialsAsync([FromBody] SocialsData data)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return StatusCode(401);

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        // Здесь нужно добавить поля в модель User (Github, Telegram, Instagram, LinkedIn)
        // Пока просто сохраняем в отдельную таблицу или игнорируем

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnGetSocialsAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        // Пока возвращаем пустые строки
        return new JsonResult(new { github = "", telegram = "", instagram = "", linkedin = "" });
    }
}



а что за фигня вроде все работает но когда я нажимаю на конструктор курсора он просто вибрирует а вместо картинок курсоров слова там верный путь frontend/CodeCollabFrontend/wwwroot/cursors/handpointing.svg
frontend/CodeCollabFrontend/wwwroot/cursors/default.svg
вот такие файлы... 7 штук
