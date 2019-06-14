using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class ChannelWebhookEntity : TableEntity
    {
        public string Channel { get; set; }
        public string Webhook { get; set; }

        public ChannelWebhookEntity()
        {
        }

        public ChannelWebhookEntity(string channel, string webhook)
        {
            RowKey = channel;
            PartitionKey = channel;
            Channel = channel;
            Webhook = webhook;
        }
    }
}