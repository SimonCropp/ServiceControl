namespace ServiceControl.Recoverability
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.Raw;
    using NServiceBus.Transport;
    using ServiceBus.Management.Infrastructure.Settings;
    using Transports;

    class RawEndpointFactory
    {
        public RawEndpointFactory(Settings settings, TransportSettings transportSettings, TransportCustomization transportCustomization)
        {
            this.transportSettings = transportSettings;
            this.settings = settings;
            this.transportCustomization = transportCustomization;
        }

        public RawEndpointConfiguration CreateReturnToSenderDequeuer(string name, Func<MessageContext, IDispatchMessages, Task> onMessage)
        {
            var config = RawEndpointConfiguration.Create(name, onMessage, transportSettings.ErrorQueue);
            config.LimitMessageProcessingConcurrencyTo(settings.MaximumConcurrencyLevel);

            transportCustomization.CustomizeForReturnToSenderIngestion(config, transportSettings);
            return config;
        }

        Settings settings;
        TransportCustomization transportCustomization;
        TransportSettings transportSettings;
    }
}