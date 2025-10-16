using ConsoleLogger;

namespace Test.Console;

static class Program
{
	public static void Main()
	{
		var logger = new Logger();
		logger.Log("Hello");
	}
}