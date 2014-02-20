﻿using System.Threading.Tasks;
using Nimbus.InfrastructureContracts;
using Nimbus.UnitTests.MessageBrokerTests.CompetingEventBrokerTests.MessageContracts;

namespace Nimbus.UnitTests.MessageBrokerTests.CompetingEventBrokerTests.Handlers
{
    public class SecondFooEventHandler : IHandleCompetingEvent<FooEvent>
    {
        public async Task Handle(FooEvent busEvent)
        {
            MethodCallCounter.RecordCall<SecondFooEventHandler>(h => h.Handle(busEvent));
        }
    }
}