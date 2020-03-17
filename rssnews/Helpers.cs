using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

[assembly: InternalsVisibleTo("UnitTests")]

namespace rssnews
{
    internal static class Helpers
    {
        public static IEnumerable<Episode> ParseEpisodes(XDocument document)
        {
            return document.XPathSelectElements("//item").Select(e => new Episode()
            {
                PartitionKey = MD5(e.Element("enclosure").Attribute("url").Value),
                RowKey = string.Empty,
                Played = false,
                PublishDate = DateTime.Parse(e.Element("pubDate").Value),
                Address = e.Element("enclosure").Attribute("url").Value
            });
        }

        public static string MD5(string str)
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
