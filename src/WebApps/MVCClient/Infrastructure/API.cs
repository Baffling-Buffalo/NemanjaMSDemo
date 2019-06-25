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
            public static string GetData(string baseUri, int? id) => id.HasValue ? $"{baseUri}/api1/GetData?id={id}" : $"{baseUri}/api1/GetData";
            public static string UpdateData(string baseUri) => $"{baseUri}/api1/UpdateData";
            public static string CreateData(string baseUri) => $"{baseUri}/api1/CreateData";

        }

        public static class API2
        {
            public static string GetBasket(string baseUri, string basketId) => $"{baseUri}/{basketId}";
        }
        
    }
}
