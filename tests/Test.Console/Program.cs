using ConsoleLogger;

namespace Test.Console;

static class Program
{
	public static void Main()
	{
		using var logger = new Logger(consoleTitle: "Logger");

		logger.WithSound(Sound.Notify);
	}
}