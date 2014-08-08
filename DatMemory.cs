using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

namespace Memory
{
	public class DatMemory : IDisposable
	{
		
		#region Setup and miscellaneous
		
		private const string invalidHandle = "The process handle is invalid.";
		
		public bool Attached
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Used by internal functions that interact with Win32.
		/// </summary>
		private IntPtr _handle;
		
		/// <summary>
		/// Used by public functions, throws an exception if handle is invalid.
		/// </summary>
		private IntPtr Handle
		{
			get
			{
				if(_handle == IntPtr.Zero)
					throw new InvalidOperationException(invalidHandle);
				else
					return _handle;
			}
			
		}
		
		private Process targetProcess = null;
		
		private bool disposed = false;

		
		public DatMemory() { } //parameterless constructor

		public DatMemory(string processName)
		{
			this.FindProcess(processName);
		}
		
		
		#region Implementing IDisposable
		~DatMemory()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposed)
				return;
			
			Detach();  // cleanup unmanaged ressources
			disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		
		
		/// <summary>
		/// Opens a process
		/// with rights to manipulate its virtual memory.
		/// </summary>
		public bool FindProcess(string name)
		{
			if(name.EndsWith(".exe"))
				name = name.Replace(".exe", string.Empty);
			
			var processes = Process.GetProcessesByName(name);
			if(processes.Length == 0)
				return false;
			
			return AttachProcess(processes[0]);
		}

		private bool AttachProcess(Process proc)
		{
			// If already attached.
			if(_handle != IntPtr.Zero)
				return true;
			
			targetProcess = proc;
			_handle = Win32.OpenProcess(
				(uint)ProcessAccessRights.All, false,
				(uint)targetProcess.Id);
			
			Attached = _handle != IntPtr.Zero;
			
			return Attached;
		}

		/// <summary>
		/// Frees the process.
		/// </summary>
		private void Detach()
		{
			if((_handle == IntPtr.Zero) || !Attached)
				return;
			
			Win32.CloseHandle(Handle);
			_handle = IntPtr.Zero;
			Attached = false;
		}

		#endregion
		
		#region Utils
		
		/// <summary>
		/// Gets all the modules loaded in the process.
		/// </summary>
		/// <returns>A Dictionary where
		/// the key is the address of the module
		/// and the value is its name.</returns>
		public SortedDictionary<int, string> GetModuleList()
		{
			var dic = new SortedDictionary<int, string>();
			
			if(!Attached)
				return dic;
			ProcessModuleCollection pmc = targetProcess.Modules;

			foreach (ProcessModule pm in pmc)
				dic.Add((int)pm.BaseAddress, pm.ModuleName);
			
			return dic;
		}

		/// <summary>
		/// Gets the base adress of the main module.
		/// </summary>
		public uint GetBaseAdress()
		{
			if (Handle == IntPtr.Zero)
				throw new InvalidOperationException(invalidHandle);

			var module = targetProcess.MainModule;

			return (uint)module.BaseAddress;
		}

		
		#endregion

		#region Functions

		#region Read
		
		/// <summary>
		/// Read a byte (8 bits) from an address.
		/// </summary>
		/// <param name="address">Address to read the value from.</param>
		public byte ReadByte(uint address)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(byte);
			var buffer = new byte[size];
			uint read;
			Win32.ReadProcessMemory(Handle, address, buffer,
			                        size, out read);
			return buffer[0];
		}

		/// <summary>
		/// Reads a short (2 bytes) from an address.
		/// </summary>
		/// <param name="address">Address to read the value from.</param>
		public short ReadShort(uint address)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(short);
			var buffer = new byte[size];
			uint read;
			Win32.ReadProcessMemory(Handle, address, buffer,
			                        size, out read);
			
			return BitConverter.ToInt16(buffer, 0);
		}

		/// <summary>
		/// Reads an integer (4 bytes) from an address.
		/// </summary>
		/// <param name="address">Address to read the value from.</param>
		public int ReadInteger(uint address)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(int);
			var buffer = new byte[size];
			uint read;
			Win32.ReadProcessMemory(Handle, address, buffer,
			                        size, out read);
			
			return BitConverter.ToInt32(buffer, 0);
		}

		/// <summary>
		/// Reads an integer (8 bytes) from an address.
		/// </summary>
		/// <param name="address">Address to read the value from.</param>
		public long ReadLong(uint address)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size =  sizeof(long);
			var buffer = new byte[size];
			uint read;
			Win32.ReadProcessMemory(Handle, address, buffer,
			                        size, out read);
			
			return BitConverter.ToInt64(buffer, 0);
		}

		/// <summary>
		/// Returns a float(single precision floating point) number.
		/// </summary>
		/// <param name="address">Address to read the value from.</param>
		public float ReadFloat(uint address)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(float);
			var buffer = new byte[size];
			uint read;
			Win32.ReadProcessMemory(Handle, address, buffer, size, out read);
			
			return BitConverter.ToSingle(buffer, 0);
		}

		/// <summary>
		/// Returns a double precision floating point number.
		/// </summary>
		/// <param name="address">Address to read the value from.</param>
		public double ReadDouble(uint address)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(double);
			var buffer = new byte[size];
			uint read;
			Win32.ReadProcessMemory(Handle, address, buffer, size, out read);
			
			return BitConverter.ToDouble(buffer, 0);
		}

		private uint ReadPointer(uint address)
		{
			const int size = sizeof(int);
			var buffer = new byte[size];
			uint read;

			Win32.ReadProcessMemory(Handle, address, buffer, size, out read);

			return BitConverter.ToUInt32(buffer, 0);
		}

		/// <summary>
		/// Gets the value holder from a base address and an array of offsets.
		/// You can then use Write/ReadXXX,
		/// to write/read whatever you want to it.
		/// </summary>
		/// <param name="staticAddress">Static address to begin counting.
		/// </param>
		/// <param name="Offsets">Array of offset to follow/</param>
		public uint GetFinalAddress(uint staticAddress, uint[] offsets)
		{
			if (staticAddress == 0)
				throw new ArgumentException("address");
			else if (offsets == null)
				throw new ArgumentNullException("The offsets are not valid.");

			uint ptr = ReadPointer(staticAddress);
			for (int i = 0; i < offsets.Length - 1; i++)
				ptr = ReadPointer(ptr + offsets[i]);
			
			ptr = ptr + offsets[offsets.Length - 1];
			
			return ptr;
		}
		#endregion

		#region Write
		
		/// <summary>
		/// Writes an Array of Bytes to an adress.
		/// </summary>
		/// <param name="address">Address to write the AOB at.</param>
		/// <param name="newValue">AOB to write to the adress.</param>
		public bool WriteAOB(uint address, byte[] newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");
			else if (newValue == null)
				throw new ArgumentNullException("newValue");

			uint written;
			return Win32.WriteProcessMemory(Handle, address, newValue,
			                                (uint)newValue.Length, out written);
		}
		
		/// <summary>
		/// Writes a byte number to an adress.
		/// </summary>
		/// <param name="adress">Address to write the value to.</param>
		/// <param name="newValue">Value to write at the adress.</param>
		public bool WriteByte(uint address, byte newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(byte);
			var buffer = BitConverter.GetBytes(newValue);
			uint written;
			
			return Win32.WriteProcessMemory(Handle, address, buffer,
			                                size, out written);
		}

		/// <summary>
		/// Writes a short (2 bytes) number to an address.
		/// </summary>
		/// <param name="adress">Address to write the value to.</param>
		/// <param name="newValue">Value to write at the adress.</param>
		public bool WriteShort(uint address, short newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(short);
			var buffer = BitConverter.GetBytes(newValue);
			uint read;
			
			return Win32.WriteProcessMemory(Handle, address, buffer,
			                                size, out read);
		}

		/// <summary>
		/// Writes a integer (4 bytes) number to an adress.
		/// </summary>
		/// <param name="adress">Address to write the value to.</param>
		/// <param name="newValue">Value to write at the adress.</param>
		public bool WriteInteger(uint address, int newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");
			
			const int size = sizeof(int);
			byte[] buffer = BitConverter.GetBytes(newValue);
			uint read;
			
			return Win32.WriteProcessMemory(Handle, address, buffer,
			                                size, out read);
		}

		/// <summary>
		/// Writes a long (8 bytes) number to an address.
		/// </summary>
		/// <param name="adress">Address to write the value to.</param>
		/// <param name="newValue">Value to write at the adress.</param>
		public bool WriteLong(uint address, long newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(long);
			byte[] buffer = BitConverter.GetBytes(newValue);
			uint read;
			
			return Win32.WriteProcessMemory(Handle, address, buffer,
			                                size, out read);
		}

		/// <summary>
		/// Writes a single precising floating point number to an address.
		/// </summary>
		/// <param name="adress">Address to write the value to.</param>
		/// <param name="newValue">Value to write at the adress.</param>
		public bool WriteFloat(uint address, float newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(float);
			var buffer = BitConverter.GetBytes(newValue);
			uint read;
			
			return Win32.WriteProcessMemory(Handle, address, buffer,
			                                size, out read);
		}

		/// <summary>
		/// Writes a double precision floating point number to an adress.
		/// </summary>
		/// <param name="adress">Address to write the value to.</param>
		/// <param name="newValue">Value to write at the adress.</param>
		public bool WriteDouble(uint address, double newValue)
		{
			if (address == 0)
				throw new ArgumentException("address");

			const int size = sizeof(double);
			var buffer = BitConverter.GetBytes(newValue);
			uint read;
			
			return	Win32.WriteProcessMemory(Handle, address, buffer,
			                                size, out read);
		}
		
		#endregion

		#region Protection
		
		public PageRights Protect(uint address, uint length,
		                    PageRights pr = PageRights.ExecuteReadWrite)
		{
			if (address == 0)
				throw new ArgumentException("address");
			if (length == 0)
				throw new ArgumentNullException("size");

			uint old;
			Win32.VirtualProtectEx(Handle, address, length, (uint)pr, out old);
			
			return (PageRights)old;
		}

		public void RemoveProtect(uint address, uint length, 
		                          PageRights oldProtection)
		{
			if (address == 0)
				throw new ArgumentException("address");
			if (length == 0)
				throw new ArgumentNullException("size");

			uint old;
			Win32.VirtualProtectEx(Handle, address, length, (uint)oldProtection,
			                       out old);
		}
		#endregion
		
		#endregion
	}
}