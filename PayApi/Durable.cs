using PayApi.Properties.get;
using PayPal.Api;

namespace PayApi
{
    public static class durable
    {

        [Function(nameof(durable))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
           
            ILogger logger = context.CreateReplaySafeLogger(nameof(durable));
            logger.LogInformation("makeing payment");

            var paymentRequest = context.GetInput<PaymentRequest>();

            var outputs = new List<string>();
            var paymentId = "";

            var paymentResult = await context.CallActivityAsync<string>(nameof(getBankInfo.PaypalPayment), paymentRequest.Amount);
            var paymentData = JsonSerializer.Deserialize<PaymentData>(paymentResult);

            if (paymentData != null && string.IsNullOrEmpty(paymentData.State) && paymentData.State != "error")
            {
                var emailString = $"Payment from {paymentData.payer.payment_method} of {paymentData.transactions.FirstOrDefault()?.amount.total} {paymentData.transactions.FirstOrDefault()?.amount.currency}";
                paymentId = paymentData.id;
                outputs.Add(paymentId);
                SendEmail.sendEmailToCustomer(paymentRequest.Email, "Payment Confirmation", emailString);
            }

            return outputs;
           
        }

        [Function("Durable_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("Durable_HttpStart");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var paymentRequest = JsonSerializer.Deserialize<PaymentRequest>(requestBody);

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(durable),paymentRequest);

            logger.LogInformation($"Started orchestration with ID = '{instanceId}'.", instanceId);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
