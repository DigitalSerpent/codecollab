using Microsoft.AspNetCore.Mvc;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{roomId}/users")]
    public IActionResult GetUsers(int roomId)
    {
        var participants = _context.RoomParticipants
            .Where(rp => rp.RoomId == roomId)
            .Select(rp => new
            {
                Name = rp.User.Name ?? "Unknown",
                Avatar = rp.User.Avatar ?? "👤",
                Cursor = rp.User.Cursor ?? "⬤",
                IsOnline = rp.IsOnline
            })
            .ToList();

        return Ok(participants);
    }
}