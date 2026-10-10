using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;
namespace CodeCollabFrontend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) { _context = context; }
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        return u == null ? NotFound() : Ok(new { id = u.Id, name = u.Name, avatar = u.Avatar ?? "👤", cursor = u.Cursor ?? "⬤", cover = u.CoverUrl });
    }
}
