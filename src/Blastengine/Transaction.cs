using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Blastengine
{

    public class Transaction
    {
        static public Client Client;

        [JsonProperty("delivery_id")]
        public long DeliveryId { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("cc")]
        public List<string> Cc { get; set; }
        [JsonProperty("bcc")]
        public List<string> Bcc { get; set; }

        [JsonProperty("insert_code")]
        public List<InsertCode> CInsertCode { get; set; }

        [JsonProperty("list_unsubscribe")]
        public ListUnsubscribe ListUnsubscribe { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("encode")]
        public string Encode { get; set; }

        [JsonProperty("text_part")]
        public string TextPart { get; set; }

        [JsonProperty("html_part")]
        public string HtmlPart { get; set; }

        public Transaction()
        {
            Encode = "UTF-8";
            From = new From();
            Cc = new List<string> { };
            Bcc = new List<string> { };
            CInsertCode = new List<InsertCode> { };
        }

        public void InsertCode(string Key, string Value)
        {
            CInsertCode.Add(new Blastengine.InsertCode($@"__{Key}__", Value));
        }
        
        public async Task<bool> Send()
        {
            var Path = "/v1/deliveries/transaction";
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropertiesResolver(new[] { "delivery_id" })
            };
            var Data = JsonConvert.SerializeObject(this, settings);
            var obj = await Client.PostText(Path, Data);
            DeliveryId = obj.DeliveryId;
            return true;
        }
    }
}

