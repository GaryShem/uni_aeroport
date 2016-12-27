using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Common
{
    public static class Util
    {
        public static string MakeRequest(string URL)
        {
            string str = MakeRequestAsync(URL).Result;
            return JToken.Parse(str).ToString();
        }
        private static async Task<string> MakeRequestAsync(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                using (HttpResponseMessage response = client.GetAsync(URL).Result)
                using (HttpContent content = response.Content)
                {
                    string result = await content.ReadAsStringAsync();
                    return result;
                }
            }
        }

        public static void Log(string filename, string logString)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filename))
            {
                file.WriteLine(logString);
            }
        }
    }
}
