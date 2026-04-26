using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;

namespace CodeCollabFrontend.Controllers;

[ApiController]
[Route("api/room/{roomId}/file")]
public class RoomFileController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomFileController(AppDbContext context)
    {
        _context = context;
    }

    // GET: список файлов в комнате
    [HttpGet]
    public async Task<IActionResult> GetFiles(int roomId)
    {
        var files = await _context.RoomFiles
            .Where(f => f.RoomId == roomId)
            .Select(f => new { f.Id, f.Name, f.IsReadme })
            .ToListAsync();
        return Ok(files);
    }

    // GET: содержимое файла
    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFile(int roomId, int fileId)
    {
        var file = await _context.RoomFiles
            .FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        if (file == null) return NotFound();
        return Ok(new { file.Id, file.Name, file.Content, file.IsReadme });
    }

    // POST: создать новый файл
    [HttpPost]
    public async Task<IActionResult> CreateFile(int roomId, [FromBody] CreateFileRequest request)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null) return NotFound("Комната не найдена");

        var newFile = new RoomFile
        {
            RoomId = roomId,
            Name = request.Name,
            Content = "",
            IsReadme = request.Name.ToLower() == "readme.md"
        };
        _context.RoomFiles.Add(newFile);
        await _context.SaveChangesAsync();
        return Ok(new { newFile.Id, newFile.Name, newFile.IsReadme });
    }

    // PUT: обновить содержимое файла
    [HttpPut("{fileId}")]
    public async Task<IActionResult> UpdateFile(int roomId, int fileId, [FromBody] UpdateFileRequest request)
    {
        var file = await _context.RoomFiles
            .FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        if (file == null) return NotFound();
        
        file.Content = request.Content;
        await _context.SaveChangesAsync();
        return Ok();
    }

    // DELETE: удалить файл
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(int roomId, int fileId)
    {
        var file = await _context.RoomFiles
            .FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        if (file == null) return NotFound();
        
        _context.RoomFiles.Remove(file);
        await _context.SaveChangesAsync();
        return Ok();
    }
}

public class CreateFileRequest
{
    public string Name { get; set; } = "";
}

public class UpdateFileRequest
{
    public string Content { get; set; } = "";
}