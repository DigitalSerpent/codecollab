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
        modelBuilder.Entity<RoomParticipant>(entity =>
        {
            entity.ToTable("RoomParticipants");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.RoomId)
                .HasColumnName("RoomId")
                .IsRequired();
            
            entity.Property(e => e.UserId)
                .HasColumnName("UserId")
                .IsRequired();
            
            entity.HasOne(e => e.Room)
                .WithMany(e => e.RoomParticipants)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.RoomParticipants)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.RoomId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.RoomId);
            entity.HasIndex(e => e.UserId);
        });
        
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Room)
            .WithMany()
            .HasForeignKey(cm => cm.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<RoomFile>()
            .HasOne(rf => rf.Room)
            .WithMany()
            .HasForeignKey(rf => rf.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}