namespace CCBR.Shared.ServiceDiscovery;

public interface IServiceDiscovery
{
    Task<string> GetServiceUrlAsync(string serviceName);
    Task RegisterServiceAsync(string serviceName, string serviceUrl, string healthCheckUrl);
    Task DeregisterServiceAsync(string serviceName, string serviceId);
}