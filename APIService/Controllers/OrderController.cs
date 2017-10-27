using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using FunctionApps.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace APIService.Controllers
{
    [RoutePrefix("functionapp/v1")]
    public class OrderController : ApiController
    {
        private readonly string StorageConn = ConfigurationManager.AppSettings["StorageConnectionString"];
        private readonly string SBConn = ConfigurationManager.AppSettings["SBConnectionString"];
        private const string QueueName = "Orders";

        [Route("sendorder")]
        public async Task<IHttpActionResult> Post()
        {
            var jsonContent = await Request.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(jsonContent);

            await QueueOrder(order);

            return Ok();
        }

        public async Task QueueOrder(Order message)
        {
            await EnsureExists(QueueName);
            var client = QueueClient.CreateFromConnectionString(SBConn, QueueName);
            await client.SendAsync(new BrokeredMessage(message));
        }

        private async Task EnsureExists(string queue)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(SBConn);
            if (!await namespaceManager.QueueExistsAsync(queue))
            {
                await namespaceManager.CreateQueueAsync(queue);
            }
        }
    }
}
