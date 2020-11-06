using Newtonsoft.Json;
using Repositories.WebAPI.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Repositories.XUnitIntegrationTest
{
    public class DeveloperApiIntegrationTest
    {
        [Fact]
        public async Task TestGetAllSuccess()
        {
            using (HttpClient client = new TestClientProvider().Client)
            {
                HttpResponseMessage response = await client.GetAsync("api/Developers");
                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetAllFailure()
        {
            using (HttpClient client = new TestClientProvider().Client) {
                HttpResponseMessage response = await client.GetAsync("api/Developers");
                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestPostSuccess()
        {
            using (HttpClient client = new TestClientProvider().Client)
            {
                HttpResponseMessage response = await client.PostAsync("api/Developers", new StringContent(
                    JsonConvert.SerializeObject(new Developer() 
                    { 
                        Name = "Test Post",
                        Bio = "Lorem ipsum",
                        Email = "email@example.com",
                        Password = "1@string"
                    }),
                    Encoding.UTF8,
                    "application/json"
                ));
                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestPostFailure()
        {
            using (HttpClient client = new TestClientProvider().Client)
            {
                HttpResponseMessage response = await client.PostAsync("api/Developers", new StringContent(
                    JsonConvert.SerializeObject(new Developer()
                    {
                        Name = "Test Post",
                        Bio = "Lorem ipsum",
                        Email = "invalid-email@",
                        Password = "1@string"
                    }),
                    Encoding.UTF8,
                    "application/json"
                ));
                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }
    }
}
