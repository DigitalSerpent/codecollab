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
            entity.Property(e => e.RoomId).HasColumnName("RoomId").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
            
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
        
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("ChatMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoomId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Text).IsRequired();
            
            entity.HasOne(e => e.Room)
                .WithMany()
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<RoomFile>(entity =>
        {
            entity.ToTable("RoomFiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoomId).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            
            entity.HasOne(e => e.Room)
                .WithMany()
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}