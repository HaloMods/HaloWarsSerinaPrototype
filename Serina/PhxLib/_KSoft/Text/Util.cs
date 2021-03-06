﻿using System;
using System.Text;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

namespace KSoft.Text
{
	public static partial class Util
	{
		/// <summary>
		/// Looks for "1", "on", or "true" in <paramref name="str"/> for a true boolean.
		/// Anything else is a false boolean
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool ParseBooleanLazy(string str)
		{
			// NOTE: This implementation differs from BlamLib's.
			// It tested for false stuff: str == "0" || str == "off" || str == "false"

			if (str == "1" ||
				string.Compare(str, "on", true) == 0 ||
				string.Compare(str, "true", true) == 0)
				return true;

			return false;
		}

		#region Enums
		/// <summary>Takes an enum value and return its string representation</summary>
		/// <param name="value">Enum value to convert to a string</param>
		/// <returns></returns>
		public static string EnumToString<TEnum>(TEnum value) where TEnum : struct, IFormattable
		{ return value.ToString("G", null); }
		/// <summary>
		/// Takes an enum value (whose type is assumed to be attributed with
		/// <see cref="FlagsAttribute"/>) and return its string representation 
		/// using commas to separate each flag name that is set
		/// </summary>
		/// <param name="value">Enum value to convert to a string</param>
		/// <returns></returns>
		public static string EnumToFlagsString<TEnum>(TEnum value) where TEnum : struct, IFormattable
		{ return value.ToString("F", null); }
		/// <summary>Takes an enum value and returns it in a hexadecimal formatted string</summary>
		/// <param name="value">Enum value to convert to a string</param>
		/// <returns></returns>
		public static string EnumToHexString<TEnum>(TEnum value) where TEnum : struct, IFormattable
		{ return value.ToString("X", null); }
		#endregion
	};
}