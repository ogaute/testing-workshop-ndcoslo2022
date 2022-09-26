namespace ForeignExchange.Api.Logger;

public interface ILoggerAdapter<T>
{
    void LogInformation(string messageTemplate, params object?[] args);
}
