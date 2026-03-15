using ConsoleLogger.Extensions;

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
	public sealed record SoundOption(int Frequency, int Duration);

	protected Logger(
		string? consoleTitle = null,
		ConsoleKey keyToQuitConsole = ConsoleKey.Q) : this(
			LogLevel.Debug,
			consoleTitle,
			keyToQuitConsole)
	{
	}

	protected Logger(LogLevel defaultLogLevel, string? consoleTitle, ConsoleKey keyToQuitConsole)
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
		string finalMessage = _hasAttachedNewConsole ? PressAnyKeyToCloseWindowMessage : ConsoleLoggerDetachedMessage;
		Console.WriteLine(finalMessage);

		if (_consoleTitle is not null)
		{
			Console.Title = _previousConsoleTitle;
		}
	}

	public Logger Log(string message, LogLevel? logLevel = null)
	{
		logLevel ??= _defaultLogLevel;
		_lastLogEntry = new LogEntry(message, logLevel.Value);
		LogFormatted(message, logLevel.Value);
		return this;
	}

	public Logger LogDebug(string message)
	{
		_lastLogEntry = new LogEntry(message, LogLevel.Debug);
		Log(message, LogLevel.Debug);
		return this;
	}

	public Logger LogInformation(string message)
	{
		_lastLogEntry = new LogEntry(message, LogLevel.Info);
		Log(message, LogLevel.Info);
		return this;
	}

	public Logger LogWarning(string message)
	{
		_lastLogEntry = new LogEntry(message, LogLevel.Warning);
		Log(message, LogLevel.Warning);
		return this;
	}

	public Logger LogError(string message)
	{
		_lastLogEntry = new LogEntry(message, LogLevel.Error);
		Log(message, LogLevel.Error);
		return this;
	}

	public Logger LogCritical(string message)
	{
		_lastLogEntry = new LogEntry(message, LogLevel.Critical);
		Log(message, LogLevel.Critical);
		return this;
	}

	public async Task WithSound(Sound? sound = null)
	{
		if (_lastLogEntry is null)
		{
			throw new InvalidOperationException(
				"Cannot call this method if any of the methods to log an actual message have not been called.");
		}

		SoundOption soundOption = sound is null ? GetSoundOptionsTypeFromLastLogLevel() : CreateSoundOption(sound.Value);
		Console.PlaySound(soundOption);
	}

	private SoundOption GetSoundOptionsTypeFromLastLogLevel()
	{
		if (_lastLogEntry is null)
		{
			throw new InvalidOperationException("Cannot create sound option without having logged any message");
		}

		return _lastLogEntry.Value.LogLevel switch
		{
			LogLevel.Debug => CreateSoundOption(Sound.Notify),
			LogLevel.Info => CreateSoundOption(Sound.Notify),
			LogLevel.Warning => CreateSoundOption(Sound.Warn),
			LogLevel.Error => CreateSoundOption(Sound.Alarm),
			LogLevel.Critical => CreateSoundOption(Sound.Critical),
			_ => CreateSoundOption(Sound.Notify)
		};
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
}