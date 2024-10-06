using NUnit.Framework;
using System;
using Blastengine;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlastengineTest
{
    [TestFixture()]
    public class TransactionTest
    {

        public TransactionTest()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
            DotNetEnv.Env.Load(dir + "/.env");
            new Blastengine.Client(DotNetEnv.Env.GetString("API_USER"), DotNetEnv.Env.GetString("API_KEY"));
        }

        [Test()]
        public void TestSendText()
        {
            Task.Run(async () =>
            {
                var transaction = new Transaction();
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
                DotNetEnv.Env.Load(dir + "/.env");
                transaction.To = DotNetEnv.Env.GetString("TO");
                transaction.Subject = "テストメール from C#";
                transaction.TextPart = "これはテストメールです to __name__";
                transaction.HtmlPart = "<h1>これはテストメールです to __name__</h1>";
                transaction.Cc.Add("cc@moongift.co.jp");
                transaction.Bcc.Add("bcc@moongift.co.jp");
                transaction.InsertCode("name", "Test");
                transaction.InsertCode("code", "001");
                transaction.From = new Blastengine.From(DotNetEnv.Env.GetString("FROM"), DotNetEnv.Env.GetString("NAME"));
                transaction.ListUnsubscribe = new Blastengine.ListUnsubscribe(Url: "https://example.com", Email: "unsub__code__@moongift.co.jp");
                var bol = await transaction.Send();
                Assert.AreEqual(bol, true);
                Assert.AreNotEqual(transaction.DeliveryId, 0);
            }).GetAwaiter().GetResult();
        }
    }
}
