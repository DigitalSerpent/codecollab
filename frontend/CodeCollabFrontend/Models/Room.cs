using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCollabFrontend.Models;

public class Room
{
    public int Id { get; set; }
    
    [StringLength(50, ErrorMessage = "Название не может быть длиннее 50 символов")]
    public string Name { get; set; } = "";
    
    public DateTime CreatedAt { get; set; }
    public int MaxParticipants { get; set; }
    public string? PreviewCode { get; set; }

    // ID владельца комнаты
    public int OwnerId { get; set; }

    // Связь с участниками
    public ICollection<RoomParticipant>? RoomParticipants { get; set; }
}