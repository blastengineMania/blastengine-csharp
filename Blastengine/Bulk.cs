using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Transactions;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Blastengine
{

    public class Bulk : Base
    {
        [JsonPropertyName("to")]
        public List<BulkTo> To { get; set; }

        public Bulk()
        {
            Encode = "UTF-8";
            To = new List<BulkTo> { };
        }

        public async Task<bool> Begin()
        {
            var Path = "/v1/deliveries/bulk/begin";
            var settings = new JsonSerializerOptions
            {
                Converters = { new DynamicPropertyConverter<Bulk>(new[] {
                    "From", "Encode", "Subject",
                    "ListUnsubscribe", "TextPart", "HtmlPart"
                }) },
            };
            var Data = JsonSerializer.Serialize(this, settings);
            var obj = await Client!.PostText(Path, Data);
            Console.WriteLine(obj!.DeliveryId);
            DeliveryId = obj!.DeliveryId;
            return true;
        }

        public async Task<bool> Update()
        {
            var Path = $@"/v1/deliveries/bulk/update/{DeliveryId}";
            var settings = new JsonSerializerOptions
            {
                Converters = { new DynamicPropertyConverter<Bulk>(new[] {
                    "From", "To", "Subject",
                    "ListUnsubscribe", "TextPart", "HtmlPart"
                }) },
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var Data = JsonSerializer.Serialize(this, settings);
            await Client!.PutText(Path, Data);
            return true;
        }

        public async Task<bool> Cancel()
        {
            var Path = $@"/v1/deliveries/{DeliveryId}/cancel";
            var obj = await Client!.PatchText(Path, null);
            DeliveryId = obj!.DeliveryId;
            return true;
        }

        public async Task<bool> Send(DateTime? ReservationTime = null)
        {
            
            var Path = ReservationTime == null ?
                $@"/v1/deliveries/bulk/commit/{DeliveryId}/immediate" :
                $@"/v1/deliveries/bulk/commit/{DeliveryId}";
            string? Params = null;
            if (ReservationTime != null)
            {
                Params = JsonSerializer.Serialize(new Blastengine.Reservation { ReservationTime = ReservationTime });
            }
            var obj = await Client!.PatchText(Path, Params);
            DeliveryId = obj!.DeliveryId;
            return true;
        }
    }
}

