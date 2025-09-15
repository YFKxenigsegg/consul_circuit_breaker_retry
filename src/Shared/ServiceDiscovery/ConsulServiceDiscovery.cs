using Consul;
using Microsoft.Extensions.Logging;

namespace CCBR.Shared.ServiceDiscovery;

public class ConsulServiceDiscovery(
    IConsulClient consulClient,
    ILogger<ConsulServiceDiscovery> logger)
    : IServiceDiscovery
{
    private readonly IConsulClient _consulClient = consulClient;
    private readonly ILogger<ConsulServiceDiscovery> _logger = logger;

    public async Task<string> GetServiceUrlAsync(string serviceName)
    {
        var services = await _consulClient.Health.Service(serviceName, string.Empty, true);

        if (services.Response?.Length == 0)
        {
            throw new ServiceNotFoundException($"Service {serviceName} not found");
        }

        var service = services.Response[Random.Shared.Next(services.Response.Length)];
        return $"http://{service.Service.Address}:{service.Service.Port}";
    }

    public async Task RegisterServiceAsync(string serviceName, string serviceUrl, string healthCheckUrl)
    {
        var uri = new Uri(serviceUrl);
        var registration = new AgentServiceRegistration
        {
            ID = $"{serviceName}-{Environment.MachineName}-{uri.Port}",
            Name = serviceName,
            Address = uri.Host,
            Port = uri.Port,
            Check = new AgentServiceCheck
            {
                HTTP = healthCheckUrl,
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5)
            }
        };

        await _consulClient.Agent.ServiceRegister(registration);
        _logger.LogInformation($"Service {serviceName} registered successfully");
    }

    public async Task DeregisterServiceAsync(string serviceName, string serviceId)
    {
        await _consulClient.Agent.ServiceDeregister(serviceId);
        _logger.LogInformation($"Service {serviceName} deregistered successfully");
    }
}

public class ServiceNotFoundException(string message) : Exception(message) { }
