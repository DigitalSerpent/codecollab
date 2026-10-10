using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Pages;

public class CreateRoomPageModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateRoomPageModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RoomData Input { get; set; } = new();

    public string ErrorMessage { get; set; } = "";

    public class RoomData
    {
        public string Name { get; set; } = "";
        public int MaxParticipants { get; set; } = 2;
        public string Rights { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Login");
        }

        if (string.IsNullOrWhiteSpace(Input.Name))
        {
            ErrorMessage = "Название обязательно";
            return Page();
        }

        var room = new Room
        {
            Name = Input.Name,
            CreatedAt = DateTime.Now,
            MaxParticipants = Input.MaxParticipants,
            PreviewCode = "// новая комната",
            OwnerId = userId.Value
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

        return RedirectToPage("/Dashboard");
    }
}