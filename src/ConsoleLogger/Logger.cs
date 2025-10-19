namespace ConsoleLogger;

public class Logger : IDisposable
{
	private readonly LogLevel _defaultLogLevel;
	private readonly Thread _guiThread;
	private static readonly ThreadStart _keepConsoleOpenAction = () =>
		{
			bool userHasPressedQ = false;

			do
			{
				ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

				userHasPressedQ = consoleKeyInfo.Key == ConsoleKey.Q;
			}
			while (!userHasPressedQ);
		};


	public Logger() : this(LogLevel.Debug)
	{
	}

	public Logger(LogLevel defaultLogLevel)
	{
		if (NativeConsole.FreeConsole())
		{
			NativeConsole.AllocConsole();
			Console.Title = "Console Logger";
			_guiThread = new Thread(_keepConsoleOpenAction);
			_guiThread.Start();
			_defaultLogLevel = defaultLogLevel;
		}
	}

	public void Dispose()
	{
		_guiThread.Abort();
	}

	public void Log(string message)
	{
		LogFormatted(message);
	}

	private void LogFormatted(string message)
	{
		switch (_defaultLogLevel)
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