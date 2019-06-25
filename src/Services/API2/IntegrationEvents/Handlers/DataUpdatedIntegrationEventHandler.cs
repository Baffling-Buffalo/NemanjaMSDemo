using API2.IntegrationEvents.Events;
using API2.Models;
using BuildingBlocks.EventBusProjects.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API2.IntegrationEvents.Handlers
{
    public class DataUpdatedIntegrationEventHandler : IIntegrationEventHandler<DataUpdatedIntegrationEvent>
    {
        //private readonly IMarketingDataRepository _marketingDataRepository;
        private readonly ILogger<DataUpdatedIntegrationEventHandler> _logger;
        private Api2Context api2Context;
        public DataUpdatedIntegrationEventHandler(
           // IMarketingDataRepository repository,
            ILogger<DataUpdatedIntegrationEventHandler> logger,
            Api2Context api2Context)
        {
            //_marketingDataRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.api2Context = api2Context;
        }

        public async Task Handle(DataUpdatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("CorrelationId", @event.CorrelationId))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                try
                {
                    var itemToUpdate = api2Context.Api2Data.Find(@event.DataId);
                    itemToUpdate.Data = @event.Data;

                    api2Context.Api2Data.Update(itemToUpdate);

                    await api2Context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
                

            }
        }
    }
}
