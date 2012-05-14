using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	public static partial class Util
	{
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
			if (type == XmlNodeType.Element)		result = s.StreamElementOpt(mode, name, ref value, PhxLib.Util.kNotNullOrEmpty);
			else if (type == XmlNodeType.Attribute)	result = s.StreamAttributeOpt(mode, name, ref value, PhxLib.Util.kNotNullOrEmpty);
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
	};
}