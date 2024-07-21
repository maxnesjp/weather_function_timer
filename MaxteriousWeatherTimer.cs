using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Diagnostics;
using static Utils;

namespace weatherTimer
{
    public class MaxteriousWeatherTimer
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;

        public MaxteriousWeatherTimer(HttpClient httpClient, IEmailService emailService)
        {
            _httpClient = httpClient;
            _emailService = emailService;
        }

        public class FormData
        {
            public string Sender { get; set; }
            public string Message { get; set; }
        }

        public class Customer
        {
            public string Email { get; set; }
            public string City { get; set; }
        }

        [FunctionName("MaxteriousWeatherTimer")]
        public async Task RunAsync([TimerTrigger("0,30 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Running timer function to send weather notifications: {DateTime.Now}");

            string azureCustomerEndpoint = Environment.GetEnvironmentVariable("CustomerAzureEndpoint");
            string ReSendAPI = Environment.GetEnvironmentVariable("RESEND_API");
            string weatherFunctionEndpoint = Environment.GetEnvironmentVariable("WeatherAzureEndpoint");

            if (string.IsNullOrEmpty(azureCustomerEndpoint) || string.IsNullOrEmpty(ReSendAPI) || string.IsNullOrEmpty(weatherFunctionEndpoint))
            {
                log.LogError("One or more environment variables 'CustomerAzureEndpoint', 'RESEND_API', or 'WeatherAzureEndpoint' are not set.");
                return;
            }

            var emailService = new EmailService(_httpClient, ReSendAPI);

            var customersReq = new HttpRequestMessage(HttpMethod.Get, new Uri(azureCustomerEndpoint));

            var customersResponse = await _httpClient.SendAsync(customersReq);
            if (!customersResponse.IsSuccessStatusCode)
            {
                log.LogError($"Failed to fetch customers. Status code: {customersResponse.StatusCode}");
                return;
            }

            var customersContent = await customersResponse.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<Customer[]>(customersContent);
            log.LogInformation($"Sending Email to {customers.Length}");
            foreach (Customer c in customers)
            {
                log.LogInformation($"Sending Email to {c.Email}");
                var weatherEndpoint = $"{weatherFunctionEndpoint}?city={c.City}";
                var weatherReq = new HttpRequestMessage(HttpMethod.Get, new Uri(weatherEndpoint));
                var weatherResponse = await _httpClient.SendAsync(weatherReq);
                if (!weatherResponse.IsSuccessStatusCode)
                {
                    log.LogError($"Failed to fetch weather data for city {c.City}. Status code: {weatherResponse.StatusCode}");
                    continue;
                }

                var weatherContent = await weatherResponse.Content.ReadAsStringAsync();
                WeatherApiResponse weatherData = JsonConvert.DeserializeObject<WeatherApiResponse>(weatherContent);

                // Access the condition text and code
                string conditionText = weatherData.Current.Condition.Text;
                int conditionCode = weatherData.Current.Condition.Code;
                await _emailService.SendEmailAsync(c.Email, "The weather condition in " + c.City + " is " + conditionText);
            }

            log.LogInformation("The function has finished running.");
        }
    }
}
