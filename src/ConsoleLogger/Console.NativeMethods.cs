using System.Runtime.InteropServices;

namespace ConsoleLogger;

internal partial class NativeConsole
{

	[LibraryImport("kernel32.dll")]
	public static partial bool AllocConsole();
}