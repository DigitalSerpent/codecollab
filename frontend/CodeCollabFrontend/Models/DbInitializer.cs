using Microsoft.EntityFrameworkCore;

namespace CodeCollabFrontend.Models;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Применяем миграции (создаём таблицы, если их нет)
        context.Database.Migrate();

        // Если в базе уже есть пользователи — ничего не делаем
        if (context.Users.Any())
        {
            return;
        }

        // Создаём тестовых пользователей
        var users = new User[]
        {
            new User { Name = "Alice", Avatar = "👩‍💻", Cursor = "✨" },
            new User { Name = "Bob", Avatar = "👨‍💻", Cursor = "🔷" },
            new User { Name = "Charlie", Avatar = "🧑‍🔧", Cursor = "🔶" },
            new User { Name = "Diana", Avatar = "👩‍🎨", Cursor = "💫" },
        };
        context.Users.AddRange(users);
        context.SaveChanges();

        // Создаём тестовые комнаты
        var rooms = new Room[]
        {
            new Room 
            { 
                Name = "Мой проект", 
                CreatedAt = DateTime.Parse("2026-03-05"), 
                MaxParticipants = 5,
                PreviewCode = "def hello():\n    print(\"Hello, world!\")"
            },
            new Room 
            { 
                Name = "Тестовая комната", 
                CreatedAt = DateTime.Parse("2026-03-04"), 
                MaxParticipants = 5,
                PreviewCode = "function test() {\n    console.log(\"hi\");\n}"
            },
        };
        context.Rooms.AddRange(rooms);
        context.SaveChanges();

        // Добавляем участников в комнаты
        var participants = new RoomParticipant[]
        {
            new RoomParticipant { RoomId = 1, UserId = 1, IsOnline = true },
            new RoomParticipant { RoomId = 1, UserId = 2, IsOnline = false },
            new RoomParticipant { RoomId = 2, UserId = 3, IsOnline = false },
            new RoomParticipant { RoomId = 2, UserId = 4, IsOnline = true },
        };
        context.RoomParticipants.AddRange(participants);
        context.SaveChanges();
    }
}