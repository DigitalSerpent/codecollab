using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeCollabFrontend.Models;
using Microsoft.EntityFrameworkCore;
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

        // Сообщаем Python-серверу, что пользователь вошёл в комнату
        await _httpClient.PostAsync("http://localhost:8001/room_join", 
            new StringContent($"{{ \"roomId\": {id}, \"userId\": {userId}, \"userName\": \"{CurrentUser.Name}\", \"avatar\": \"{CurrentUser.Avatar}\", \"cursor\": \"{CurrentUser.Cursor}\" }}", 
            System.Text.Encoding.UTF8, "application/json"));

        return Page();
    }
}