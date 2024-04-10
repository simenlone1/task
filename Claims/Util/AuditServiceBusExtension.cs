using System.Configuration;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Claims.persistence.Cosmos;
using Claims.Persistence.Options;
using Claims.Persistence.ServiceBus;
using Claims.Persistence.ServiceBus.Interface;
using Claims.Presistence.Cosmos;

namespace Claims.Util;

public static class AuditServiceBusExtension
{
    public static IServiceCollection AddAuditServiceBus(this IServiceCollection services, IConfiguration servicesBusOptions)
    {
        var options = servicesBusOptions.Get<ServiceBusOptions>();
        if (options == null) throw new ConfigurationErrorsException("ServiceBus configuration error");
        var clientOptions = new ServiceBusClientOptions
        { 
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        var client = new ServiceBusClient(
            $"{options.ServiceBusName}.servicebus.windows.net",
            new DefaultAzureCredential(),
            clientOptions);
        var sender = client.CreateSender($"{options.QueName}");
        var serviceBus = new AuditRepository(sender);
        services.AddSingleton<IAuditRepository>(serviceBus);
        return services;
    }
}