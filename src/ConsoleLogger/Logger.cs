namespace ConsoleLogger;

public class Logger : IDisposable
{
	private const string DefaultConsoleTitle = "Console Logger";
	private readonly LogLevel _defaultLogLevel;
	private readonly Thread _guiThread;
	private static bool _hasBeenDisposed;
	private static readonly ThreadStart _keepConsoleOpenAction = () =>
		{
			bool hasToCloseConsole = false;

			do
			{
				ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
				hasToCloseConsole = consoleKeyInfo.Key == ConsoleKey.Q;
			}
			while (!hasToCloseConsole && !_hasBeenDisposed);
		};


	public Logger(string? consoleTitle = null) : this(LogLevel.Debug, consoleTitle ?? DefaultConsoleTitle)
	{
	}

	public Logger(LogLevel defaultLogLevel, string consoleTitle)
	{
		_guiThread = new Thread(_keepConsoleOpenAction);

		if (NativeConsole.FreeConsole())
		{
			NativeConsole.AllocConsole();
			Console.Title = consoleTitle;
			_guiThread.Start();
			_defaultLogLevel = defaultLogLevel;
		}
	}

	public void Dispose()
	{
		_hasBeenDisposed = true;
	}

	public void Log(string message, LogLevel? logLevel = null)
	{
		LogFormatted(message, logLevel ?? _defaultLogLevel);
	}

	public void LogDebug(string message)
	{
		Log(message, LogLevel.Debug);
	}

	public void LogInformation(string message)
	{
		Log(message, LogLevel.Info);
	}

	public void LogWarning(string message)
	{
		Log(message, LogLevel.Warning);
	}

	public void LogError(string message)
	{
		Log(message, LogLevel.Error);
	}

	public void LogCritical(string message)
	{
		Log(message, LogLevel.Critical);
	}

	private static void LogFormatted(string message, LogLevel logLevel)
	{
		switch (logLevel)
		{
			case LogLevel.Debug:
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(message);
				Console.ResetColor();
				break;
			case LogLevel.Info:
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(message);
				Console.ResetColor();
				break;
			case LogLevel.Warning:
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(message);
				Console.ResetColor();
				break;
			case LogLevel.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(message);
				Console.ResetColor();
				break;
			case LogLevel.Critical:
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(message);
				Console.ResetColor();
				break;
		}
	}
}