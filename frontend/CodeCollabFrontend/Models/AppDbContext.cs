using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomParticipant> RoomParticipants { get; set; }
public DbSet<ChatMessage> ChatMessages { get; set; }
public DbSet<RoomFile> RoomFiles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Связь участников с комнатами
        modelBuilder.Entity<RoomParticipant>()
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(rp => rp.RoomId);

        modelBuilder.Entity<RoomParticipant>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(rp => rp.UserId);
    }
}