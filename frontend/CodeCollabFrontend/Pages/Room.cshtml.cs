using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Pages;

public class RoomModel : PageModel
{
    private readonly AppDbContext _context;

    public RoomModel(AppDbContext context)
    {
        _context = context;
    }

    public Room? Room { get; set; }
    public User? CurrentUser { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");

        CurrentUser = await _context.Users.FindAsync(userId);
        if (CurrentUser == null) return RedirectToPage("/Login");

        Room = await _context.Rooms
            .Include(r => r.RoomParticipants)
                .ThenInclude(rp => rp.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (Room == null) return RedirectToPage("/Dashboard");

        // ========== АВТО-ДОБАВЛЕНИЕ УЧАСТНИКА ==========
        var participant = await _context.RoomParticipants
            .FirstOrDefaultAsync(rp => rp.RoomId == id && rp.UserId == userId.Value);

        if (participant == null)
        {
            participant = new RoomParticipant
            {
                RoomId = id,
                UserId = userId.Value,
                IsOnline = true,
                JoinedAt = DateTime.UtcNow
            };
            _context.RoomParticipants.Add(participant);
        }
        else
        {
            participant.IsOnline = true;
        }

        await _context.SaveChangesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostLeaveAsync(int roomId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");

        var participant = await _context.RoomParticipants
            .FirstOrDefaultAsync(rp => rp.RoomId == roomId && rp.UserId == userId.Value);

        if (participant != null)
        {
            participant.IsOnline = false;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Dashboard");
    }
}