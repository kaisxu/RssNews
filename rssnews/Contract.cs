using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace rssnews
{
    internal class Station
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }

    internal class Episode : TableEntity
    {
        public string Address { get; set; }
        public bool Played { get; set; }
        public DateTime PublishDate { get; set; }
        public string Title;
    }
}
