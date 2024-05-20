using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayApi.Properties.properties
{
    public class PaymentDataInfo
    {
        public string id { get; set; }
        public string State { get; set; }
        public string intent { get; set; }
        public PayerData payer { get; set; }
        public List<TransactionData> transactions { get; set; }
        public RedirectUrlsData redirect_urls { get; set; }
    }

    public class PayerData
    {
        public string payment_method { get; set; }
    }

    public class TransactionData
    {
        public string description { get; set; }
        public AmountData amount { get; set; }
    }

    public class AmountData
    {
        public string currency { get; set; }
        public string total { get; set; }
    }

    public class RedirectUrlsData
    {
        public string returnUrl { get; set; }
        public string cancelUrl { get; set; }
    }

}
