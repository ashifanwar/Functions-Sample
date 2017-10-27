using System;
using System.IO;
using FunctionApps.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApps.OrderReceive
{
    public class OrderReceiveFunction
    {
        public static async void Run(Order orderItem, TraceWriter log, IAsyncCollector<Order> outputTable, IBinder binder)
        {
            log.Info($"Order {orderItem.OrderId} received from {orderItem.Email} for Product {orderItem.ProductId}");
            orderItem.PartitionKey = "order";
            orderItem.RowKey = orderItem.OrderId;

            await outputTable.AddAsync(orderItem);
            log.Info("Order stored");

            using (var outputBlob = binder.Bind<TextWriter>(
                new BlobAttribute($"licenses/{orderItem.OrderId}.lic")))
            {

                log.Info($"Received order of {orderItem.OrderId}, Product {orderItem.ProductId}");
                outputBlob.WriteLine($"OrderId: {orderItem.OrderId}");
                outputBlob.WriteLine($"ProductId: {orderItem.ProductId}");
                outputBlob.WriteLine($"Email: {orderItem.Email}");
                outputBlob.WriteLine($"DateOfPurchase: {DateTime.UtcNow}");
                var md5 = System.Security.Cryptography.MD5.Create();
                var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(orderItem.Email + "secret"));
                outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");
                log.Info($"Stored in blob order of {orderItem.OrderId}, Product {orderItem.ProductId}");
            }


        }
    }
}
