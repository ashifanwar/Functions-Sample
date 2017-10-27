using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionApps.Models
{
    public class Order : TableEntity
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}