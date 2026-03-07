using Microsoft.AspNetCore.Mvc;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChatController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{roomId}")]
    public IActionResult GetMessages(int roomId)
    {
        var messages = _context.ChatMessages
            .Where(m => m.RoomId == roomId)
            .OrderBy(m => m.Timestamp)
            .ToList();
        return Ok(messages);
    }

    [HttpPost("send")]
    public IActionResult SendMessage([FromBody] ChatMessage message)
    {
        message.Timestamp = DateTime.Now;
        _context.ChatMessages.Add(message);
        _context.SaveChanges();
        return Ok(new { success = true });
    }
}