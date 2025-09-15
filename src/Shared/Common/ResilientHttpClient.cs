using System.Text.Json;
using CCBR.Shared.CircuitBreaker;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace CCBR.Shared.Common;

public class ResilientHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ICircuitBreaker _circuitBreaker;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ILogger<ResilientHttpClient> _logger;

    public ResilientHttpClient(
        HttpClient httpClient,
        ICircuitBreaker circuitBreaker,
        ILogger<ResilientHttpClient> logger)
    {
        _httpClient = httpClient;
        _circuitBreaker = circuitBreaker;
        _logger = logger;
        _retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) => _logger.LogWarning($"Retry {retryCount} in {timespan} seconds"));
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
            await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Making GET request to {requestUri}");
                return await _httpClient.GetAsync(requestUri);
            }));
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
            await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Making POST request to {requestUri}");
                return await _httpClient.PostAsync(requestUri, content);
            }));
    }

    public async Task<T> GetAsync<T>(string requestUri)
    {
        var response = await GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}