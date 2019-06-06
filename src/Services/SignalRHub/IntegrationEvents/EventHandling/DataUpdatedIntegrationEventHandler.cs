using BuildingBlocks.EventBusProjects.EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRHub.Hubs;
using Serilog.Context;
using SignalRHub.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRHub.IntegrationEvents.Handlers
{
    public class DataUpdatedIntegrationEventHandler : IIntegrationEventHandler<DataUpdatedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<DataUpdatedIntegrationEventHandler> _logger;

        public DataUpdatedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<DataUpdatedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DataUpdatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("CorrelationId", @event.CorrelationId))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                //await _hubContext.Clients
                //    .All
                //    .SendAsync("DataUpdated", new { DataId = @event.DataId, Data = @event.Data });

                await _hubContext.Clients
                   .Group(@event.Username)
                   .SendAsync("Test", new { DataId = @event.DataId, Data = @event.Data, Username = @event.Username });
            }
        }
    }
}
