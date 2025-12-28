using SNS.Application.Abstractions.Loggings;
using Microsoft.Extensions.Logging;

namespace SNS.Infrastructure.Helpers.Loggings;

public class AppLogger<TEntity> : IAppLogger<TEntity>
{
    private readonly ILogger<TEntity> _logger;

    public AppLogger(
        ILogger<TEntity> logger)
    {
        _logger = logger;
    }

    public void LogError(string message, Exception exception, params object[] args) 
        => _logger.LogError(exception, message, args);

    public void LogInformation(string message, params object[] args) 
        => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);
}