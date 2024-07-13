using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class EmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public EmailService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<string> SendEmailAsync(string sender, string message)
    {
        var request = new
        {
            from = "Contact Form <onboarding@resend.dev>",
            to = "nesmax.jp01@gmail.com",
            subject = "New message from Portfolio App",
            react = new
            {
                message,
                sender
            },
            reply_to = sender
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("https://api.resend.dev/emails", request);
            response.EnsureSuccessStatusCode();
            return "Email sent successfully.";
        }
        catch (Exception ex)
        {
            return Utils.GetErrorMessage(ex);
        }
    }
}
