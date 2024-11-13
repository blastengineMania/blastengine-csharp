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
    public class ReportTest
    {

        public ReportTest()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory)!);
            DotNetEnv.Env.Load(dir?.Parent?.Parent + "/.env");
            new Blastengine.Client(DotNetEnv.Env.GetString("API_USER"), DotNetEnv.Env.GetString("API_KEY"));
        }

        [Test]
        public void TestReport()
        {
            Task.Run(async () =>
            {

                var report = new Blastengine.Report(2852);
                var job = await report.Get();
                while (!(await job.Finished()))
                {
                    await Task.Delay(5000);
                }
                var result = await job.GetReport();
                foreach (var dict in result)
                {
                    foreach (var kvp in dict)
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                    Console.WriteLine();
                }
            }).GetAwaiter().GetResult();
        }

    }
}
