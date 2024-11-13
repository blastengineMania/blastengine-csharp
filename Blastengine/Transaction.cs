using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Json.Serialization;
using System.Text;

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

        public Transaction(): base()
        {
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
            if (Attachments.Count > 0)
            {
                return await SendFile();
            } else
            {
                return await SendText();
            }
        }

        public async Task<bool> SendFile()
        {
            var ApiPath = "/v1/deliveries/transaction";
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
            byte[] byteArray = Encoding.UTF8.GetBytes(Data);
            ByteArrayContent byteArrayContent = new ByteArrayContent(byteArray);
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var Form = new MultipartFormDataContent
            {
                { byteArrayContent, "data" }
            };
            Attachments.ForEach(filePath =>
            {
                var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                Form.Add(fileContent, "file", Path.GetFileName(filePath));
            });
            var obj = await Client!.PostFile(ApiPath, Form);
            DeliveryId = obj!.DeliveryId;
            return true;
        }

        public async Task<bool> SendText()
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

