﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using Nimbus.Configuration;
using Nimbus.Configuration.Settings;
using Nimbus.Extensions;
using Nimbus.Infrastructure.MessageSendersAndReceivers;
using Nimbus.InfrastructureContracts;

namespace Nimbus.Infrastructure.RequestResponse
{
    internal class MulticastRequestMessagePumpsFactory
    {
        private readonly ILogger _logger;
        private readonly RequestHandlerTypesSetting _requestHandlerTypes;
        private readonly ApplicationNameSetting _applicationName;
        private readonly IQueueManager _queueManager;
        private readonly MessagingFactory _messagingFactory;
        private readonly IMulticastRequestBroker _multicastRequestBroker;
        private readonly DefaultBatchSizeSetting _defaultBatchSize;

        public MulticastRequestMessagePumpsFactory(ILogger logger,
                                                   RequestHandlerTypesSetting requestHandlerTypes,
                                                   ApplicationNameSetting applicationName,
                                                   IQueueManager queueManager,
                                                   MessagingFactory messagingFactory,
                                                   IMulticastRequestBroker multicastRequestBroker,
                                                   DefaultBatchSizeSetting defaultBatchSize)
        {
            _logger = logger;
            _requestHandlerTypes = requestHandlerTypes;
            _applicationName = applicationName;
            _queueManager = queueManager;
            _messagingFactory = messagingFactory;
            _multicastRequestBroker = multicastRequestBroker;
            _defaultBatchSize = defaultBatchSize;
        }

        public IEnumerable<IMessagePump> CreateAll()
        {
            _logger.Debug("Creating multicast request message pumps");

            var requestTypes = _requestHandlerTypes.Value.SelectMany(ht => ht.GetGenericInterfacesClosing(typeof (IHandleRequest<,>)))
                                                   .Select(gi => gi.GetGenericArguments().First())
                                                   .OrderBy(t => t.FullName)
                                                   .Distinct()
                                                   .ToArray();

            foreach (var requestType in requestTypes)
            {
                _logger.Debug("Creating message pump for multicase request type {0}", requestType.Name);

                var topicPath = PathFactory.TopicPathFor(requestType);
                var applicationSharedSubscriptionName = String.Format("{0}", _applicationName);
                var messageReceiver = new NimbusSubscriptionMessageReceiver(_queueManager, topicPath, applicationSharedSubscriptionName);
                var dispatcher = new MulticastRequestMessageDispatcher(_messagingFactory, _multicastRequestBroker, requestType);

                var pump = new MessagePump(messageReceiver, dispatcher, _logger, _defaultBatchSize);
                yield return pump;
            }
        }
    }
}