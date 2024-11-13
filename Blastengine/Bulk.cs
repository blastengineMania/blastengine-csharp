using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Transactions;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text;
using System.Net.Http;
using System.Collections;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Reflection.Metadata;

namespace Blastengine
{

    public class Bulk : Base
    {
        [JsonPropertyName("to")]
        public List<BulkTo> To { get; set; }

        public Bulk() : base()
        {
            To = new List<BulkTo> { };
        }

        public async Task<bool> Begin()
        {
            if (Attachments.Count > 0)
            {
                return await BeginFile();
            } else
            {
                return await BeginText();
            }
        }

        public async Task<bool> BeginFile()
        {
            var ApiPath = "/v1/deliveries/bulk/begin";
            var settings = new JsonSerializerOptions
            {
                Converters = { new DynamicPropertyConverter<Bulk>(new[] {
                    "From", "Encode", "Subject",
                    "ListUnsubscribe", "TextPart", "HtmlPart"
                }) },
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

        public async Task<bool> BeginText()
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
            // Console.WriteLine(obj!.DeliveryId);
            DeliveryId = obj!.DeliveryId;
            return true;
        }

        public async Task<bool> Update()
        {
            var properties = new[] {
                "From", "Subject", "ListUnsubscribe",
                "TextPart", "HtmlPart"
            };
            if (To.Count > 50)
            {
                await UpdateByCSV();
            }
            else
            {
                Array.Resize(ref properties, properties.Length + 1);
                properties[properties.Length - 1] = "To";
            }
            var Path = $@"/v1/deliveries/bulk/update/{DeliveryId}";
            var settings = new JsonSerializerOptions
            {
                Converters = { new DynamicPropertyConverter<Bulk>(properties) },
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

        public async Task<Job> Import(string FilePath, bool IgnoreErrors = false, bool Immediate = false)
        {
            var fileContent = new ByteArrayContent(File.ReadAllBytes(FilePath));
            return await Import(fileContent, IgnoreErrors, Immediate);
        }

        public async Task<Job> Import(ByteArrayContent Content, bool IgnoreErrors = false, bool Immediate = false)
        {
            if (DeliveryId == 0)
            {
                throw new Exception("DeliveryId is required.");
            }
            var Path = $@"/v1/deliveries/{DeliveryId}/emails/import";
            var options = new BulkImportOptions { IgnoreErrors = IgnoreErrors, Immediate = Immediate };
            var Data = JsonSerializer.Serialize(options);
            byte[] byteArray = Encoding.UTF8.GetBytes(Data);
            ByteArrayContent byteArrayContent = new ByteArrayContent(byteArray);
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var Form = new MultipartFormDataContent
            {
                { byteArrayContent, "data" }
            };
            Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            Form.Add(Content, "file", "address.csv");
            var obj = await Client!.PostFile(Path, Form) ?? throw new Exception("Create CSV Import failed.");
            var job = new Job(obj.JobId, "Error");
            return job;
        }

        public async Task<bool> UpdateByCSV()
        {
            var csvString = CreateCSV();
            byte[] byteArray = Encoding.UTF8.GetBytes(csvString);
            ByteArrayContent byteArrayContent = new ByteArrayContent(byteArray);
            var job = await Import(byteArrayContent);
            while (!(await job.Finished()))
            {
                await Task.Delay(5000);
            }
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

        public string CreateCSV()
        {
            // すべてのユニークなキーを抽出し、ヘッダーを生成
            var allKeys = To
                .SelectMany(b => b.CInsertCode ?? new List<InsertCode>())
                .Select(ic => ic.Key)
                .Distinct()
                .OrderBy(k => k)
                .ToList();
            // ヘッダー行を作成
            var headers = new List<string> { "email" };
            headers.AddRange(allKeys);

            // データ行を作成
            var records = To.Select(b =>
            {
                var record = new Dictionary<string, string>
                {
                    { "email", b.Email }
                };

                foreach (var key in allKeys)
                {
                    var insertCode = b.CInsertCode?.FirstOrDefault(ic => ic.Key == key);
                    record[key] = insertCode?.Value ?? string.Empty;
                }

                return record;
            }).ToList();

            // CSV に変換
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });
            // ヘッダーを書き込み
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }
            csv.NextRecord();

            // 各レコードを書き込み
            foreach (var record in records)
            {
                foreach (var header in headers)
                {
                    csv.WriteField(record[header]);
                }
                csv.NextRecord();
            }

            return writer.ToString();
        }

    }
}

