using Microsoft.Extensions.Logging;

namespace CCBR.Shared.CircuitBreaker;

public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}

public interface ICircuitBreaker
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation);
    CircuitBreakerState State { get; }
}

public class CircuitBreaker : ICircuitBreaker
{
    private readonly int _failureThreshold;
    private readonly TimeSpan _timeout;
    private readonly ILogger<CircuitBreaker> _logger;

    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitBreakerState _state;

    public CircuitBreaker(int failureThreshold = 5, TimeSpan? timeout = null,
                         ILogger<CircuitBreaker> logger = null)
    {
        _failureThreshold = failureThreshold;
        _timeout = timeout ?? TimeSpan.FromMinutes(1);
        _logger = logger;
        _state = CircuitBreakerState.Closed;
    }

    public CircuitBreakerState State => _state;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (_state == CircuitBreakerState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime >= _timeout)
            {
                _state = CircuitBreakerState.HalfOpen;
                _logger?.LogInformation("Circuit breaker moving to Half-Open state");
            }
            else
            {
                throw new CircuitBreakerOpenException("Circuit breaker is open");
            }
        }

        try
        {
            var result = await operation();

            if (_state == CircuitBreakerState.HalfOpen)
            {
                _state = CircuitBreakerState.Closed;
                _failureCount = 0;
                _logger?.LogInformation("Circuit breaker closed - operation successful");
            }

            return result;
        }
        catch (Exception ex)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= _failureThreshold)
            {
                _state = CircuitBreakerState.Open;
                _logger?.LogWarning($"Circuit breaker opened due to {_failureCount} failures");
            }

            throw;
        }
    }
}

public class CircuitBreakerOpenException : Exception
{
    public CircuitBreakerOpenException(string message) : base(message) { }
}
