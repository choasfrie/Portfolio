using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Portfolio_Backend.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendContactNotification(string name, string surname, string email, string messageContent)
    {
        var from = _config["Email:From"];
        var password = _config["Email:Password"];
        var to = "AlbertoManser.portfolio@gmail.com";

        var message = new MailMessage(from, to)
        {
            Subject = "New Contact Submission",
            Body = $"Name: {name} {surname}\nEmail: {email}\n\nMessage:\n{messageContent}"
        };

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(from, password),
            EnableSsl = true
        };

        smtp.Send(message);
    }
}
