namespace Memory
{
	public static class Extensions
	{
		/// <summary>
		/// Returns a formatted string of the hex number (ex: 0x1337)
		/// </summary>
		public static string ToHexString(this int hexNumber)
		{
			return string.Format("0x{0:X}", hexNumber);
		}
		
		/// <summary>
		/// Returns a formatted string of the hex number (ex: 0x1337)
		/// </summary>
		public static string ToHexString(this long hexNumber)
		{
			return string.Format("0x{0:X}", hexNumber);
		}
		
		/// <summary>
		/// Returns a formatted string of the hex number (ex: 0x1337)
		/// </summary>
		public static string ToHexString(this short hexNumber)
		{
			return string.Format("0x{0:X}", hexNumber);
		}
		
		/// <summary>
		/// Returns a formatted string of the hex number (ex: 0x1337)
		/// </summary>
		public static string ToHexString(this byte hexNumber)
		{
			return string.Format("0x{0:X}", hexNumber);
		}


	}
}