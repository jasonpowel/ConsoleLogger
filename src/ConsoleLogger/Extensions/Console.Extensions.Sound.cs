
namespace ConsoleLogger.Extensions;

public static class ConsoleExtensions
{
    extension(Console)
    {
        public static void PlaySound()
        {
            Console.Beep();
        }
    }
}