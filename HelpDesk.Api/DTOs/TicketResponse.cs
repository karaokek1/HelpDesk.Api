namespace HelpDeskAPI.Models.DTOs;

public class TicketResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public UserDto Creator { get; set; } = new UserDto();
    public UserDto? AssignedAdmin { get; set; }
}