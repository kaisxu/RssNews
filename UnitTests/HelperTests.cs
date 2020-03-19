using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace rssnews
{
    [TestClass]
    public class HelperTests
    {
        private const string RssString = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<rss version=""2.0"" xmlns:itunes=""http://www.itunes.com/dtds/podcast-1.0.dtd"" xmlns:googleplay=""http://www.google.com/schemas/play-podcasts/1.0"" xmlns:atom=""http://www.w3.org/2005/Atom"" xmlns:media=""http://search.yahoo.com/mrss/"" xmlns:content=""http://purl.org/rss/1.0/modules/content/"">
  <channel>
    <atom:link href=""https://feeds.megaphone.fm/WSJ8523681216?limit=20"" rel=""self"" type=""application/rss+xml""/>
    <title>WSJ Tech News Briefing</title>
    <link>https://www.wsj.com/podcasts/tech-news-briefing</link>
    <language>en-us</language>
    <copyright>Copyright Dow Jones &amp; Company, Inc. All Rights Reserved.</copyright>
    <description>Get the latest in technology news for your weekday commute. The Wall Street Journal's reporters and editors highlight leading companies, new gadgets, consumer trends and cyber issues. From San Francisco to New York to the hottest conferences, our journalists help you stay plugged in..</description>
    <image>
      <url>https://megaphone-prod.s3.amazonaws.com/podcasts/786a40de-9f81-11e5-8c1b-9792cea21589/image/im-29358.jpeg</url>
      <title>WSJ Tech News Briefing</title>
      <link>https://www.wsj.com/podcasts/tech-news-briefing</link>
    </image>
    <itunes:explicit>no</itunes:explicit>
    <itunes:type>episodic</itunes:type>
    <itunes:subtitle></itunes:subtitle>
    <itunes:author>The Wall Street Journal</itunes:author>
    <itunes:summary>Get the latest in technology news for your weekday commute. The Wall Street Journal's reporters and editors highlight leading companies, new gadgets, consumer trends and cyber issues. From San Francisco to New York to the hottest conferences, our journalists help you stay plugged in..</itunes:summary>
    <content:encoded>
      <![CDATA[Get the latest in technology news for your weekday commute. The Wall Street Journal's reporters and editors highlight leading companies, new gadgets, consumer trends and cyber issues. From San Francisco to New York to the hottest conferences, our journalists help you stay plugged in..]]>
    </content:encoded>
    <itunes:owner>
      <itunes:name>The Wall Street Journal</itunes:name>
      <itunes:email>podcasts@dowjones.com</itunes:email>
    </itunes:owner>
    <itunes:image href=""https://megaphone-prod.s3.amazonaws.com/podcasts/786a40de-9f81-11e5-8c1b-9792cea21589/image/im-29358.jpeg""/>
    <itunes:category text=""News"">
      <itunes:category text=""Tech News""/>
    </itunes:category>
    <item>
      <title>Working From Home: Coronavirus Edition</title>
      <description>Silicon Valley's major tech companies were the first U.S. employers to take their operations online because of the coronavirus. As the rest of the country prepares to follow their lead, our reporter Rob Copeland joins us to explain how they did it - and how we can avoid their mistakes. And: Senior Personal Tech Columnist Joanna Stern gives her best work-from-home tech tips. Have a question for Joanna, or want to share your own experience? Leave her a message at ‪(314) 635-0388‬ or email her at joanna.stern@wsj.com. Kateri Jochum hosts.
Learn more about your ad choices. Visit megaphone.fm/adchoices</description>
      <pubDate>Tue, 17 Mar 2020 07:00:00 -0000</pubDate>
      <itunes:episodeType>full</itunes:episodeType>
      <itunes:author>The Wall Street Journal</itunes:author>
      <itunes:subtitle/>
      <itunes:summary>Silicon Valley's major tech companies were the first U.S. employers to take their operations online because of the coronavirus. As the rest of the country prepares to follow their lead, our reporter Rob Copeland joins us to explain how they did it - and how we can avoid their mistakes. And: Senior Personal Tech Columnist Joanna Stern gives her best work-from-home tech tips. Have a question for Joanna, or want to share your own experience? Leave her a message at ‪(314) 635-0388‬ or email her at joanna.stern@wsj.com. Kateri Jochum hosts.
Learn more about your ad choices. Visit megaphone.fm/adchoices</itunes:summary>
      <content:encoded>
        <![CDATA[Silicon Valley's major tech companies were the first U.S. employers to take their operations online because of the coronavirus. As the rest of the country prepares to follow their lead, our reporter Rob Copeland joins us to explain how they did it - and how we can avoid their mistakes. And: Senior Personal Tech Columnist Joanna Stern gives her best work-from-home tech tips. Have a question for Joanna, or want to share your own experience? Leave her a message at ‪(314) 635-0388‬ or email her at joanna.stern@wsj.com. Kateri Jochum hosts.<p> </p><p>Learn more about your ad choices. Visit <a href=""https://megaphone.fm/adchoices"">megaphone.fm/adchoices</a></p>]]>
      </content:encoded>
      <itunes:duration>966</itunes:duration>
      <guid isPermaLink=""false""><![CDATA[100da364-681d-11ea-9c51-8f22d33e77a9]]></guid>
      <enclosure url=""https://traffic.megaphone.fm/WSJ4677319017.mp3"" length=""0"" type=""audio/mpeg""/>
    </item>
    <item>
      <title>New Federal Rules Ease Access to Your Health Data</title>
      <description>The federal government has introduced new rules that will make it easier for tech companies to access health data. Reporter Melanie Evans joins us to explain. Plus, tech columnist Christopher Mims shares tips for coping with the work-from-home blues. Kateri Jochum hosts.
Learn more about your ad choices. Visit megaphone.fm/adchoices</description>
      <pubDate>Mon, 16 Mar 2020 07:00:00 -0000</pubDate>
      <itunes:episodeType>full</itunes:episodeType>
      <itunes:author>The Wall Street Journal</itunes:author>
      <itunes:subtitle/>
      <itunes:summary>The federal government has introduced new rules that will make it easier for tech companies to access health data. Reporter Melanie Evans joins us to explain. Plus, tech columnist Christopher Mims shares tips for coping with the work-from-home blues. Kateri Jochum hosts.
Learn more about your ad choices. Visit megaphone.fm/adchoices</itunes:summary>
      <content:encoded>
        <![CDATA[The federal government has introduced new rules that will make it easier for tech companies to access health data. Reporter Melanie Evans joins us to explain. Plus, tech columnist Christopher Mims shares tips for coping with the work-from-home blues. Kateri Jochum hosts. <p> </p><p>Learn more about your ad choices. Visit <a href=""https://megaphone.fm/adchoices"">megaphone.fm/adchoices</a></p>]]>
      </content:encoded>
      <itunes:duration>658</itunes:duration>
      <guid isPermaLink=""false""><![CDATA[e64c90f4-6753-11ea-8f47-33ab90572dda]]></guid>
      <enclosure url=""https://traffic.megaphone.fm/WSJ1100364074.mp3"" length=""0"" type=""audio/mpeg""/>
    </item>
  </channel>
</rss>
";

        [TestMethod]
        public void TestRssParse()
        {
            var doc = XDocument.Parse(RssString);
            var eps = Helpers.ParseEpisodes(doc);
            Assert.AreEqual(2, eps.Count());
            var ep1 = eps.First();
            var ep2 = eps.Last();
            Assert.AreEqual("https://traffic.megaphone.fm/WSJ4677319017.mp3", ep1.Address);
            Assert.AreEqual(new DateTime(2020, 3, 17, 7, 0, 0, DateTimeKind.Utc), ep1.PublishDate.ToUniversalTime());
            Assert.AreEqual(false, ep1.Played);
            Assert.AreNotEqual(ep1.PartitionKey, ep2.PartitionKey);
        }
    }
}