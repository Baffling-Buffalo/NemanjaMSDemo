using Microsoft.Extensions.Options;
using MVCClient.Infrastructure;
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

        public async Task<string> GetData()
        {
            string uri = API.API1.GetData(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            return responseString;
        }

        public async Task<string> GetUserData()
        {
            string uri = API.API1.GetUserData(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            return responseString;
        }

        public async Task<string> GetAdminData()
        {
            string uri = API.API1.GetAdminData(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            return responseString;
        }

        public async Task<string> GetApi1and2JoinedData()
        {
            string uri = API.API1.GetApi1and2JoinedData(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            return responseString;
        }

        public async Task<string> CreateData(string data)
        {
            string uri = API.API1.CreateData(_remoteServiceBaseUrl,data);

            var responseString = await _httpClient.PostAsync(uri, null);

            responseString.EnsureSuccessStatusCode();

            return "Successfuly created";
        }
    }
}

