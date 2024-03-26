using Newtonsoft.Json;
using PayPal.Api;

namespace PayApi.Properties.get
{
    public class getBankInfo
    {
        private readonly ILogger<getBankInfo> _logger;
        private readonly IConfiguration _config;

        public getBankInfo(ILogger<getBankInfo> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Function("getBankInfo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "get")] HttpRequestData req)
        {
            try
            {
                var apiContext = GetAPIContext(_config);

                var payment = new Payment()
                {
                    intent = "sale",
                    payer = new Payer() { payment_method = "paypal" },
                    transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    description = "Test betalning",
                    amount = new Amount() { currency = "SEK", total = "10" },
                }
            },
                };

                var createdPayment = payment.Create(apiContext);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");

                await response.WriteStringAsync(JsonConvert.SerializeObject(createdPayment));
                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                errorResponse.Headers.Add("Content-Type", "application/json");

                await errorResponse.WriteStringAsync(JsonConvert.SerializeObject(new { error = ex.Message }));
                return errorResponse;
            }
        }

        public static APIContext GetAPIContext(IConfiguration configen)
        {

            string clientId = configen["ClientID"];
            string clientSecret = configen["Secret"];

            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;

        }
    }
}
