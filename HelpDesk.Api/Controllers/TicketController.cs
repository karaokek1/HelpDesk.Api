using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using HelpDeskAPI.Models.DTOs;

namespace HelpDeskAPI.Controllers;

[Authorize] // Только авторизованные пользователи
[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Ticket
    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Creator)
            .Include(t => t.AssignedAdmin)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedDate = t.CreatedDate,
                Creator = new UserDto
                {
                    Id = t.Creator.Id,
                    Name = t.Creator.Name,
                    Email = t.Creator.Email
                },
                AssignedAdmin = t.AssignedAdmin == null ? null : new UserDto
                {
                    Id = t.AssignedAdmin.Id,
                    Name = t.AssignedAdmin.Name,
                    Email = t.AssignedAdmin.Email
                }
            })
            .ToListAsync();

        return Ok(tickets);
    }

    // POST: api/Ticket
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        // Получаем Id пользователя из JWT
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"));
        if (userIdClaim == null)
            return Unauthorized(new { error = "User ID not found in token" });

        int userId = int.Parse(userIdClaim.Value);

        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            CreatedBy = userId,
            Status = "Open",
            CreatedDate = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return Ok(new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedDate = ticket.CreatedDate,
            Creator = await _context.Users
                        .Where(u => u.Id == ticket.CreatedBy)
                        .Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email })
                        .FirstOrDefaultAsync(),
            AssignedAdmin = null
        });
    }
}
