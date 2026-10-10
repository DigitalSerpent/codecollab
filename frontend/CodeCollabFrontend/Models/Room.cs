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
    public int? OwnerId { get; set; }

    // Навигационное свойство — ОБЯЗАТЕЛЬНО
    public virtual ICollection<RoomParticipant> RoomParticipants { get; set; } = new List<RoomParticipant>();
}