namespace CodeCollabFrontend.Models;

public class RoomFile
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string Name { get; set; } = "";
    public string? Content { get; set; }
    public bool IsReadme { get; set; }
    
    public Room? Room { get; set; }
}