using System.ComponentModel.DataAnnotations;

namespace CodeCollabFrontend.Models;

public class User
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(50)]
    public string Name { get; set; } = "";
    
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string? Email { get; set; }
    
    public string? PasswordHash { get; set; }
    
    public string? TelegramUsername { get; set; }
    public long? TelegramChatId { get; set; }
    public string? ConfirmationCode { get; set; }
    public bool IsConfirmed { get; set; } = false;
    
    public string? Avatar { get; set; } = "👤";
    public string? Cursor { get; set; } = "⬤";
    public string? SocialLinks { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<RoomParticipant>? RoomParticipants { get; set; }
}