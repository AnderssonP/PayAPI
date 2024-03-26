namespace PayApi.Properties
{
    public static class makePayment
    {
        [Function(nameof(makePayment))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var emailData = context.GetInput<EmailData>();

            await context.CallActivityAsync("SendEmailConfirmation", emailData);
        }

        [Function("makePayment_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("makePayment_HttpStart");

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(makePayment));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}

