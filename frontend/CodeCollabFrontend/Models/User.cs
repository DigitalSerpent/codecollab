namespace CodeCollabFrontend.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Avatar { get; set; }
    public string? Cursor { get; set; }

    // Связь с участием в комнатах
    public ICollection<RoomParticipant>? RoomParticipants { get; set; }
}