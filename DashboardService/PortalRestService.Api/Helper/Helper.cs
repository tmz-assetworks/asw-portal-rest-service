using PortalRestService.Core.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace RestService.Assets.Helpers
{

    public static class Helper
    {
        private static readonly string AssetBaseAPIAddress = "http://10.0.0.4:6009/api/";

        private static readonly string AssetBaseAPIAddress = "http://localhost:7200/api/";


        public static async Task<HttpResponseMessage> GetCallAPIAsync(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AssetBaseAPIAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(SiteURL(apiUrl));
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                return response;
            }
        }

        public static async Task<HttpResponseMessage> GetCallMockAPIAsync(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AssetBaseAPIAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                return response;
            }
        }

        public static async Task<HttpResponseMessage> PostCallAPIAsync(string apiUrl, string contentBody)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(contentBody.ToString(), Encoding.UTF8, "application/json");
                return client.PostAsync(SiteURL(apiUrl), content).Result;
            }
        }


        private static string SiteURL(string urlLocation)
        {
            return AssetBaseAPIAddress + urlLocation;
        }

    }
}