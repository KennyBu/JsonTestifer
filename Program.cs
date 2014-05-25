using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Formo;

namespace JsonTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var t = new TestClass();

            t.Test().Wait();

            var t1 = t.Count1;

            var t2 = t.Count2;

            var l1 = t.Success;

            var l2 = t.Fail;
        }
    }

    public class TestClass
    {
        public int Count1 { get; set; }
        public int Count2 { get; set; }

        public List<string> Success { get; set; }
        public List<string> Fail { get; set; }

        private readonly int _stressTestCount;
        private readonly string _baseUrl;
        private readonly string _url;
        private readonly string _responseValidation;
        
        

        public TestClass()
        {
            Count1 = 0;
            Success = new List<string>();
            Count2 = 0;
            Fail = new List<string>();

            dynamic config = new Configuration();
            _stressTestCount = config.StressRequestCount<int>(5);  
            _baseUrl = config.UrlBase;
            _url = config.Url;
            _responseValidation = config.ResponseValidation;
        }

        public async Task Test()
        {
            var tasks = new List<Task>();

            for (var i = 0; i < _stressTestCount; i++)
            {
                tasks.Add(Request());
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public async Task Request()
        {

            var client = new HttpClient();

            var urlbase = _baseUrl;

            var obj = new
            {
                tracking = "USPS 9405510200850006793316",
                trackingUrl = ""
            };

            client.BaseAddress = new Uri(urlbase);

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));

            var url = _url;

            var response = await client.PostAsJsonAsync(url, obj);

            var rsp = await response.Content.ReadAsStringAsync();

            client.Dispose();

            if (rsp.Contains(_responseValidation))
            {
                Count1++;
                Success.Add(rsp);
            }
            else
            {
                Count2++;
                Fail.Add(rsp);
            }
        }
    }
}
