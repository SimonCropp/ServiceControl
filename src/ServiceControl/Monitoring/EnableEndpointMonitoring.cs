namespace ServiceControl.Audit.Monitoring
{
    using System;
    using NServiceBus;

    public class EnableEndpointMonitoring : ICommand
    {
        public Guid EndpointId { get; set; }
    }
}