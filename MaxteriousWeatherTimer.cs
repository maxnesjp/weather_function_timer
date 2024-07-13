using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace weatherTimer
{

    public class MaxteriousWeatherTimer
    {
        private readonly EmailService _emailService;

        public MaxteriousWeatherTimer(EmailService emailService)
        {
            _emailService = emailService;
        }

        public class FormData
        {
            public string Sender { get; set; }
            public string Message { get; set; }
        }


        [FunctionName("MaxteriousWeatherTimer")]
        public async Task<IActionResult> RunAsync([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            string requestBody = "";
            var formData = JsonConvert.DeserializeObject<FormData>(requestBody);

            var sender = formData.Sender;
            var message = formData.Message;

            if (!Utils.ValidateString(sender, 30) || !Utils.ValidateEmailFormat(sender))
            {
                return new BadRequestObjectResult(new { error = "Invalid sender email" });
            }

            if (!Utils.ValidateString(message, 500))
            {
                return new BadRequestObjectResult(new { error = "Invalid message" });
            }

            var result = await _emailService.SendEmailAsync(sender, message);
            if (result != "Email sent successfully.")
            {
                return new StatusCodeResult(500);
            }

            return new OkObjectResult(new { data = result });
        }
    }
}
