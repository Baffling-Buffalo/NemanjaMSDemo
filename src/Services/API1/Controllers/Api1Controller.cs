using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API1.Extensions;
using API1.IntegrationEvents.Events;
using API1.Models;
using API1.Services;
using BuildingBlocks.EventBusProjects.EventBus.Abstractions;
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
        private readonly IEventBus eventBus;
        private readonly Api1Context api1Context;
        //private readonly ILogger logger;

        public Api1Controller(IEventBus eventBus, Api1Context api1Context)
        {
            this.eventBus = eventBus;
            this.api1Context = api1Context;
            //this.logger = logger;
        }

        [Route("getdata")]
        public async Task<List<Api1Data>> GetData(int? id)
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
            var currentData = await api1Context.Api1Data.AsNoTracking().SingleOrDefaultAsync(d => d.Id == data.Id);

            if (currentData == null) // dosnt exists in db
                return BadRequest();

            try
            {
                api1Context.Api1Data.Update(data);

                await api1Context.SaveChangesAsync();

                var @event = new DataUpdatedIntegrationEvent(HttpContext.GetCorrelationId(), data.Id, data.Data, HttpContext.User.Identity.Name);

                eventBus.Publish(@event);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }       
    }
}
