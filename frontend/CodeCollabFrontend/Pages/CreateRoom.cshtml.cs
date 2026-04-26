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

        // Получаем ID текущего пользователя из сессии
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized();  // ✅ теперь без строки
        }

        // Генерируем токен для приглашения
        string inviteToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, 16);

        // Создаём комнату
        var room = new Room
        {
            Name = data.Name,
            CreatedAt = DateTime.Now,
            MaxParticipants = data.MaxParticipants,
            PreviewCode = "// новая комната",
            InviteToken = inviteToken,
            OwnerId = userId.Value
        };

        _context.Rooms.Add(room);
        _context.SaveChanges();

        // Добавляем создателя как участника
        var participant = new RoomParticipant
        {
            RoomId = room.Id,
            UserId = userId.Value,
            IsOnline = true
        };
        _context.RoomParticipants.Add(participant);
        _context.SaveChanges();

        return new JsonResult(new { success = true, roomId = room.Id });
    }
}