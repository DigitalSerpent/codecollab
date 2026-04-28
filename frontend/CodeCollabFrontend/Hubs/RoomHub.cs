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
        var participant = await _db.RoomParticipants
            .FirstOrDefaultAsync(p => p.RoomId == roomId && p.UserId == userId);

        if (participant == null)
        {
            participant = new RoomParticipant
            {
                RoomId = roomId,
                UserId = userId,
                IsOnline = true,
                UserName = userName,
                Avatar = avatar,
                Cursor = cursor,
                LastSeen = DateTime.UtcNow,
                ConnectionId = Context.ConnectionId
            };
            _db.RoomParticipants.Add(participant);
        }
        else
        {
            participant.IsOnline = true;
            participant.UserName = userName;
            participant.Avatar = avatar;
            participant.Cursor = cursor;
            participant.LastSeen = DateTime.UtcNow;
            participant.ConnectionId = Context.ConnectionId;
            _db.RoomParticipants.Update(participant);
        }

        await _db.SaveChangesAsync();
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        await SendParticipantList(roomId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var participant = await _db.RoomParticipants
            .FirstOrDefaultAsync(p => p.ConnectionId == Context.ConnectionId);

        if (participant != null)
        {
            participant.IsOnline = false;
            await _db.SaveChangesAsync();
            await SendParticipantList(participant.RoomId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task SendParticipantList(int roomId)
    {
        var participants = await _db.RoomParticipants
            .Where(p => p.RoomId == roomId && p.IsOnline)
            .Select(p => new
            {
                p.UserId,
                name = p.UserName,
                avatar = p.Avatar,
                cursor = p.Cursor,
                online = p.IsOnline
            })
            .ToListAsync();

        await Clients.Group(roomId.ToString()).SendAsync("ParticipantList", participants);
    }
}