using Microsoft.AspNetCore.Mvc;
using CodeCollabFrontend.Models;
using System.Linq;
namespace CodeCollabFrontend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _context;
    public ChatController(AppDbContext context) { _context = context; }
    [HttpGet("{roomId}")]
    public IActionResult GetMessages(int roomId)
    {
        return Ok(_context.ChatMessages.Where(m => m.RoomId == roomId).OrderBy(m => m.Timestamp).ToList());
    }
    [HttpPost("send")]
    public IActionResult SendMessage([FromBody] SendMessageRequest req)
    {
        var msg = new ChatMessage { RoomId = req.RoomId, UserId = req.UserId, UserName = req.UserName, Text = req.Text, Timestamp = DateTime.UtcNow };
        _context.ChatMessages.Add(msg);
        _context.SaveChanges();
        return Ok(new { success = true });
    }
}
public class SendMessageRequest
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Text { get; set; } = "";
}
