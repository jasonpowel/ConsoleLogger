using ConsoleLogger;

namespace Test.Console;

static class Program
{
	public static void Main()
	{
		using var logger = new Logger(consoleTitle: "Logs");
		logger.Log("Logger started...");

		Thread.Sleep(1_000);
	}
}