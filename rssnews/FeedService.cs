using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace rssnews
{
    public static class FeedService
    {
        private static HttpClient client = new HttpClient();

        private static readonly Station[] stations =
        {
            // uri can be get from itune
            new Station(){ Name = "WSJ Journal", Address="https://video-api.wsj.com/podcast/rss/wsj/the-journal"},
            new Station(){ Name = "WSJ Business", Address="https://video-api.wsj.com/podcast/rss/wsj/whats-news"},
            new Station(){ Name = "WSJ Opinion", Address="https://video-api.wsj.com/podcast/rss/wsj/opinion-potomac-watch"},
            new Station(){ Name = "WSJ Future", Address="https://video-api.wsj.com/podcast/rss/wsj/wsj-the-future-of-everything"},
            new Station(){ Name = "Morgan Stanley Ideas", Address="https://rss.art19.com/morgan-stanley-ideas-podcast"}
        };

        [FunctionName("FeedService")]
        public static async Task Run([QueueTrigger("fetch", Connection = "AzureWebJobsStorage")]string queueItem,
            [Blob("buffer", FileAccess.Write, Connection = "AzureWebJobsStorage")]CloudBlobContainer container,
            [Table("episode", Connection = "AzureWebJobsStorage")]CloudTable episodeList,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueItem}");

            try
            {
                // download new data
                var eplist = stations
                    .Select(s => ReadRss(s, log).GetAwaiter().GetResult())
                    .Aggregate((a, b) => a.Concat(b))
                    .OrderByDescending(e => e.PublishDate);
                Episode ep = new Episode() { PartitionKey = string.Empty };
                foreach (var e in eplist)
                {
                    var existing = await episodeList.ExecuteQuerySegmentedAsync(new TableQuery()
                        .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, e.PartitionKey)), null);
                    if (existing.Count() == 0)
                    {
                        ep = e;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(ep.PartitionKey))
                {
                    return;
                }

                log.LogInformation($"PartitionKey {ep.PartitionKey}");

                var blob = container.GetBlockBlobReference(ep.PartitionKey);
                await Download(ep, blob, log);
                TableOperation tableOperation = TableOperation.Insert(ep);
                await episodeList.ExecuteAsync(tableOperation);

                // remove old data
                var eps = (await episodeList.ExecuteQuerySegmentedAsync(new TableQuery<Episode>(), null))
                    .Where(e =>
                        (e.Timestamp < (DateTime.Now - TimeSpan.FromDays(3)) && e.Played)
                        || (e.Timestamp < (DateTime.Now - TimeSpan.FromDays(15))));
                foreach (var e in eps)
                {
                    log.LogInformation($"Remove {e.PartitionKey}");
                    await container.GetBlockBlobReference(e.PartitionKey).DeleteIfExistsAsync();
                    await episodeList.ExecuteAsync(TableOperation.Delete(e));
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "FeedService failed");
                throw;
            }
        }

        private static async Task<IEnumerable<Episode>> ReadRss(Station station, ILogger log)
        {
            string uri = station.Address;
            log.LogInformation($"ReadRss {uri}");
            var content = await client.GetAsync(uri);

            var doc = XDocument.Load(await content.Content.ReadAsStreamAsync());
            var res = Helpers.ParseEpisodes(doc, station);
            var sample = res.First();
            log.LogInformation($"Sampe for {station.Name}, Uri: {sample.Address}, Publish Data: {sample.PublishDate}, PK: {sample.PartitionKey}");

            return res;
        }

        private static async Task Download(Episode eps, CloudBlockBlob blob, ILogger log)
        {
            log.LogInformation($"Download {eps.Address}");
            var content = await client.GetAsync(eps.Address);
            await blob.UploadFromStreamAsync(await content.Content.ReadAsStreamAsync());
            log.LogInformation($"Download {eps.Address} success");
        }
    }
}