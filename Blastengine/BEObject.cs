using System;
using System.Text.Json.Serialization;

namespace Blastengine
{
    public class BEObject
    {
        [JsonPropertyName("delivery_id")]
        public long DeliveryId { get; set; }

        [JsonPropertyName("job_id")]
        public long JobId { get; set; }

        [JsonPropertyName("from")]
        public Blastengine.From From { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("delivery_time")]
        public DateTime? DeliveryTime { get; set; }
        [JsonPropertyName("updated_time")]
        public DateTime UpdatedTime { get; set; }
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }
        [JsonPropertyName("reservation_time")]
        public DateTime? ReservationTime { get; set; }

        [JsonPropertyName("text_part")]
        public string? TextPart { get; set; }
        [JsonPropertyName("html_part")]
        public string? HtmlPart { get; set; }
        [JsonPropertyName("delivery_type")]
        public string? DeliveryType { get; set; }
        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("total_count")]
        public long TotalCount { get; set; }
        [JsonPropertyName("sent_count")]
        public long SentCount { get; set; }
        [JsonPropertyName("drop_count")]
        public long DropCount { get; set; }
        [JsonPropertyName("hard_error_count")]
        public long HardErrorCount { get; set; }
        [JsonPropertyName("soft_error_count")]
        public long SoftErrorCount { get; set; }
        [JsonPropertyName("open_count")]
        public long OpenCount { get; set; }

        [JsonPropertyName("percentage")]
        public int Percentage { get; set; }
        [JsonPropertyName("success_count")]
        public long SuccessCount { get; set; }
        [JsonPropertyName("failed_count")]
        public long FailedCount { get; set; }
        [JsonPropertyName("error_file_url")]
        public string? ErrorFileUrl { get; set; }

        [JsonPropertyName("mail_open_file_url")]
        public string? MailOpenFileUrl { get; set; }

        public BEObject()
        {
        }
    }
}
