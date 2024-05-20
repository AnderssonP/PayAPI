using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayApi.Properties.Logg
{
    public class LoggerMethods
    {
        public static void LoggPayment(TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(durable));
            logger.LogInformation("making payment");
        }

        public static ILogger LoggHTTPStart(FunctionContext executionContext)
        {
            return executionContext.GetLogger("Durable_HttpStart");
        }
    }
}
