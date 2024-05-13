using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace PayApi.Properties
{
    public static class MakePayment
    {
        [FunctionName("startPayment")]
        public static async Task RunOrchestrator([Microsoft.Azure.Functions.Worker.OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var result = await context.CallActivityAsync<string>("getBankInfo", null);

            if (result == "OK")  
            {
                var emailData = new EmailData
                {
                    Email = "pontus.andersson1998@gmail.com",
                    Subject = "Betalningsbekräftelse",
                    Message = "Din betalning på 10 SEK har mottagits."
                };

                await context.CallActivityAsync("SendEmailConfirmation", emailData);
            }
        }

        [FunctionName("StartOrchestration")]
        public static async Task<HttpResponseData> Start(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
        [Microsoft.Azure.Functions.Worker.DurableClient] IDurableOrchestrationClient starter)
        {
            string instanceId = await starter.StartNewAsync("OrchestratorFunction", null);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Started orchestration with ID = '{instanceId}'.");
            return response;
        }

    }
}
