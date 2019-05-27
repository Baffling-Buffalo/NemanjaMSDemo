using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Api1Controller : ControllerBase
    {
        [Route("data")]
        public string Data()
        {
            User.Claims.ToArray();
            return $"API1 - Anonymous data (everyone can access)";
        }

        [Authorize]
        [Route("userdata")]
        public string UserData()
        {
            return $"API1 - User data (registered users can access)";
        }


        [Authorize(Roles = "admin")]
        [Route("admindata")]
        public string AdminData()
        {
            return $"API1 - Admin data (users with role of admin can access)";
        }


        [Route("delayeddata")]
        public string data()
        {
            System.Threading.Thread.Sleep(1000);
            return $"some data";
        }

        //// GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
