using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using DateTimeStyles = System.Globalization.DateTimeStyles;

namespace PhxLib
{
	namespace IO
	{
		#region IPhxXmlStreamable
		[Contracts.ContractClass(typeof(IPhxXmlStreamableContract))]
		public interface IPhxXmlStreamable
		{
			void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs);
		};
		[Contracts.ContractClassFor(typeof(IPhxXmlStreamable))]
		abstract class IPhxXmlStreamableContract : IPhxXmlStreamable
		{
			public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
			{
				Contract.Requires(s != null);
				Contract.Requires(mode != System.IO.FileAccess.ReadWrite);
				Contract.Requires(xs != null);
			}
		};
		#endregion
	};

	public static partial class Util
	{
		public const int kInvalidInt32 = -1;
		public const float kInvalidSingle = -1.0f;
		public const float kInvalidSingleNaN = float.NaN;

		/// <summary>Sentinel for cases which reference undefined data (eg, an undefined ProtoObject)</summary>
		public const int kInvalidReference = kInvalidInt32 - 1;

		public static readonly Func<int> kGetInvalidInt32 = () => kInvalidInt32;
		public static readonly Func<float> kGetInvalidSingle = () => kInvalidSingle;

		public static readonly Predicate<bool> kNotFalsePredicate = x => x != false;
		public static readonly Predicate<int> kNotInvalidPredicate = x => x > kInvalidInt32;
		public static readonly Predicate<sbyte> kNotInvalidPredicateSByte = x => x > kInvalidInt32;
		public static readonly Predicate<short> kNotInvalidPredicateInt16 = x => x > kInvalidInt32;
		public static readonly Predicate<float> kNotInvalidPredicateSingle = x => x > kInvalidSingle;
		public static readonly Predicate<float> kNotInvalidPredicateSingleNaN = x => !float.IsNaN(x);
		public static readonly Predicate<float> kNotZeroPredicateSingle = x => x != 0.0f;
		public static readonly Predicate<string> kNotNullOrEmpty = x => !string.IsNullOrEmpty(x);

		public static bool StrEqualsIgnoreCase(string str1, string str2)
		{
			return string.Compare(str1, str2, true) == 0;
		}

		public static void DisposeAndNull<T>(ref T d)
			where T : class, IDisposable
		{
			if (d != null)
			{
				d.Dispose();
				d = null;
			}
		}

		public static bool ParseGameTime(string str, out DateTime game_time, out string error_details)
		{
			const DateTimeStyles k_styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | 
				DateTimeStyles.NoCurrentDateDefault;

			bool result = false;
			game_time = new DateTime(1, 1, 1);
			error_details = "";

			if (!str.Contains(':'))
			{
				int seconds;
				result = int.TryParse(str, out seconds);

				if (!result)
					error_details = "Invalid 'seconds' value";
				else
					game_time = game_time.AddSeconds(seconds);
			}
			else
			{
				result = DateTime.TryParseExact(str, "mm:ss", 
					System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat, k_styles, out game_time);

				if (!result)
					error_details = "Invalid 'time' value";
			}

			return result;
		}
	};
}

namespace KSoft
{
	/// <summary>Valid numerical bases (radix) that can be used in this library suite</summary>
	public enum NumeralBase : byte
	{
		Binary = 2,
		Octal = 8,
		Decimal = 10,
		Hex = 16,
	};

	public static class Util
	{
		public static bool CanRead(this System.IO.FileAccess value)			{ return (value & System.IO.FileAccess.Read) != 0; }
		public static bool CanWrite(this System.IO.FileAccess value)		{ return (value & System.IO.FileAccess.Write) != 0; }
	};
}