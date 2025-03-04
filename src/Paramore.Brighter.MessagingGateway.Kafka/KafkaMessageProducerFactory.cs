﻿#region Licence
/* The MIT License (MIT)
Copyright © 2015 Wayne Hunsley <whunsley@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */
#endregion

using System;
using Confluent.Kafka;

namespace Paramore.Brighter.MessagingGateway.Kafka
{
    /// <summary>
    /// Creates a <see cref="KafkaMessageProducer"/> from a <see cref="Publication"/>
    /// Note that we only return the interface and <see cref="KafkaMessageProducer"/> is internal as the underlying type is not needed
    /// </summary>
    public class KafkaMessageProducerFactory : IAmAMessageProducerFactory
    {
        private readonly KafkaMessagingGatewayConfiguration _globalConfiguration;
        private readonly KafkaPublication _publication;
        private Action<ProducerConfig> _configHook;

        /// <summary>
        /// This constructs a <see cref="KafkaMessageProducerFactory"/> which can be used to create a <see cref="KafkaMessageProducer"/>.
        /// It takes a dependency on a <see cref="KafkaMessagingGatewayConfiguration"/> to connect to the broker, but defaults its
        /// <see cref="KafkaPublication"/> with the MakeChannels equal to OnMissingChannel.Create
        /// </summary>
        /// <param name="globalConfiguration">Configures how we connect to the broker</param>
        public KafkaMessageProducerFactory(
            KafkaMessagingGatewayConfiguration globalConfiguration
            ) : this(globalConfiguration, new KafkaPublication{MakeChannels = OnMissingChannel.Create})
        {
        }

        /// <summary>
        /// This constructs a <see cref="KafkaMessageProducerFactory"/> which can be used to create a <see cref="KafkaMessageProducer"/>.
        /// It takes a dependency on a <see cref="KafkaMessagingGatewayConfiguration"/> to connect to the broker, and a <see cref="KafkaPublication"/>
        /// that determines how we publish to Kafka and the parameters of any topic if required.
        /// </summary>
        /// <param name="globalConfiguration">Configures how we connect to the broker</param>
        /// <param name="publication">How do we publish, both producer parameters and topic configuration</param>
        public KafkaMessageProducerFactory(
            KafkaMessagingGatewayConfiguration globalConfiguration, 
            KafkaPublication publication)
        {
            _globalConfiguration = globalConfiguration;
            _publication = publication;
            _configHook = null;
        }
        
        /// <summary>
        /// Create a message producer from tne <see cref="KafkaMessagingGatewayConfiguration"/> and <see cref="KafkaPublication"/> supplied
        /// to the constructor
        /// </summary>
        /// <returns>An <see cref="IAmAMessageProducerSync"/> that represents a Kafka Message Producer</returns>
        public IAmAMessageProducerSync Create()
        {
            var producer = new KafkaMessageProducer(_globalConfiguration, _publication);
            if (_configHook != null)
                producer.ConfigHook(_configHook);
            producer.Init();
            return producer;
        }

        /// <summary>
        /// Set a configuration hook to set properties not exposed by KafkaMessagingGatewayConfiguration or KafkaPublication
        /// Intended as 'get out of gaol free' this couples us to the Confluent .NET Kafka client. Bear in mind that a future release
        /// might drop the Confluent client, and this hook
        /// </summary>
        /// <param name="hook"></param>
        public void SetConfigHook(Action<ProducerConfig> hook)
        {
            _configHook = hook;
        }
    }
}
