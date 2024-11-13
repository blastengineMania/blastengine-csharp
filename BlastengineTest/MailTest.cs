using NUnit.Framework;
using System;
using Blastengine;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Transactions;

namespace BlastengineTest
{
    [TestFixture()]
    public class MailTest
    {
        public MailTest()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
            DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
            new Blastengine.Client(DotNetEnv.Env.GetString("API_USER"), DotNetEnv.Env.GetString("API_KEY"));
        }

        [Test]
        public void TestSendText()
        {
            Task.Run(async () =>
            {

                var mail = new Blastengine.Mail();
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
                DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
                mail.To.Add(new Blastengine.BulkTo(DotNetEnv.Env.GetString("TO"), new List<Blastengine.InsertCode>() {
                    new Blastengine.InsertCode("name", "User1"),
                    new Blastengine.InsertCode("code", "001"),
                }));

                mail.Subject = "テストメール from C#";
                mail.TextPart = "これはテストメールです to __name__";
                mail.HtmlPart = "<h1>これはテストメールです to __name__</h1>";
                mail.Cc.Add("cc@moongift.co.jp");
                mail.Bcc.Add("bcc@moongift.co.jp");
                mail.From = new Blastengine.From(DotNetEnv.Env.GetString("FROM"), DotNetEnv.Env.GetString("NAME"));
                mail.ListUnsubscribe = new Blastengine.ListUnsubscribe(Url: "https://example.com", Email: "unsub__code__@moongift.co.jp");
                var bol = await mail.Send();
                Assert.That(bol, Is.EqualTo(true));
                Assert.That(mail.DeliveryId > 0, Is.EqualTo(true));
            }).GetAwaiter().GetResult();
        }

        [Test]
        public void TestSendFile()
        {
            Task.Run(async () =>
            {

                var mail = new Blastengine.Mail();
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
                DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
                mail.To.Add(new Blastengine.BulkTo(DotNetEnv.Env.GetString("TO"), new List<Blastengine.InsertCode>() {
                    new Blastengine.InsertCode("name", "User1"),
                    new Blastengine.InsertCode("code", "001"),
                }));
                mail.To.Add(new Blastengine.BulkTo(DotNetEnv.Env.GetString("TO2"), new List<Blastengine.InsertCode>() {
                    new Blastengine.InsertCode("name", "User2"),
                    new Blastengine.InsertCode("code", "002"),
                }));
                mail.Subject = "テストメール from C#";
                mail.TextPart = "これはテストメールです to __name__";
                mail.HtmlPart = "<h1>これはテストメールです to __name__</h1>";
                mail.From = new Blastengine.From(DotNetEnv.Env.GetString("FROM"), DotNetEnv.Env.GetString("NAME"));
                mail.ListUnsubscribe = new Blastengine.ListUnsubscribe(Url: "https://example.com", Email: "unsub__code__@moongift.co.jp");
                var Dir = Directory.GetParent(Path.GetFullPath(".")).Parent.Parent.FullName;
                mail.Attachments.Add($@"{Dir}/test.png");
                mail.Attachments.Add($@"{Dir}/LICENSE");
                var bol = await mail.Send();
                Assert.That(bol, Is.EqualTo(true));
                Assert.That(mail.DeliveryId > 0, Is.EqualTo(true));
            }).GetAwaiter().GetResult();
        }

        [Test]
        public void TestSendCSV()
        {
            Task.Run(async () =>
            {

                var mail = new Blastengine.Mail();
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
                DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
                for (var i = 1; i <= 60; i++)
                {
                    var email = DotNetEnv.Env.GetString("TO3").Replace("__CODE__", $@"{i}");
                    mail.To.Add(new Blastengine.BulkTo(email, new List<Blastengine.InsertCode>() {
                        new Blastengine.InsertCode("name", $@"User{i}"),
                        new Blastengine.InsertCode("code", $@"00{i}"),
                    }));
                }
                mail.Subject = "テストメール from C# __code__";
                mail.TextPart = "これはテストメールです to __name__";
                mail.HtmlPart = "<h1>これはテストメールです to __name__</h1>";
                mail.From = new Blastengine.From(DotNetEnv.Env.GetString("FROM"), DotNetEnv.Env.GetString("NAME"));
                mail.ListUnsubscribe = new Blastengine.ListUnsubscribe(Url: "https://example.com", Email: "unsub__code__@moongift.co.jp");
                var bol = await mail.Send();
                Assert.That(bol, Is.EqualTo(true));
                Assert.That(mail.DeliveryId > 0, Is.EqualTo(true));
            }).GetAwaiter().GetResult();
        }
    }
}
