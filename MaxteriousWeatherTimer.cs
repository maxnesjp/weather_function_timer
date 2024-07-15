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

        public MaxteriousWeatherTimer(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
        public async Task RunAsync([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Running timer function to send weather notifications: {DateTime.Now}");

            string azureCustomerEndpoint = Environment.GetEnvironmentVariable("CustomerAzureEndpoint");
            string azureWeatherEndpoint = Environment.GetEnvironmentVariable("WeatherAzureEndpoint");
            string ReSendAPI = Environment.GetEnvironmentVariable("RESEND_API");
            if (string.IsNullOrEmpty(azureCustomerEndpoint) || string.IsNullOrEmpty(azureWeatherEndpoint) || string.IsNullOrEmpty(ReSendAPI))
            {
                log.LogError("Environment variables 'GetCustomersAzureEndpoint' or 'RESEND_API' are not set.");
                return;
            }
            var emailService = new EmailService(_httpClient, ReSendAPI);

            var customersReq = new HttpRequestMessage(HttpMethod.Get, azureCustomerEndpoint);

            var customersResponse = await _httpClient.SendAsync(customersReq);
            var customersContent = await customersResponse.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<Customer[]>(customersContent);

            foreach (Customer c in customers)
            {
                var weatherReq = new HttpRequestMessage(HttpMethod.Get, $"${azureWeatherEndpoint}?city={c.City}");
                var weatherResponse = await _httpClient.SendAsync(weatherReq);
                var weatherContent = await weatherResponse.Content.ReadAsStringAsync();
                WeatherApiResponse weatherData = JsonConvert.DeserializeObject<WeatherApiResponse>(weatherContent);

                // Access the condition text and code
                string conditionText = weatherData.Current.Condition.Text;
                int conditionCode = weatherData.Current.Condition.Code;
                await emailService.SendEmailAsync(c.Email, "The weather condition in " + c.City + " is " + conditionText);
            }

            log.LogInformation("The function has finished running.");
        }
    }
}
