using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Api2Controller : ControllerBase
    {
        [Route("data")]
        public string Data()
        {
            return $"API2 - Anonymous data (everyone can access)";
        }

        [Route("userdata")]
        public string UserData()
        {
            return $"API2 - User data (registered users can access)";
        }


        [Route("admindata")]
        public string AdminData()
        {
            return $"API2 - Admin data (users with role of admin can access)";
        }
    }
}
