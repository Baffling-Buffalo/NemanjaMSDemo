using BuildingBlocks.EventBusProjects.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API2.IntegrationEvents.Events
{
    public class DataUpdatedIntegrationEvent : IntegrationEvent
    {
        public DataUpdatedIntegrationEvent(int dataId, string data, string username)
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
