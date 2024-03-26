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
        public async Task<HttpResponseData> SendEmailConfirmation(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
        FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SendEmailConfirmation");
            logger.LogInformation("Starting to process SendEmail function for payment");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var emailData = JsonSerializer.Deserialize<EmailData>(requestBody);

            try
            {
                var email = emailData?.Email ?? "missing";
                var subject = emailData?.Subject ?? "missing";
                var message = emailData?.Message ?? "missing";

                logger.LogInformation($"Received email: {email}");
                logger.LogInformation($"Subject: {subject}");
                logger.LogInformation($"Message: {message}");

                SendEmail.sendEmailToCustomer(email, subject, message);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Email sent to {email}");
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("An error occurred processing your request.");
                return response;
            }
        }

        public class EmailData
        {
            public string Email { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
        }
    }
}
