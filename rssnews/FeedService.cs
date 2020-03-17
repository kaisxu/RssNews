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
            new Station(){ Name = "WSJ", Address="https://video-api.wsj.com/podcast/rss/wsj/the-journal"},

            new Station(){ Name = "WSJ Tech", Address="https://video-api.wsj.com/podcast/rss/wsj/tech-news-briefing"}
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

                var blob = container.GetBlockBlobReference(ep.PartitionKey);
                await Download(ep, blob, log);
                TableOperation tableOperation = TableOperation.Insert(ep);
                await episodeList.ExecuteAsync(tableOperation);

                // remove old data
                var op = new TableBatchOperation();
                var eps = (await episodeList.ExecuteQuerySegmentedAsync(new TableQuery<Episode>(), null))
                    .Where(e => e.Timestamp < DateTime.Now - TimeSpan.FromDays(7));
                foreach (var e in eps)
                {
                    await container.GetBlockBlobReference(e.PartitionKey).DeleteAsync();
                    op.Add(TableOperation.Delete(e));
                }
                if (op.Count() > 0)
                {
                    await episodeList.ExecuteBatchAsync(op);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "FeedService failed:");
                throw;
            }
        }

        private static async Task<IEnumerable<Episode>> ReadRss(Station station, ILogger log)
        {
            string uri = station.Address;
            log.LogInformation($"ReadRss {uri}");
            var content = await client.GetAsync(uri);

            var doc = XDocument.Load(await content.Content.ReadAsStreamAsync());

            return doc.XPathSelectElements("//item").Select(e => new Episode()
            {
                PartitionKey = MD5(e.Element("enclosure").Attribute("url").Value),
                RowKey = string.Empty,
                Played = false,
                PublishDate = DateTime.Parse(e.Element("pubDate").Value),
                Address = e.Element("enclosure").Attribute("url").Value
            });
        }

        private static async Task Download(Episode eps, CloudBlockBlob blob, ILogger log)
        {
            log.LogInformation($"Download {eps.Address}");
            var content = await client.GetAsync(eps.Address);
            await blob.UploadFromStreamAsync(await content.Content.ReadAsStreamAsync());
        }

        private static string MD5(string str)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
