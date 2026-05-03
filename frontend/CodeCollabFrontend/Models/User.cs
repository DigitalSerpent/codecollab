using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCollabFrontend.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? Avatar { get; set; }
    public string? CoverUrl { get; set; }
    public string? SocialLinks { get; set; }
    public string? TelegramChatId { get; set; }
    public string? TelegramUsername { get; set; }
    public bool IsConfirmed { get; set; }
    public string? ConfirmationCode { get; set; }
    public string? Cursor { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RoomParticipant> RoomParticipants { get; set; } = new List<RoomParticipant>();
}