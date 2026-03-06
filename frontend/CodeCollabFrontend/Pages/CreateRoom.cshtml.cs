using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using System.Text.Json;

namespace CodeCollabFrontend.Pages;

public class CreateRoomModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateRoomModel(AppDbContext context)
    {
        _context = context;
    }

    public class RoomData
    {
        public string Name { get; set; } = "";
        public int MaxParticipants { get; set; }
        public string Rights { get; set; } = "";
    }

    public IActionResult OnPost([FromBody] RoomData data)
    {
        if (string.IsNullOrWhiteSpace(data.Name))
        {
            return BadRequest("Название обязательно");
        }

        // Создаём комнату
        var room = new Room
        {
            Name = data.Name,
            CreatedAt = DateTime.Now,
            MaxParticipants = data.MaxParticipants,
            PreviewCode = "// новая комната"
        };

        _context.Rooms.Add(room);
        _context.SaveChanges();

        // Добавляем создателя как участника (пока заглушка — пользователь 1)
        var participant = new RoomParticipant
        {
            RoomId = room.Id,
            UserId = 1, // Позже заменим на реального пользователя
            IsOnline = true
        };
        _context.RoomParticipants.Add(participant);
        _context.SaveChanges();

        return new JsonResult(new { success = true, roomId = room.Id });
    }
}