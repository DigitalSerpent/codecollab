using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;
using System.Net.Http;

namespace CodeCollabFrontend.Pages;

public class RoomModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public RoomModel(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
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

        // ========== ДОБАВЛЕНИЕ УЧАСТНИКА ==========
        var participant = await _context.RoomParticipants
            .FirstOrDefaultAsync(rp => rp.RoomId == id && rp.UserId == userId.Value);

        if (participant == null)
        {
            participant = new RoomParticipant
            {
                RoomId = id,
                UserId = userId.Value,
                IsOnline = true,
                JoinedAt = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                UserName = CurrentUser.Name,
                Avatar = CurrentUser.Avatar,
                Cursor = CurrentUser.Cursor
            };
            _context.RoomParticipants.Add(participant);
        }
        else
        {
            participant.IsOnline = true;
            participant.LastSeen = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Page();
    }
}