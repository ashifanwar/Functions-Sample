using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FunctionApps.Models;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Service.Controllers
{
    [RoutePrefix("function/v1")]
    public class OrdersController : ApiController
    {
        private readonly string _sbConn = ConfigurationManager.AppSettings["StorageConnectionString"];
        private const string QueueName = "Orders";


        [Route("order")]
        public async Task<IHttpActionResult> Post()
        {
            var jsonContent = await Request.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(jsonContent);

            await QueueOrder(order);

            return Ok();
        }

        public async Task QueueOrder(Order message)
        {
            var client = QueueClient.CreateFromConnectionString(_sbConn, QueueName);
            await client.SendAsync(new BrokeredMessage(message));
        }

    }
}
