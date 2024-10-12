using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Blastengine;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Blastengine
{

    public class Transaction : Base
    {
        [JsonPropertyName("to")]
        public string? To { get; set; }
        [JsonPropertyName("cc")]
        public List<string> Cc { get; set; }
        [JsonPropertyName("bcc")]
        public List<string> Bcc { get; set; }

        [JsonPropertyName("insert_code")]
        public List<InsertCode> CInsertCode { get; set; }



        public Transaction()
        {
            Encode = "UTF-8";
            Cc = new List<string> { };
            Bcc = new List<string> { };
            CInsertCode = new List<InsertCode> { };
            ListUnsubscribe = new ListUnsubscribe { };
        }

        public void InsertCode(string Key, string Value)
        {
            CInsertCode.Add(new Blastengine.InsertCode($@"__{Key}__", Value));
        }

        public async Task<bool> Send()
        {
            var Path = "/v1/deliveries/transaction";
            var settings = new JsonSerializerOptions
            {
                Converters = { new DynamicPropertyConverter<Transaction>(new[] {
                    "From", "Subject", "TextPart", "HtmlPart",
                    "Encode", "To", "Cc", "Bcc", "ListUnsubscribe",
                    "CInsertCode"
                })
                },
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var Data = JsonSerializer.Serialize(this, settings);
            var obj = await Client!.PostText(Path, Data);
            DeliveryId = obj!.DeliveryId;
            return true;
        }
    }
}

