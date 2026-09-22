using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Helper
{
    public class APICall
    {
        public static async Task<string> GetCallBC(string APIEndPoint, string Token)
        {
            var retval = string.Empty;

            try
            {
                var Tenant = Environment.GetEnvironmentVariable("Tenant");
                var SandboxName = Environment.GetEnvironmentVariable("SandboxName");
                APIEndPoint = "https://api.businesscentral.dynamics.com/v2.0/" + Tenant + "/" + SandboxName + "/api/" + APIEndPoint;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(APIEndPoint);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = client.GetAsync(APIEndPoint).Result;
                if (response.IsSuccessStatusCode)
                    retval = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
            }

            return retval;
        }

        public static async Task<string> PostCallShip(string APIEndPoint, string Data)
        {
            var retval = string.Empty;

            try
            {
                var APIURL = Environment.GetEnvironmentVariable("ShipEngineAPIURL");
                var APIKey = Environment.GetEnvironmentVariable("ShipEngineAPIKey");
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, APIURL + "" + APIEndPoint);
                request.Headers.Add("API-Key", APIKey);
                var jsonContent = new StringContent(Data);
                jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                request.Content = jsonContent;
                var response = await client.SendAsync(request);
                retval = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
            }

            return retval;
        }
    }
}
