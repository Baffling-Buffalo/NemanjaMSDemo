using BuildingBlocks.EventBusProjects.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.IntegrationEvents.Events
{
    public class Api1DataCreatedIntegrationEvent : IntegrationEvent
    {
        public Api1DataCreatedIntegrationEvent(string correlationId, int dataId, string data, string username) : base(correlationId)
        {
            DataId = dataId;
            Data = data;
            Username = username;
        }

        public int DataId { get; set; }
        public string Data { get; set; }
        public string Username { get; set; }

    }
}
