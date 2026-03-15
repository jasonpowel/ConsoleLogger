namespace ConsoleLogger;

public enum Sound
{
    Prompt = 400,
    Notify = 800,
    Warn = 1200,
    Alarm = 2000,
    Critical = 2001,
}

public enum SoundDuration
{
    Prompt = 100,
    Notify = 200,
    Warn = 600,
    Alarm = 1000,
    Critical = int.MaxValue
}