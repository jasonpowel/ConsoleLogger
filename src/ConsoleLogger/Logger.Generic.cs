namespace ConsoleLogger;

public sealed class Logger<T> : Logger
{
	public Logger(string? consoleTitle = null) : base(consoleTitle ?? typeof(T).Name)
	{
	}

	public ConsoleLogBuilder LogDebug(string message)
	{
		return Log(message, LogLevel.Debug);
	}

	public ConsoleLogBuilder LogInformation(string message)
	{
		return Log(message, LogLevel.Info);
	}

	public ConsoleLogBuilder LogWarning(string message)
	{
		return Log(message, LogLevel.Warning);
	}

	public ConsoleLogBuilder LogError(string message)
	{
		return Log(message, LogLevel.Error);
	}

	public ConsoleLogBuilder LogCritical(string message)
	{
		return Log(message, LogLevel.Critical);
	}
}