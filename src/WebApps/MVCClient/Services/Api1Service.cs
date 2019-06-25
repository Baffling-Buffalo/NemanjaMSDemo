using Microsoft.Extensions.Options;
using MVCClient.Infrastructure;
using MVCClient.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVCClient.Services
{
    public class Api1Service : IApi1Service
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<AppSettings> _settings;

        private readonly string _remoteServiceBaseUrl;


        public Api1Service(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.ApiGWUrl}";
        }

        public async Task<List<Api1Object>> GetData(int? id)
        {
            string uri = API.API1.GetData(_remoteServiceBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(uri);

            var data = JsonConvert.DeserializeObject<List<Api1Object>>(responseString);

            return data;
        }

        public async Task UpdateData(Api1Object model)
        {
            string uri = API.API1.UpdateData(_remoteServiceBaseUrl);

            var api1DataContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

            var responseString = await _httpClient.PutAsync(uri, api1DataContent);

            responseString.EnsureSuccessStatusCode();
        }

        public async Task CreateData(Api1Object model)
        {
            string uri = API.API1.CreateData(_remoteServiceBaseUrl);

            var api1DataContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

            var responseString = await _httpClient.PostAsync(uri, api1DataContent);

            responseString.EnsureSuccessStatusCode();
        }
    }
}

