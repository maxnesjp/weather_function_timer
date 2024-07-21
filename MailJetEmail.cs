using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

public class MailJetEmail : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public MailJetEmail(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }


    public async Task<string> SendEmailAsync(string sender, string message)
    {
        MailjetClient client = new MailjetClient(Environment.GetEnvironmentVariable("MJ_APIKEY_PUBLIC"), Environment.GetEnvironmentVariable("MJ_APIKEY_PRIVATE"));
        MailjetRequest request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
           .Property(Send.FromEmail, "pilot@mailjet.com")
           .Property(Send.FromName, "Mailjet Pilot")
           .Property(Send.Subject, "Your email flight plan!")
           .Property(Send.TextPart, "Dear passenger, welcome to Mailjet! May the delivery force be with you!")
           .Property(Send.HtmlPart, "<h3>Dear passenger, welcome to <a href=\"https://www.mailjet.com/\">Mailjet</a>!<br />May the delivery force be with you!")
           .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email", "passenger@mailjet.com"}
                 }
               });
        MailjetResponse response = await client.PostAsync(request);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            Console.WriteLine(response.GetData());
            return "Email sent successfully.";
        }
        else
        {
            Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
            Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            return "Email could not be sent.";
        }
    }
}