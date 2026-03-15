using static ConsoleLogger.Logger;

namespace ConsoleLogger.Extensions;

public static class ConsoleExtensions
{
    extension(Console)
    {
        public static void PlaySound(SoundOption soundOption)
        {
            Console.Beep(soundOption.Frequency, soundOption.Duration);
        }
    }
}