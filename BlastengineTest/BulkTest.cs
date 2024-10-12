using NUnit.Framework;
using System;
using Blastengine;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Transactions;
using System.Data.Common;

namespace BlastengineTest
{
    [TestFixture()]
    public class BulkTest
    {

        public BulkTest()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
            DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
            new Blastengine.Client(DotNetEnv.Env.GetString("API_USER"), DotNetEnv.Env.GetString("API_KEY"));
        }

        [Test()]
        public void TestRegister()
        {
            Task.Run(async () =>
            {
                var bulk = new Blastengine.Bulk();
                bulk.TextPart = "Test from Bulk";
                var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
                DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
                bulk.Subject = "テストメール from C#";
                bulk.TextPart = "これはテストメールです to __name__";
                bulk.HtmlPart = "<h1>これはテストメールです to __name__</h1>";
                bulk.From = new Blastengine.From(DotNetEnv.Env.GetString("FROM"), DotNetEnv.Env.GetString("NAME"));
                var bol1 = await bulk.Begin();
                Assert.That(bol1, Is.EqualTo(true));
                Assert.That(bulk.DeliveryId > 0, Is.EqualTo(true));
                bulk.To.Add(new Blastengine.BulkTo(DotNetEnv.Env.GetString("TO"), new List<Blastengine.InsertCode>() {
                    new Blastengine.InsertCode("name", "User1")
                }));
                await bulk.Update();
                var bol2 = await bulk.Delete();
                Assert.That(bol2, Is.EqualTo(true));
            }).GetAwaiter().GetResult();
        }
    }
}
