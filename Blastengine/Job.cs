using System;
using System.IO;
using System.Text.Json.Serialization;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Blastengine
{
    public class Job : Base
    {
        public long Id { get; set; }

        
        public int Percentage { get; set; }
        public long SuccessCount { get; set; }
        public long FailedCount { get; set; }
        public string? ErrorFileUrl { get; set; }
        public string? MailOpenFileUrl { get; set; }
        public string JobType { get; set; }

        public Job(long id, string jobType = "Error") : base()
        {
            Id = id;
            JobType = jobType;
        }

        public string GetPath()
        {
            if (JobType == "Error")
            {
                return $@"/v1/deliveries/-/emails/import/{Id}";
            }
            if (JobType == "Report")
            {
                return $@"/v1/deliveries/-/analysis/report/{Id}";
            }
            throw new Exception($@"No JobType Error {JobType}");
        }

        public async Task<bool> Finished()
        {
            var obj = await Client!.GetText(GetPath(), null) ?? throw new Exception(@$"JobId {Id} is not found.");
            Percentage = obj.Percentage;
            SuccessCount = obj.SuccessCount;
            FailedCount = obj.FailedCount;
            TotalCount = obj.TotalCount;
            Status = obj.Status;
            ErrorFileUrl = obj.ErrorFileUrl;
            MailOpenFileUrl = obj.MailOpenFileUrl;
            return Percentage == 100;
        }

        public async Task<List<Dictionary<string, string>>> GetError()
        {
            if (ErrorFileUrl == null || ErrorFileUrl == "") {
                throw new Exception("Error is not found.");
            }
            var Stream = await Client!.GetFile(ErrorFileUrl);
            return StreamToList(Stream);
        }

        public async Task<List<Dictionary<string, string>>> GetReport()
        {
            if (MailOpenFileUrl == null || MailOpenFileUrl == "")
            {
                throw new Exception("Url is not found.");
            }
            var Stream = await Client!.GetFile(MailOpenFileUrl);
            return StreamToList(Stream);
        }

        public List<Dictionary<string, string>> StreamToList(Stream Stream)
        {
            var archive = new ZipArchive(Stream);
            var entry = archive.Entries[0];
            var entryStream = entry.Open();
            var reader = new StreamReader(entryStream, Encoding.UTF8);
            var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Encoding = System.Text.Encoding.UTF8,
            });
            var records = new List<Dictionary<string, string>>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Dictionary<string, string>();
                foreach (var header in csv.HeaderRecord!)
                {
                    record[header] = csv.GetField(header)!;
                }
                records.Add(record);
            }
            return records;
        }

        public bool IsError()
        {
            return ErrorFileUrl != "";
        }
    }
}

