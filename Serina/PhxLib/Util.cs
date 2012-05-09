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
			void StreamXml(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db);
		};
		[Contracts.ContractClassFor(typeof(IPhxXmlStreamable))]
		abstract class IPhxXmlStreamableContract : IPhxXmlStreamable
		{
			public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db)
			{
				Contract.Requires(s != null);
				Contract.Requires(mode != System.IO.FileAccess.ReadWrite);
				Contract.Requires(db != null);
			}
		};
		#endregion
	};

	public static class Util
	{
		public const int kInvalidInt32 = -1;
		public const float kInvalidSingle = -1.0f;
		public const float kInvalidSingleNaN = float.NaN;

		public static readonly Func<int> kGetInvalidInt32 = () => kInvalidInt32;
		public static readonly Func<float> kGetInvalidSingle = () => kInvalidSingle;

		public static readonly Predicate<bool> kNotFalsePredicate = x => x != false;
		public static readonly Predicate<int> kNotInvalidPredicate = x => x != -1;
		public static readonly Predicate<sbyte> kNotInvalidPredicateSByte = x => x != -1;
		public static readonly Predicate<short> kNotInvalidPredicateInt16 = x => x != -1;
		public static readonly Predicate<float> kNotInvalidPredicateSingle = x => x != -1.0f;
		public static readonly Predicate<float> kNotInvalidPredicateSingleNaN = x => !float.IsNaN(x);
		public static readonly Predicate<float> kNotZeroPredicateSingle = x => x != 0.0f;
		public static readonly Predicate<string> kNotNullOrEmpty = x => !string.IsNullOrEmpty(x);

		public static bool StrEqualsIgnoreCase(string str1, string str2)
		{
			return string.Compare(str1, str2, true) == 0;
		}

		#region XmlElementStream Util
		public const XmlNodeType kSourceAttr = XmlNodeType.Attribute;
		public const XmlNodeType kSourceElement = XmlNodeType.Element;
		public const XmlNodeType kSourceCursor = XmlNodeType.Text;

		public static void StreamString(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref string value, bool to_lower, XmlNodeType type = kSourceAttr, bool intern = false)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(type));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(type) == (name != null));

			if (type == XmlNodeType.Element)		s.StreamElement(mode, name, ref value);
			else if (type == XmlNodeType.Attribute)	s.StreamAttribute(mode, name, ref value);
			else if (type == XmlNodeType.Text)		s.StreamCursor(mode, ref value);

			if (mode == FA.Read)
			{
				if (to_lower) value = value.ToLowerInvariant();
				if (intern) value = string.Intern(value);
			}
		}
		public static bool StreamStringOpt(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref string value, bool to_lower, XmlNodeType type = kSourceAttr, bool intern = false)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(type));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(type) == (name != null));

			bool result = true;
			if (type == XmlNodeType.Element)		result = s.StreamElementOpt(mode, name, ref value, kNotNullOrEmpty);
			else if (type == XmlNodeType.Attribute)	result = s.StreamAttributeOpt(mode, name, ref value, kNotNullOrEmpty);
			else if (type == XmlNodeType.Text)		s.StreamCursor(mode, ref value);

			if (mode == FA.Read && result)
			{
				if (to_lower) value = value.ToLowerInvariant();
				if (intern) value = string.Intern(value);
			}

			return result;
		}

		public static void StreamInternString(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref string value, bool to_lower, XmlNodeType type = kSourceAttr)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(type));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(type) == (name != null));

			StreamString(s, mode, name, ref value, to_lower, type, true);
		}
		public static bool StreamInternStringOpt(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref string value, bool to_lower, XmlNodeType type = kSourceAttr)
		{
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceIsValid(type));
			Contract.Requires(KSoft.IO.XmlElementStream.StreamSourceRequiresName(type) == (name != null));

			return StreamStringOpt(s, mode, name, ref value, to_lower, type, true);
		}

		public static bool StreamElementNamedFlag(KSoft.IO.XmlElementStream s, FA mode, string name,
			ref bool value)
		{
			if (mode == FA.Read) value = s.ElementsExists(name);
			else if (mode == FA.Write && value) s.WriteElement(name);

			return value;
		}

		public static string GetAttributeNameHack(KSoft.IO.XmlElementStream s, string attr_name)
		{
			if (s.AttributeExists(attr_name)) return attr_name;

			attr_name = attr_name.ToLower();
			if (s.AttributeExists(attr_name)) return attr_name;

			throw new InvalidOperationException();
		}
		#endregion

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