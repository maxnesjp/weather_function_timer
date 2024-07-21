using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ReSendEmail : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ReSendEmail(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<string> SendEmailAsync(string sender, string message)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        var emailData = new
        {
            from = "Contact Form <onboarding@resend.dev>",
            to = new[] { sender },
            subject = "Weather Update",
            html = $"<strong>${message}</strong>",
            reply_to = sender
        };

        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(emailData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.resend.dev/emails", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "Email sent successfully.";
            }
            return "Email could not be sent.";
        }
        catch (Exception ex)
        {
            return Utils.GetErrorMessage(ex);
        }
    }
}