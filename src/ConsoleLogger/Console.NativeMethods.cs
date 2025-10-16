using System.Runtime.InteropServices;

namespace ConsoleLogger;

internal partial class NativeConsole
{

	[LibraryImport("Kernel32.dll")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool AllocConsole();
}