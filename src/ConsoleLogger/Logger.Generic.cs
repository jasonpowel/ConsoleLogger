namespace ConsoleLogger;

public sealed class Logger<T> : Logger
{
	public Logger(string? consoleTitle = null) : base(consoleTitle ?? typeof(T).Name)
	{
	}
}