using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMasterAPI.Data;
using TaskMasterAPI.Models;
using TaskMasterAPI.DTOs;

[Route("api/tasks")]
[ApiController]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new UnauthorizedAccessException("User ID not found in token");

        return int.Parse(claim.Value);
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
    {
        var userId = GetUserId();

        var task = new TaskItemModel
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        return Ok(task);
    }

}
