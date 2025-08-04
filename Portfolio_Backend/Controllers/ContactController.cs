using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Portfolio_Backend.Models;
using Portfolio_Backend.Services;

namespace Portfolio_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly string _connString = "Data Source=contact.db";

    public ContactController(EmailService emailService)
    {
        _emailService = emailService;

        // Create table if not exists
        using var connection = new SqliteConnection(_connString);
        connection.Open();

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Contacts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Surname TEXT NOT NULL,
                Email TEXT NOT NULL,
                Message TEXT,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
            );
        ";
        tableCmd.ExecuteNonQuery();
    }

    [HttpPost]
    public IActionResult Submit(Contact contact)
    {
        if (string.IsNullOrWhiteSpace(contact.Name) ||
            string.IsNullOrWhiteSpace(contact.Surname) ||
            string.IsNullOrWhiteSpace(contact.Email))
        {
            return BadRequest("All fields are required.");
        }

        // Insert into SQLite
        using var connection = new SqliteConnection(_connString);
        connection.Open();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = @"
            INSERT INTO Contacts (Name, Surname, Email, Message) 
            VALUES ($name, $surname, $email, $message);
        ";
        insertCmd.Parameters.AddWithValue("$name", contact.Name);
        insertCmd.Parameters.AddWithValue("$surname", contact.Surname);
        insertCmd.Parameters.AddWithValue("$email", contact.Email);
        insertCmd.Parameters.AddWithValue("$message", contact.Message);
        insertCmd.ExecuteNonQuery();

        // Send email
        try
        {
            _emailService.SendContactNotification(contact.Name, contact.Surname, contact.Email, contact.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Email error: " + ex.Message);
            return StatusCode(500, "Saved to DB but email failed.");
        }

        return Ok("Contact saved and email sent.");
    }
}
