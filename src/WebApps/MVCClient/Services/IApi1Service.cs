using MVCClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Services
{
    public interface IApi1Service
    {
        Task<string> GetData();
        Task<string> GetUserData();
        Task<string> GetAdminData();
        Task<string> GetApi1and2JoinedData();
        Task<string> CreateData(string data);
    }
}
