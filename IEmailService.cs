using System.Threading.Tasks;

public interface IEmailService
{
    Task<string> SendEmailAsync(string sender, string message);
}