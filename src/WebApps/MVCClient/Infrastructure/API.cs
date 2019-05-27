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
            public static string GetData(string baseUri) => $"{baseUri}/api1/data";
            public static string GetUserData(string baseUri) => $"{baseUri}/api1/userdata";
            public static string GetAdminData(string baseUri) => $"{baseUri}/api1/admindata";
            public static string GetApi1and2JoinedData(string baseUri) => $"{baseUri}/api1and2";
            public static string CreateData(string baseUri, string data) => $"{baseUri}/api1/CreateData?data={data}";

        }

        public static class API2
        {
            public static string GetBasket(string baseUri, string basketId) => $"{baseUri}/{basketId}";
        }
        
    }
}
