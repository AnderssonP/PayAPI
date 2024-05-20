using PayApi.Properties.get;
using PayApi.Properties.Logg;
using PayApi.Properties.paypal;
using PayApi.Properties.properties;
using PayPal.Api;

namespace PayApi
{
    public static class durable
    {

        [Function(nameof(durable))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            LoggerMethods.LoggPayment(context);

            var paymentRequest = context.GetInput<PaymentRequestInfo>();

            var outputs = new List<string>();

            var paymentResult = await context.CallActivityAsync<string>(nameof(MakePayment.PaypalPayment), paymentRequest.Amount);
            SendEmail.SendEmailOnSuccessfulPayment(paymentResult, outputs, paymentRequest);

            return outputs;
           
        }

        [Function("Durable_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            var logger = LoggerMethods.LoggHTTPStart(executionContext);

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var paymentRequest = JsonSerializer.Deserialize<PaymentRequestInfo>(requestBody);

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(durable),paymentRequest);

            logger.LogInformation($"Started orchestration with ID = '{instanceId}'.", instanceId);

            return client.CreateCheckStatusResponse(req, instanceId);
        }

    }
}
