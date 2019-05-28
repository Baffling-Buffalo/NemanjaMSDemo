using API1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Services
{
    public interface IDataService 
    {
        Task<bool> UpdateData(Api1Data data);
    }
}
