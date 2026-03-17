namespace ConsoleLogger;

public class Logger : IDisposable
{
	private const string DefaultConsoleTitle = "Console Logger";
	private const string ConsoleLoggerDetachedMessage = "\nDetaching Console Logger....\n";
	private const string PressAnyKeyToCloseWindowMessage = "Press any key to close window..";
	private readonly LogLevel _defaultLogLevel;
	private readonly Thread _guiThread;
	private static bool _hasBeenDisposed;
	private readonly ConsoleKey _keyToQuiteConsole;
	private LogEntry? _lastLogEntry;

	private readonly ThreadStart _keepConsoleOpenAction;
	private readonly string _previousConsoleTitle;
	private readonly string _consoleTitle;
	private readonly bool _hasAttachedNewConsole;
	private const int MaxConsoleTitleLength = 24500;

	internal record struct LogEntry(string Message, LogLevel LogLevel);
	internal sealed record SoundOption(int Frequency, int Duration);

	protected internal Logger(
	  string? consoleTitle = null,
	  ConsoleKey keyToQuitConsole = ConsoleKey.Q) : this(
		  LogLevel.Debug,
		  consoleTitle,
		  keyToQuitConsole)
	{
	}

	protected internal Logger(LogLevel defaultLogLevel, string? consoleTitle, ConsoleKey keyToQuitConsole)
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

		_hasAttachedNewConsole = NativeConsole.AllocConsole();

		if (_hasAttachedNewConsole)
		{
			if (consoleTitle is null)
			{
				consoleTitle = DefaultConsoleTitle;
			}
			else
			{
				ArgumentOutOfRangeException.ThrowIfGreaterThan(
					consoleTitle.Length,
					MaxConsoleTitleLength);
			}


			_keyToQuiteConsole = keyToQuitConsole;
			_previousConsoleTitle = Console.Title;
			_consoleTitle = consoleTitle;
			Console.Title = consoleTitle;
			_guiThread.Start();
			_defaultLogLevel = defaultLogLevel;
		}
		else
		{
			if (consoleTitle is not null)
			{
				ArgumentOutOfRangeException.ThrowIfGreaterThan(
					consoleTitle.Length,
					MaxConsoleTitleLength);

				_previousConsoleTitle = Console.Title;
				_consoleTitle = consoleTitle;
				Console.Title = consoleTitle;
				_defaultLogLevel = defaultLogLevel;
			}

		}
	}

	public void Dispose()
	{
		_hasBeenDisposed = true;
		string finalMessage;

		if (_hasAttachedNewConsole)
		{
			finalMessage = PressAnyKeyToCloseWindowMessage;
		}
		else
		{
			finalMessage = ConsoleLoggerDetachedMessage;
		}

		Console.WriteLine(finalMessage);

		if (_consoleTitle is not null)
		{
			Console.Title = _previousConsoleTitle;
		}
	}

	protected ConsoleLogBuilder Log(string message, LogLevel? logLevel = null)
	{
		logLevel ??= _defaultLogLevel;
		LogEntry logEntry = new LogEntry(message, logLevel.Value);
		CacheLogEntry(logEntry);
		LogFormatted(message, logLevel.Value);
		return new ConsoleLogBuilder(logEntry);
	}

	private void CacheLogEntry(LogEntry logEntry)
	{
		_lastLogEntry = logEntry;
	}

	private static SoundOption CreateSoundOption(Sound soundOption)
	{
		int soundOptionAsInteger = (int)soundOption;

		return soundOption switch
		{
			Sound.Prompt => new SoundOption(soundOptionAsInteger, (int)SoundDuration.Prompt),
			Sound.Notify => new SoundOption(soundOptionAsInteger, (int)SoundDuration.Notify),
			Sound.Warn => new SoundOption(soundOptionAsInteger, (int)SoundDuration.Warn),
			Sound.Alarm => new SoundOption(soundOptionAsInteger, (int)SoundDuration.Alarm),
			Sound.Critical => new SoundOption(soundOptionAsInteger, (int)SoundDuration.Critical),
			_ => new SoundOption(1, 1)
		};
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

	public sealed class ConsoleLogBuilder
	{
		private readonly LogEntry _logEntry;

		internal ConsoleLogBuilder(LogEntry logEntry)
		{
			_logEntry = logEntry;
		}

		public void WithSound(Sound? sound = null)
		{
			SoundOption soundOption;

			if (sound is null)
			{
				soundOption = GetSoundOptionsTypeFromLastLogLevel();
			}
			else
			{
				soundOption = CreateSoundOption(sound.Value);
			}

			PlaySound(soundOption);
		}

		private static void PlaySound(SoundOption soundOption)
		{
			Console.Beep(soundOption.Frequency, soundOption.Duration);
		}

		private SoundOption GetSoundOptionsTypeFromLastLogLevel()
		{
			return _logEntry.LogLevel switch
			{
				LogLevel.Debug => CreateSoundOption(Sound.Prompt),
				LogLevel.Info => CreateSoundOption(Sound.Notify),
				LogLevel.Warning => CreateSoundOption(Sound.Warn),
				LogLevel.Error => CreateSoundOption(Sound.Alarm),
				LogLevel.Critical => CreateSoundOption(Sound.Critical),
				_ => CreateSoundOption(Sound.Notify)
			};
		}
	}
}