using System;
using System.Runtime.InteropServices;

namespace Memory
{
	static class Win32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess
			( uint desiredAcess, bool inheritHandle, uint processID);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr objectHandle);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory
			(IntPtr process, uint baseAddress, byte[] buffer, uint size,
			 ref uint numberOfBytesRead);
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory
			(IntPtr process, uint baseAddress, byte[] buffer, uint size,
			 ref uint numberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualProtectEx
			(IntPtr proces, uint address, uint size, uint newProtect,
			 ref uint oldProtect);
	}
}