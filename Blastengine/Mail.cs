using System;
using System.Text.Json.Serialization;

namespace Blastengine
{
    public class Mail : Base
    {
        [JsonPropertyName("to")]
        public List<BulkTo> To { get; set; }

        [JsonPropertyName("cc")]
        public List<string> Cc { get; set; }
        [JsonPropertyName("bcc")]
        public List<string> Bcc { get; set; }

        public Mail()
        {
            Encode = "UTF-8";
            To = new List<BulkTo> { };
            Cc = new List<string> { };
            Bcc = new List<string> { };
            ListUnsubscribe = new ListUnsubscribe { };
            Attachments = new List<string> { };

        }

        public async Task<bool> Send(DateTime? ReservationTime = null)
        {
            if (ReservationTime != null)
            {
                // Bulk
                if (Cc.Count > 0 || Bcc.Count > 0)
                {
                    throw new Exception("Don't set reservation time with cc or bcc");
                }
            }
            if (To.Count > 1)
            {
                if (Cc.Count > 0 || Bcc.Count > 0)
                {
                    throw new Exception("Don't set multiple to with cc or bcc");
                }
                return await SendBulk(ReservationTime);
            }
            else
            {
                return await SendTransaction();
            }
        }

        private async Task<bool> SendBulk(DateTime? ReservationTime = null)
        {
            var bulk = new Bulk
            {
                Subject = Subject,
                TextPart = TextPart,
                HtmlPart = HtmlPart,
                Encode = Encode,
                Attachments = Attachments,
                To = To,
                From = From,
                ListUnsubscribe = ListUnsubscribe
            };
            _ = await bulk.Begin();
            _ = await bulk.Update();
            _ = await bulk.Send(ReservationTime);
            DeliveryId = bulk.DeliveryId;
            return true;
        }

        private async Task<bool> SendTransaction()
        {
            var transaction = new Transaction
            {
                Subject = Subject,
                TextPart = TextPart,
                HtmlPart = HtmlPart,
                Encode = Encode,
                Attachments = Attachments,
                Cc = Cc,
                Bcc = Bcc,
                From = From,
                ListUnsubscribe = ListUnsubscribe,
                To = To[0].Email
            };
            if (To[0].CInsertCode != null)
            {
                transaction.CInsertCode = To[0].CInsertCode!;
            }
            _ = await transaction.Send();
            DeliveryId = transaction.DeliveryId;
            return true;
        }
    }
}
