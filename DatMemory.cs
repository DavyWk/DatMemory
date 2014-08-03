using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

// Author : Davy.W(davydavekk), please give credits if you use it.
// Last update : 4//04/14
namespace Memory
{
    public class DatMemory : IDisposable
    {

        #region Setup and miscellaneous

        private Process _targetProcess = null;
        private IntPtr _targetProcessHandle = IntPtr.Zero;
        private bool disposed = false;

        public DatMemory() { } //parameterless constructor

        public DatMemory(string processName)
        {
            this.FindProcess(processName);
        }

        ~DatMemory()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) // cleanup managed ressources
                {
                 
                }
                Detach();  // cleanup unmanaged ressources
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Returns an array with the name of all modules loaded in the process.
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<int, string> GetModuleList()
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }

            SortedDictionary<int, string> dic = new SortedDictionary<int, string>();
            ProcessModuleCollection pmc = _targetProcess.Modules;

            foreach (ProcessModule pm in pmc)
            {
                dic.Add((int)pm.BaseAddress, pm.ModuleName);
            }

            return dic;
        }

        /// <summary>
        /// Gets the base adress of the main module. 
        /// </summary>
        /// <returns></returns>
        public uint GetBaseAdress()
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }

            ProcessModule mMain = _targetProcess.MainModule;

            return (uint)mMain.BaseAddress;
        }

        /// <summary>
        /// Opens a process, to allow operations on the process's virtual memory.
        /// </summary>
        /// <param name="ProcessName">Name of the process you want to open</param>
        /// <returns></returns>
        public bool FindProcess(string ProcessName)
        {
            Process[] processes = Process.GetProcessesByName(ProcessName);

            if (processes.Length > 0)
            {
                foreach (Process p in processes)
                {
                    if (AttachProcess(p))
                        return true;

                    else return false;
                }
            }

            return false;
        }

        private bool AttachProcess(Process proc)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                _targetProcess = proc;
                _targetProcessHandle = Win32.OpenProcess((uint)Win32.PROCESS_ACCESS_RIGHTS.VM_ALL, false, (uint)_targetProcess.Id);
                if (_targetProcessHandle == IntPtr.Zero)
                {
                    return false;
                }
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Free the process.
        /// </summary>
        private void Detach()
        {
            if (_targetProcessHandle != IntPtr.Zero)
            {
                _targetProcess = null;
                Win32.CloseHandle(_targetProcessHandle);
                _targetProcessHandle = IntPtr.Zero;

            }
        }
       

        #endregion

        #region Functions

        /// <summary>
        /// Returns a byte value.
        /// </summary>
        /// <param name="address">Address to read the value from</param>
        /// <returns></returns>
        public byte ReadByte(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            uint bytesRead = 0;
            byte[] _Value = new byte[1];
            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, sizeof(byte), ref bytesRead);
            byte val = _Value[0];
            return val;

        }

        /// <summary>
        /// Returns a short (2 bytes) value.
        /// </summary>
        /// <param name="address">Address to read the value from</param>
        /// <returns></returns>
        public short ReadShort(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            uint bytesRead = 0;
            byte[] _Value = new byte[sizeof(short)];
            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, sizeof(short), ref bytesRead);
            return BitConverter.ToInt16(_Value, 0);
        }

        /// <summary>
        /// Returns a integer (4 bytes) value.
        /// </summary>
        /// <param name="address">Address to read the value from.</param>
        /// <returns></returns>
        public int ReadInteger(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            uint bytesRead = 0;
            byte[] _Value = new byte[sizeof(int)];
            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, sizeof(int), ref bytesRead);
            return BitConverter.ToInt32(_Value, 0);
        }

        /// <summary>
        /// Returns a long (8 bytes) value.
        /// </summary>
        /// <param name="address">Address to read the value from.</param>
        /// <returns></returns>
        public long ReadLong(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            uint bytesRead = 0;
            byte[] _Value = new byte[sizeof(long)];
            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, sizeof(long), ref bytesRead);
            return BitConverter.ToInt64(_Value, 0);
        }

        /// <summary>
        /// Returns a float(single precision floating point) number.
        /// </summary>
        /// <param name="address">Address to read the value from.</param>
        /// <returns></returns>
        public float ReadFloat(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            uint bytesRead = 0;
            Byte[] _Value = new byte[sizeof(float)];
            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, sizeof(float), ref bytesRead);
            return BitConverter.ToSingle(_Value, 0);
        }

        /// <summary>
        /// Returns a double precision floating point number.
        /// </summary>
        /// <param name="address">Address to read the value from.</param>
        /// <returns></returns>
        public double ReadDouble(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            uint bytesRead = 0;
            byte[] _Value = new byte[sizeof(double)];
            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, sizeof(double), ref bytesRead);
            return BitConverter.ToDouble(_Value, 0);
        }

        public int Readlvl1Pointer(uint address)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }

            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            uint bytesRead = 0;
            byte[] _Value = new byte[4];

            Win32.ReadProcessMemory(_targetProcessHandle, address, _Value, (uint)IntPtr.Size, ref bytesRead);
            uint ptrVal = BitConverter.ToUInt32(_Value, 0);

            Win32.ReadProcessMemory(_targetProcessHandle, ptrVal, _Value, 4, ref bytesRead);

            return BitConverter.ToInt32(_Value, 0);
        }

        private uint ReadPointer(uint adress)
        {
            uint ptrNext;
            uint bytesRead = 0;
            byte[] _Value = new byte[4];

            Win32.ReadProcessMemory(_targetProcessHandle, adress, _Value, (uint)IntPtr.Size, ref bytesRead);
            ptrNext = BitConverter.ToUInt32(_Value, 0);
            return ptrNext;
        }



        /// <summary>
        /// Gets the value holder from a base address and an array of offsets. You can then use WriteXXX, to write whatever you want to it.
        /// </summary>
        /// <param name="staticAddress">Static address to begin the read at.</param>
        /// <param name="Offsets">Array of offset to follow/</param>
        /// <returns></returns>
        public uint GetFinalAddress(uint staticAddress, uint[] Offsets)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (staticAddress == 0) throw new ArgumentNullException("The address is not valid.");
            else if (Offsets == null) throw new ArgumentNullException("The offsets are not valid.");

            uint ptr = ReadPointer(staticAddress);
            for (int i = 0; i < Offsets.Length - 1; i++)
            {
                ptr = ReadPointer(ptr + Offsets[i]);
            }
            ptr = ptr + Offsets[Offsets.Length - 1];
            return ptr;
        }

        /// <summary>
        /// Writes an Array of Bytes to an adress.
        /// </summary>
        /// <param name="address">Address to write the AOB at.</param>
        /// <param name="newValue">AOB to write to the adress.</param>
        public void WriteAOB(uint address, byte[] newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            else if (newValue == null) throw new ArgumentNullException("The new value is not valid.");

            uint bytesWritten = 0;
            Win32.WriteProcessMemory(_targetProcessHandle, address, newValue, (uint)newValue.Length, ref bytesWritten);

        }
        /// <summary>
        /// Writes a byte number to an adress.
        /// </summary>
        /// <param name="adress">Address to write the number to.</param>
        /// <param name="newValue">Value to write at the adress.</param>
        public void WriteByte(uint address, byte newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            else if (newValue == 0) throw new ArgumentNullException("The new value is not valid.");

            uint bytesWritten = 0;
            Byte[] Buffer = BitConverter.GetBytes(newValue);
            Win32.WriteProcessMemory(_targetProcessHandle, address, Buffer, 1, ref bytesWritten);
        }

        /// <summary>
        /// Writes a short (2 bytes) number to an address.
        /// </summary>
        /// <param name="adress">Address to write the number to.</param>
        /// <param name="newValue">Value to write at the adress.</param>
        public void WriteShort(uint address, short newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            else if (newValue == 0) throw new ArgumentNullException("The new value is not valid.");

            uint bytesWritten = 0;
            Byte[] Buffer = BitConverter.GetBytes(newValue);
            Win32.WriteProcessMemory(_targetProcessHandle, address, Buffer, sizeof(short), ref bytesWritten);

        }

        /// <summary>
        /// Writes a integer (4 bytes) number to an adress.
        /// </summary>
        /// <param name="adress">Address to write the number to.</param>
        /// <param name="newValue">Value to write at the adress.</param>
        public void WriteInteger(uint address, int newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");

            else if (newValue == 0) throw new ArgumentNullException("The new value is not valid.");
            uint bytesWritten = 0;
            byte[] Buffer = BitConverter.GetBytes(newValue);
            Win32.WriteProcessMemory(_targetProcessHandle, address, Buffer, sizeof(int), ref bytesWritten);

        }

        /// <summary>
        /// Writes a long (8 bytes) number to an address.
        /// </summary>
        /// <param name="adress">Address to write the number to.</param>
        /// <param name="newValue">Value to write at the adress.</param>
        public void WriteLong(uint address, long newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            else if (newValue == 0) throw new ArgumentNullException("The new value is not valid.");

            uint bytesWritten = 0;
            byte[] Buffer = BitConverter.GetBytes(newValue);
            Win32.WriteProcessMemory(_targetProcessHandle, address, Buffer, sizeof(long), ref bytesWritten);


        }

        /// <summary>
        /// Writes a single precising floating point number to an address.
        /// </summary>
        /// <param name="adress">Address to write the number to.</param>
        /// <param name="newValue">Value to write at the adress.</param>
        public void WriteFloat(uint address, float newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            else if (newValue == 0) throw new ArgumentNullException("The new value is not valid.");

            uint bytesWritten = 0;
            byte[] Buffer = BitConverter.GetBytes(newValue);
            Win32.WriteProcessMemory(_targetProcessHandle, address, Buffer, sizeof(float), ref bytesWritten);
        }

        /// <summary>
        /// Writes a double precision floating point number to an adress.
        /// </summary>
        /// <param name="adress">Address to write the number to.</param>
        /// <param name="newValue">Value to write at the adress.</param>
        public void WriteDouble(uint address, double newValue)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            else if (newValue == 0) throw new ArgumentNullException("The new value is not valid.");

            uint bytesWritten = 0;
            byte[] Buffer = BitConverter.GetBytes(newValue);
            Win32.WriteProcessMemory(_targetProcessHandle, address, Buffer, sizeof(double), ref bytesWritten);

        }

        public uint Protect(uint address, uint size, PAGES_RIGHTS pr = PAGES_RIGHTS.EXECUTE_READWRITE)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            if (size == 0) throw new ArgumentNullException("The size cannot be 0.");

            uint old = 0x0;
            Win32.VirtualProtectEx(_targetProcessHandle, address, size, (uint)pr, ref old);
            return old;
        }

        public void RemoveProtect(uint address, uint size, uint oldProtection)
        {
            if (_targetProcessHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The process handle is not valid. ");
            }
            if (address == 0) throw new ArgumentNullException("The address is not valid.");
            if (size == 0) throw new ArgumentNullException("The size cannot be 0.");

            uint old = 0x0;
            Win32.VirtualProtectEx(_targetProcessHandle, address, size, oldProtection, ref old);
        }
        #endregion
    }

    internal static class Win32
    {


        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(uint dwDesiredAcess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAdress, byte[] lpBuffer, uint iSize, ref uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, uint lpBaseAdress, byte[] lpBuffer, uint nSize, ref uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool VirtualProtectEx(IntPtr hProcess, uint lpAddress, uint dwSize, uint flNewProtect, ref uint lpflOldProtect);



        [Flags]
        internal enum PROCESS_ACCESS_RIGHTS
        {
            VM_OPERATION = 0x8,
            VM_READ = 0x10,
            VM_WRITE = 0x20,
            VM_ALL = (VM_OPERATION | VM_READ | VM_WRITE),
        }


    }

    [Flags]
    public enum PAGES_RIGHTS
    {

        EXECUTE = 0x10,
        EXECUTE_READ = 0x20,
        EXECUTE_READWRITE = 0x40,
        EXECUTE_WRITECOPY = 0x80,
        NOACCESS = 0x1,
        READONLY = 0x2,
        READWRITE = 0x4,
        WRITECOPY = 0x8,

    }

    public static class Extensions
    {
        /// <summary>
        /// Returns a formatted string of the hex number (ex: 0x1337)
        /// </summary>
        /// <param name="hexNumber"></param>
        /// <returns></returns>
        public static string ToHexString(this int hexNumber)
        {
            return string.Format("0x{0}", hexNumber.ToString("X"));
        }

        public static string ToHexString(this long hexNumber)
        {
            return string.Format("0x{0}", hexNumber.ToString("X"));
        }

        public static string ToHexString(this short hexNumber)
        {
            return string.Format("0x{0}", hexNumber.ToString("X"));
        }

        public static string ToHexString(this byte hexNumber)
        {
            return string.Format("0x{0}", hexNumber.ToString("X"));
        }


    }
}