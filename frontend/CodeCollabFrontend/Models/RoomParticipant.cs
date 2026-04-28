namespace CodeCollabFrontend.Models;

public class RoomParticipant
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public bool IsOnline { get; set; }
    public string? ConnectionId { get; set; }
    
    // Новые поля для SignalR
    public string? UserName { get; set; }
    public string? Avatar { get; set; }
    public string? Cursor { get; set; }
    public DateTime LastSeen { get; set; }

    // Навигационные свойства
    public Room? Room { get; set; }
    public User? User { get; set; }
}