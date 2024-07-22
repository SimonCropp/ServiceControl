﻿namespace Particular.LicensingComponent.Contracts;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserIndicator
{
    NServiceBusEndpoint,
    NotNServiceBusEndpoint,
    SendOnlyOrTransactionSessionEndpoint,
    NServiceBusEndpointNoLongerInUse,
    PlannedToDecommission
}