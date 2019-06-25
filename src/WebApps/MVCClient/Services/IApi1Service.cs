using MVCClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Services
{
    public interface IApi1Service
    {
        Task<List<Api1Object>> GetData(int? id);
        Task UpdateData(Api1Object data);
        Task CreateData(Api1Object data);      
    }
}
