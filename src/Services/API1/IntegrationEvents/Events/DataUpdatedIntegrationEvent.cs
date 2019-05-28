using BuildingBlocks.EventBusProjects.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.IntegrationEvents.Events
{
    public class DataUpdatedIntegrationEvent : IntegrationEvent
    {
        public DataUpdatedIntegrationEvent(int dataId, string data)
        {
            DataId = dataId;
            Data = data;
        }

        public int DataId { get; set; }
        public string Data { get; set; }

    }
}
