using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PayApi.Properties.email;
using System.Text.Json;

namespace PayApi
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("SendEmailConfirmation")]
        public async Task<HttpResponseData> SendEmailConfirmation([HttpTrigger(AuthorizationLevel.Function, "post", Route = "kassa/payment/Done")] HttpRequestData req)
        {
            _logger.LogInformation("HTTP trigger processed a SendEmail function for payment");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(requestBody);

                var email = data.TryGetProperty("email", out JsonElement emailElement) ? emailElement.GetString() : "missing";
                var subject = data.TryGetProperty("subject", out JsonElement subjectElement) ? subjectElement.GetString() : "missing";
                var message = data.TryGetProperty("message", out JsonElement messageElement) ? messageElement.GetString() : "missing";

                _logger.LogInformation($"Received email: {email}");
                _logger.LogInformation($"Subject: {subject}");
                _logger.LogInformation($"Message: {message}");

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteStringAsync("Email data received and logged.");
                SendEmail.sendEmailToCustomer(email,subject,message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                var response = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An error occurred processing your request.");
                return response;
            }
        }
    }
}
