using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Hubs;

public class RoomHub : Hub
{
    private readonly AppDbContext _db;

    public RoomHub(AppDbContext db)
    {
        _db = db;
    }

    public async Task JoinRoom(int roomId, int userId, string userName, string avatar, string cursor)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");

        var participant = _db.RoomParticipants.FirstOrDefault(p => p.RoomId == roomId && p.UserId == userId);
        if (participant == null)
        {
            participant = new RoomParticipant { RoomId = roomId, UserId = userId, IsOnline = true };
            _db.RoomParticipants.Add(participant);
        }
        else
        {
            participant.IsOnline = true;
        }
        await _db.SaveChangesAsync();

        await Clients.Group($"room_{roomId}").SendAsync("ParticipantList", await GetParticipants(roomId));
        await Clients.Group($"room_{roomId}").SendAsync("UserMessage", $"{userName} вошёл в комнату");
    }

    private async Task<List<object>> GetParticipants(int roomId)
    {
        return await _db.RoomParticipants
            .Where(p => p.RoomId == roomId)
            .Include(p => p.User)
            .Select(p => new
            {
                id = p.User.Id,
                name = p.User.Name,
                avatar = p.User.Avatar,
                cursor = p.User.Cursor,
                online = p.IsOnline
            })
            .ToListAsync<object>();
    }
}