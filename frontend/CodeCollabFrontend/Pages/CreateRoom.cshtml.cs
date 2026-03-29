using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Pages;

public class CreateRoomModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateRoomModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RoomData Input { get; set; } = new();

    public class RoomData
    {
        public string Name { get; set; } = "";
        public int MaxParticipants { get; set; }
        public string Rights { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return StatusCode(401, new { success = false, message = "Вы не авторизованы" });
            }

            if (string.IsNullOrWhiteSpace(Input.Name))
            {
                return BadRequest(new { success = false, message = "Название обязательно" });
            }

            var room = new Room
            {
                Name = Input.Name,
                CreatedAt = DateTime.Now,
                MaxParticipants = Input.MaxParticipants,
                PreviewCode = "// новая комната"
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var participant = new RoomParticipant
            {
                RoomId = room.Id,
                UserId = userId.Value,
                IsOnline = true
            };
            _context.RoomParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, roomId = room.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}