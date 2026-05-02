using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetRoomUsers(int roomId)
    {
        var participants = await _context.RoomParticipants
            .Where(rp => rp.RoomId == roomId)
            .Include(rp => rp.User)
            .Select(rp => new {
                user_id = rp.UserId,
                name = rp.User.Name,
                avatar = rp.User.Avatar ?? "👤",
                cursor = rp.User.Cursor ?? "⬤",
                online = rp.IsOnline
            })
            .ToListAsync();

        return Ok(participants);
    }
}