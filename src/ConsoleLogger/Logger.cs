namespace ConsoleLogger;

public class Logger
{
	private readonly LogLevel _defaultLogLevel;
	private readonly Thread _guiThread;

	public Logger() : this(LogLevel.Debug)
	{
		if (NativeConsole.FreeConsole())
		{
			if (NativeConsole.AllocConsole())
			{
				_guiThread = new Thread(() =>
				{
					while (true)
					{
						Console.ReadKey();
					}
				});

				_guiThread.Start();
			}
		}
	}

	public Logger(LogLevel defaultLogLevel)
	{
		_defaultLogLevel = defaultLogLevel;
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