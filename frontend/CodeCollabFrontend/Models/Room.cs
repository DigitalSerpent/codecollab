using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCollabFrontend.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int MaxParticipants { get; set; }
    public string? PreviewCode { get; set; }
    public string? InviteToken { get; set; }
    
    // Кто создал комнату
    public int? OwnerId { get; set; }

    // Связь с участниками
    public ICollection<RoomParticipant>? RoomParticipants { get; set; }
}