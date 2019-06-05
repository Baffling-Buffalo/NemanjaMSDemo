using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API1.IntegrationEvents.Events;
using API1.Models;
using BuildingBlocks.EventBusProjects.EventBus.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API1.Services
{
    public class DataService : IDataService
    {
        private Api1Context dbContext { get; set; }
        private readonly IEventBus eventBus;
        private IHttpContextAccessor httpContext;

        public DataService(Api1Context dbContext, IEventBus eventBus, IHttpContextAccessor httpContext)
        {
            this.eventBus = eventBus;
            this.dbContext = dbContext;
            this.httpContext = httpContext;
        }

        public async Task<bool> UpdateData(Api1Data data)
        {
            var currentData = await dbContext.Api1Data.AsNoTracking().SingleOrDefaultAsync(d => d.Id == data.Id);

            if (currentData == null) // exists in db
                return false;

            try
            {
                dbContext.Api1Data.Update(data);

                await dbContext.SaveChangesAsync();
                PublishUpdatedDataIntegrationEvent(data);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }          
        }

        private void PublishUpdatedDataIntegrationEvent(Api1Data api1data)
        {
            var @event = new DataUpdatedIntegrationEvent(api1data.Id, api1data.Data, httpContext.HttpContext.User.Identity.Name);

            eventBus.Publish(@event);
        }
    }
}
