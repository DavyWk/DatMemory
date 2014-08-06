using System;

namespace Memory
{
	[Flags]
	/// <summary>
	/// Access rights on the virtual memory of the process.
	/// </summary>
	enum ProcessAccessRights
	{
		Operation = 0x8,
		Read = 0x10,
		Write= 0x20,
		All = (Operation | Read | Write)
	}
}