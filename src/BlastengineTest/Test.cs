using NUnit.Framework;
using System;
using Blastengine;
using System.IO;

namespace BlastengineClientTest
{
    [TestFixture()]
    public class Test
    {
        public Client _client;

        public Test()
        {
            var dir = Directory.GetParent(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
            DotNetEnv.Env.Load(dir + "/.env");
            _client = new Blastengine.Client(DotNetEnv.Env.GetString("API_USER"), DotNetEnv.Env.GetString("API_KEY"));
        }

        [Test()]
        public void TestCase()
        {
            Assert.AreNotEqual(_client.Token, "");
        }
    }
}
