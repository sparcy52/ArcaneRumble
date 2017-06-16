using System;

namespace Model
{
	public static class Utils
	{
		/// <summary>
		/// Print and object as a string to Unity's console
		/// </summary>
		/// <param name="obj">object print</param>
		public static void Print(object obj)
		{
			UnityEngine.Debug.Log(obj);
		}

		/// <summary>
		/// Print a formatted string to Unity's console. Details here:
		/// https://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx#Starting
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="args">arguments</param>
		public static void Printf(string format, params object[] args)
		{
			Print(string.Format(format, args));
		}
	}
}