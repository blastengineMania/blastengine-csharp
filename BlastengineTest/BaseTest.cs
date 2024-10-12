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
    public class BaseTest
    {

        public BaseTest()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
            DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
            new Blastengine.Client(DotNetEnv.Env.GetString("API_USER"), DotNetEnv.Env.GetString("API_KEY"));
        }

        [Test()]
        public void TestGet()
        {
            Task.Run(async () =>
            {
                var transaction = new Blastengine.Transaction();
                transaction.DeliveryId = 2822;
                await transaction.Get();
                Assert.That(transaction.DeliveryId > 0, Is.EqualTo(true));
                Assert.That(transaction.TotalCount > 0, Is.EqualTo(true));
            }).GetAwaiter().GetResult();
        }
    }
}
