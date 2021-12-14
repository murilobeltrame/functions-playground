using ClassLibrary.Entities;
using ClassLibrary.Shared.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MyExampleFunction
{
    public class Function2
    {
        private readonly IRepository<Product> _repository;

        public Function2(IRepository<Product> repository)
        {
            _repository = repository;
        }

        [FunctionName("Function2")]
        public async Task Run(
            [ServiceBusTrigger("product_changed", "product_changed_subscription", Connection = "BrokerConnectionString")] string message, 
            string messageId,
            string correlationId,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");

            var product = Product.FromJson(message);
            await _repository.UpsertAsync(product);
        }
    }
}
