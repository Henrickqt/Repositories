using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Repositories.WebAPI;
using System;
using System.Net.Http;

namespace Repositories.XUnitIntegrationTest
{
    public class TestClientProvider : IDisposable
    {
        private TestServer Server;
        public HttpClient Client { get; private set; }

        public TestClientProvider()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        public void Dispose()
        {
            Server?.Dispose();
            Client?.Dispose();
        }
    }
}
