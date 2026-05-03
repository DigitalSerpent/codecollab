using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCollabFrontend.Models;

public class RoomParticipant
{
    public int Id { get; set; }
    
    [Column("RoomId")]
    public int RoomId { get; set; }
    
    [Column("UserId")]
    public int UserId { get; set; }
    
    public bool IsOnline { get; set; }
    public string? ConnectionId { get; set; }
    
    public string? UserName { get; set; }
    public string? Avatar { get; set; }
    public string? Cursor { get; set; }
    public DateTime LastSeen { get; set; }

    // Навигационные свойства
    [ForeignKey("RoomId")]
    public Room? Room { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
}