namespace ConsoleLogger;

public class Logger
{
	private readonly LogLevel _defaultLogLevel;

	public Logger() : this(LogLevel.Debug)
	{
		//allocate new console
	}

	public Logger(LogLevel defaultLogLevel)
	{
		_defaultLogLevel = defaultLogLevel;
	}

	public void Log(string message)
	{
	}
}