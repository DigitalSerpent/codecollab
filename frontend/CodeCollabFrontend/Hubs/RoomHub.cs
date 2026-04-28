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

    public async Task JoinRoom(string roomId, int userId, string userName, string avatar, string cursor)
    {
        var participant = await _db.RoomParticipants
            .FirstOrDefaultAsync(p => p.RoomId.ToString() == roomId && p.UserId == userId);

        if (participant == null)
        {
            participant = new RoomParticipant { RoomId = int.Parse(roomId), UserId = userId };
            _db.RoomParticipants.Add(participant);
        }

        participant.UserName = userName;
        participant.Avatar = avatar;
        participant.Cursor = cursor;
        participant.IsOnline = true;
        participant.ConnectionId = Context.ConnectionId;
        participant.LastSeen = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        await SendUpdateToGroup(roomId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var participant = await _db.RoomParticipants
            .FirstOrDefaultAsync(p => p.ConnectionId == Context.ConnectionId);

        if (participant != null)
        {
            participant.IsOnline = false;
            await _db.SaveChangesAsync();
            await SendUpdateToGroup(participant.RoomId.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task SendUpdateToGroup(string roomId)
    {
        var list = await _db.RoomParticipants
            .Where(p => p.RoomId.ToString() == roomId && p.IsOnline)
            .Include(p => p.User)
            .Select(p => new
            {
                p.UserId,
                UserName = p.User.Name,
                Avatar = p.User.Avatar,
                Cursor = p.User.Cursor
            })
            .ToListAsync();

        await Clients.Group(roomId).SendAsync("ParticipantList", list);
    }
}