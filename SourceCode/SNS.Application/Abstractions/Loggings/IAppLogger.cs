namespace SNS.Application.Abstractions.Loggings;

public interface IAppLogger<TEntity>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, Exception exception, params object[] args);
}