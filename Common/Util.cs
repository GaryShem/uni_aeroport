using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Common
{
    static class Util
    {
        public static string MakeRequest(string URL)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(URL).Result)
            using (HttpContent content = response.Content)
            {
                return content.ToString();
            }
        }
    }
}
