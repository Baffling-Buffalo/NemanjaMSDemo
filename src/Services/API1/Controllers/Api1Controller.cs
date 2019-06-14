using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API1.Models;
using API1.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace API1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Api1Controller : ControllerBase
    {
        private readonly Api1Context api1Context;
        private IDataService dataService { get; set; }
        //private readonly ILogger logger;

        public Api1Controller(Api1Context api1Context, IDataService dataService)
        {
            this.dataService = dataService;
            this.api1Context = api1Context;
            //this.logger = logger;
        }

        [Route("data")]
        public async Task<List<Api1Data>> Data(int? id)
        {
            List<Api1Data> response = new List<Api1Data>();

            if (id.HasValue)
            {
                var data = await api1Context.Api1Data.FindAsync(id);
                if (data != null)
                    response.Add(data);
            }
            else
            {
                response = await api1Context.Api1Data.ToListAsync();
            }

            return response;
        }

        [Route("userdata")]
        public string UserData()
        {
            return $"API1 - User data (registered users can access)";
        }


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

        [Route("CreateData")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateData(Api1Data data)
        {
            api1Context.Add(data);

            try
            {
                await api1Context.SaveChangesAsync();
                Log.Information("Created Api1Data with data: {Api1Data} ", data.Data);

                return CreatedAtAction(nameof(CreateData), new { data = data });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("UpdateData")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateData(Api1Data data)
        {
            if (await dataService.UpdateData(data))
                return Ok();
            else
                return BadRequest();
        }
    }
}
