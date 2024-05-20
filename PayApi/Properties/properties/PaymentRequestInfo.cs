using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayApi.Properties.get
{
    public class PaymentRequestInfo
    {
        public string Email { get; set; }
        public string Amount { get; set; }
        //public string Currency { get; set; }
        //public string Description { get; set; }
    }
}
