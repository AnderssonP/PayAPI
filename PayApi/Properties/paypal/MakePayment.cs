namespace PayApi.Properties.paypal
{
    public class MakePayment
    {
        private readonly ILogger<MakePayment> _logger;
        private readonly IConfiguration _config;

        public MakePayment(ILogger<MakePayment> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Function(nameof(PaypalPayment))]
        public async Task<string> PaypalPayment([ActivityTrigger] string amount)
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
                            amount = new Amount() { currency = "USD", total = amount }
                        }
                    },

                    redirect_urls = new RedirectUrls()
                    {
                        return_url = "https://example.com/return",
                        cancel_url = "https://example.com/cancel"
                    }
                };

                var createdPayment = payment.Create(apiContext);

                return JsonSerializer.Serialize(createdPayment);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        private static APIContext GetAPIContext(IConfiguration configen)
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
