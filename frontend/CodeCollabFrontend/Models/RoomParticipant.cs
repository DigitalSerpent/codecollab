namespace CodeCollabFrontend.Models;

public class RoomParticipant
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public bool IsOnline { get; set; }

    // Навигационные свойства
    public Room? Room { get; set; }
    public string? ConnectionId { get; set; }
    public User? User { get; set; }
}