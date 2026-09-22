using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Order.Repository;
using Order.Repository.Services;
using System;
using System.Threading.Tasks;

namespace SOHGenerateShippingLabel
{
    public class CreateShippingLabel
    {
        private readonly ILogger _logger;
        private readonly ISalesOrderRepository _salesOrderRepo;

        public CreateShippingLabel(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SOHOrderContextFactory>();
            var context = new SOHOrderContextFactory().CreateDbContext();
            _salesOrderRepo = new SalesOrderRepository(context);
        }

        // 0 * * * * *	Every minute at 0th second
        // */30 * * * * *	Every 30 seconds
        // 0 */5 * * * *	Every 5 minutes
        [FunctionName("CreateShippingLabel")]
        public async Task Run([TimerTrigger("*/15 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            await GenerateShippingLabelForSO(log);
        }

        public async Task<bool> GenerateShippingLabelForSO(ILogger log)
        {
            var retval = false;

            try
            {
                var orders = await _salesOrderRepo.GenerateShippingLabel(log).ConfigureAwait(false);

                if (orders)
                {
                    log.LogInformation("Pending sales orders shipping label generate successfully.");
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while generating the shipping label for the pending sales order");
                retval = false;
            }

            return retval;
        }
    }
}
