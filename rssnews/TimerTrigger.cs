using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace rssnews
{
    public static class TimerTrigger
    {
        [FunctionName("TimerTrigger")]
        public static async Task Run([TimerTrigger("0 0 10,22 * * *")]TimerInfo myTimer,
            [Queue("fetch", Connection = "AzureWebJobsStorage")]CloudQueue queue,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await queue.AddMessageAsync(new CloudQueueMessage(""));
        }
    }
}