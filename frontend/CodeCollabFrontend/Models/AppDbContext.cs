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
        // Явно указываем таблицы и колонки
        modelBuilder.Entity<RoomParticipant>().ToTable("RoomParticipants");
        
        // Настройка первичного ключа
        modelBuilder.Entity<RoomParticipant>()
            .HasKey(rp => rp.Id);
        
        // Настройка свойств
        modelBuilder.Entity<RoomParticipant>()
            .Property(rp => rp.RoomId)
            .HasColumnName("RoomId")
            .IsRequired();
            
        modelBuilder.Entity<RoomParticipant>()
            .Property(rp => rp.UserId)
            .HasColumnName("UserId")
            .IsRequired();
        
        // Связь с Room
        modelBuilder.Entity<RoomParticipant>()
            .HasOne(rp => rp.Room)
            .WithMany(r => r.RoomParticipants)
            .HasForeignKey(rp => rp.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Связь с User
        modelBuilder.Entity<RoomParticipant>()
            .HasOne(rp => rp.User)
            .WithMany(u => u.RoomParticipants)
            .HasForeignKey(rp => rp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Уникальный индекс для пары RoomId + UserId (чтобы предотвратить дубликаты)
        modelBuilder.Entity<RoomParticipant>()
            .HasIndex(rp => new { rp.RoomId, rp.UserId })
            .IsUnique();
        
        // Индекс для быстрого поиска по RoomId
        modelBuilder.Entity<RoomParticipant>()
            .HasIndex(rp => rp.RoomId);
        
        // Индекс для быстрого поиска по UserId
        modelBuilder.Entity<RoomParticipant>()
            .HasIndex(rp => rp.UserId);
        
        // Настройка остальных таблиц (если нужно)
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Room>().ToTable("Rooms");
        modelBuilder.Entity<ChatMessage>().ToTable("ChatMessages");
        modelBuilder.Entity<RoomFile>().ToTable("RoomFiles");
        
        // Конфигурация для ChatMessage (если есть внешние ключи)
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
        
        // Конфигурация для RoomFile
        modelBuilder.Entity<RoomFile>()
            .HasOne(rf => rf.Room)
            .WithMany()
            .HasForeignKey(rf => rf.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}