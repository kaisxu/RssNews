using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace rssnews
{
    public static class FrontEnd
    {
        private const string ResponseTemplate = @"
<audio controls autoplay onended='location.reload();'>
  <source src = '{0}' type='audio/mpeg'>
  Your browser does not support the audio tag.
</audio>
";

        [FunctionName("FrontEnd")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("fetch", Connection = "AzureWebJobsStorage")]CloudQueue queue,
            [Blob("buffer", FileAccess.Write, Connection = "AzureWebJobsStorage")]CloudBlobContainer container,
            [Table("episode", Connection = "AzureWebJobsStorage")]CloudTable episodeList,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await queue.AddMessageAsync(new CloudQueueMessage(""));
                var ep = episodeList.ExecuteQuerySegmentedAsync(new TableQuery<Episode>(), null)
                    .GetAwaiter().GetResult()
                    .OrderBy(e => e.Timestamp).Last();
                ep.Played = true;
                var uri = container.GetBlockBlobReference(ep.PartitionKey).Uri;

                await episodeList.ExecuteAsync(TableOperation.InsertOrReplace(ep));

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(string.Format(ResponseTemplate, uri));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
            catch (Exception e)
            {
                log.LogError(e, "FrontEnd failed:");
                throw;
            }
        }
    }
}