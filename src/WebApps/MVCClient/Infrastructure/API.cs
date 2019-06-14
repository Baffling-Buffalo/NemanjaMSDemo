using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Infrastructure
{
    public static class API
    {

        public static class API1
        {
            public static string GetData(string baseUri, int? id) => id.HasValue ? $"{baseUri}/api1/data?id={id}" : $"{baseUri}/api1/data";
            public static string GetUserData(string baseUri) => $"{baseUri}/api1/userdata";
            public static string GetAdminData(string baseUri) => $"{baseUri}/api1/admindata";
            public static string GetApi1and2JoinedData(string baseUri) => $"{baseUri}/api1and2";
            public static string CreateData(string baseUri) => $"{baseUri}/api1/CreateData";

        }

        public static class API2
        {
            public static string GetBasket(string baseUri, string basketId) => $"{baseUri}/{basketId}";
        }
        
    }
}
