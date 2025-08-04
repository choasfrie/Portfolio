namespace Portfolio_Backend.Models;

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Email { get; set; } = "";
    public string Message { get; set; } = "";  // 👈 Add this
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}