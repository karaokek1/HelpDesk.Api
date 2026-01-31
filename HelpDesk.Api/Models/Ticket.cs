using System.ComponentModel.DataAnnotations;

namespace HelpDeskAPI.Models;

public class Ticket
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string Status { get; set; } = "Open"; 
    public int? AssignedTo { get; set; } 
    [Required]
    public int CreatedBy { get; set; } 
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    [MaxLength(2000)]
    public string? Resolution { get; set; }
    public User Creator { get; set; } = null!;
    public User? AssignedAdmin { get; set; }
}