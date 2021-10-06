using System;
using System.Collections.Generic;
using System.Text;
using TheSwamp.Shared;

namespace TheSwamp.Api.DAL.IOT.Entities
{
    public class Message
    {
        public Message()
        {
            Properties = new List<MessageProperty>();
            TimestampUtc = DateTime.UtcNow;
        }

        public long Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string ClientIp { get; set; }
        public string Type { get; set; }

        public ICollection<MessageProperty> Properties { get; set; }
    }


    public class MessageProperty : IProperty
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public Message Message { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
