using System.Text.Json.Serialization;

namespace Blastengine
{
    public struct From
    {
        [JsonPropertyName("email")]
        public string Email { get; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public From(string Email, string Name = "")
        {
            this.Email = Email;
            this.Name = Name;
        }
    }

    public struct InsertCode
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }

        public InsertCode(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

    }

    public struct BulkTo
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("insert_code")]
        public List<InsertCode>? CInsertCode { get; set; }

        public BulkTo(string Email, List<InsertCode>? CInsertCode = null)
        {
            this.Email = Email;
            if (CInsertCode != null) {
                this.CInsertCode = CInsertCode.Select(insertCode => new Blastengine.InsertCode($@"__{insertCode.Key}__", insertCode.Value)).ToList();
            } else
            {
                this.CInsertCode = new List<InsertCode> { };
            }
        }

    }
    public struct ListUnsubscribe
    {
        [JsonPropertyName("mailto")]
        public string? MailTo { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        public ListUnsubscribe(string? Email, string? Url)
        {
            if (Email == null)
            {
                MailTo = "";
            }
            else
            {
                MailTo = $@"mailto:{Email}";
            }
            this.Url = Url;
        }
    }

    public struct Attachement
    {
        [JsonPropertyName("delivery_attach_id")]
        public long DeliveryAttachId { get; set; }
        [JsonPropertyName("file_name")]
        public string? FileName { get; set; }
        [JsonPropertyName("mime")]
        public string? Mime { get; set; }
        [JsonPropertyName("size")]
        public long Size { get; set; }
        [JsonPropertyName("created_time")]
        public string? CreatedTime { get; set; }
        [JsonPropertyName("updated_time")]
        public string? UpdatedTime { get; set; }
    }

    public struct Reservation
    {
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonPropertyName("reservation_time")]
        public DateTime? ReservationTime { get; set; }
    }
}

