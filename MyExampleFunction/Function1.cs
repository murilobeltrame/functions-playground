using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ClassLibrary.Entities;
using Microsoft.Azure.ServiceBus;
using System;

namespace MyExampleFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        [return: ServiceBus("product_changed", Connection = "BrokerConnectionString")]
        public static async Task<Message> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var cmsProduct = await LegacyProduct.FromStreamAsync(req.Body);
            return new Message
            {
                CorrelationId = Guid.NewGuid().ToString(),
                Body = Product
                    .FromCmsProduct(cmsProduct)
                    .ToBytes()
            };
        }
    }
}
