using FunctionApps.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Http;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;

namespace API
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
            var client = QueueClient.CreateFromConnectionString(SBConn, QueueName);
            await client.SendAsync(new BrokeredMessage(message));
        }
    }
}