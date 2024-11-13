using System;
using System.Text.Json.Serialization;
using System.Web;

namespace Blastengine
{
    public class Base
    {
        static public Client? Client;

        [JsonPropertyName("delivery_id")]
        public long DeliveryId { get; set; }

        [JsonPropertyName("from")]
        public Blastengine.From? From { get; set; }

        [JsonPropertyName("list_unsubscribe")]
        public ListUnsubscribe ListUnsubscribe { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("encode")]
        public string Encode { get; set; }

        [JsonPropertyName("text_part")]
        public string? TextPart { get; set; }

        [JsonPropertyName("html_part")]
        public string? HtmlPart { get; set; }

        [JsonIgnore]
        public List<string> Attachments { get; set; }

        public string? Status { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ReservationTime { get; set; }
        public string? DeliveryType { get; set; }

        public long TotalCount { get; set; }
        public long SentCount { get; set; }
        public long DropCount { get; set; }
        public long HardErrorCount { get; set; }
        public long SoftErrorCount { get; set; }
        public long OpenCount { get; set; }

        public Base()
        {
            Encode = "UTF-8";
            Attachments = new List<string> { };
            ListUnsubscribe = new ListUnsubscribe { };
        }

        public async Task Get()
        {
            if (DeliveryId == 0)
            {
                throw new Exception("DeliveryId is required.");
            }
            var Path = $@"/v1/deliveries/{DeliveryId}";
            var obj = await Client!.GetText(Path, null) ?? throw new Exception(@$"DeliveryId {DeliveryId} is not found.");
            DeliveryId = obj.DeliveryId;
            Status = obj.Status;
            DeliveryTime = obj.DeliveryTime;
            UpdatedTime = obj.UpdatedTime;
            CreatedTime = obj.CreatedTime;
            ReservationTime = obj.ReservationTime;
            DeliveryType = obj.DeliveryType;
            From = obj.From;
            Subject = obj.Subject;
            TextPart = obj.TextPart;
            HtmlPart = obj.HtmlPart;
            TotalCount = obj.TotalCount;
            SentCount = obj.SentCount;
            DropCount = obj.DropCount;
            HardErrorCount = obj.HardErrorCount;
            SoftErrorCount = obj.SoftErrorCount;
            OpenCount = obj.OpenCount;
        }

        public async Task<bool> Delete()
        {
            if (DeliveryId == 0)
            {
                throw new Exception("DeliveryId is required.");
            }
            var Path = $@"/v1/deliveries/{DeliveryId}";
            var obj = await Client!.DeleteText(Path);
            return obj!.DeliveryId == DeliveryId;
        }
    }
}

