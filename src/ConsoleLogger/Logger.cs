namespace ConsoleLogger;

public class Logger : IDisposable
{
	private const string DefaultConsoleTitle = "Console Logger";
	private readonly LogLevel _defaultLogLevel;
	private readonly Thread _guiThread;
	private static bool _hasBeenDisposed;
	private readonly ConsoleKey _keyToQuiteConsole;

	private readonly ThreadStart _keepConsoleOpenAction;
	private readonly string _consoleTitle;

	public Logger(
		string? consoleTitle = null,
		ConsoleKey keyToQuitConsole = ConsoleKey.Q) : this(
			LogLevel.Debug,
			consoleTitle ?? DefaultConsoleTitle,
			keyToQuitConsole)
	{
	}

	public Logger(LogLevel defaultLogLevel, string consoleTitle, ConsoleKey keyToQuitConsole)
	{
		_keepConsoleOpenAction = () =>
		{
			bool hasToCloseConsole = false;

			do
			{
				ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
				hasToCloseConsole = consoleKeyInfo.Key == _keyToQuiteConsole;
			}
			while (!hasToCloseConsole && !_hasBeenDisposed);
		};


		_guiThread = new Thread(_keepConsoleOpenAction);

		if (NativeConsole.FreeConsole())
		{
			NativeConsole.AllocConsole();
			_keyToQuiteConsole = keyToQuitConsole;
			Console.Title = consoleTitle;
			_consoleTitle = consoleTitle;
			_guiThread.Start();
			_defaultLogLevel = defaultLogLevel;
		}
	}

	public void Dispose()
	{
		_hasBeenDisposed = true;
		Console.WriteLine("Press any key to close window..");
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