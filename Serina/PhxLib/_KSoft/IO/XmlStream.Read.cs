using System;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

namespace KSoft.IO
{
	partial class XmlElementStream
	{
		/// <summary>Should Enums read from the steam be treated with no respect to case sensitivity?</summary>
		/// <remarks>If true, 'CanRead' would be parsed where 'canread' appears</remarks>
		public bool IgnoreCaseOnEnums { get; set; }

		public bool ExceptionOnEnumParseFail { get; set; }

		static int EnumToInt<TEnum>(TEnum value) where TEnum : struct, IConvertible
		{
			// Note: Enum's convertible implementation isn't efficient. Uses 'GetValue' which returns the value in a boxed object
			return value.ToInt32(null);
		}

		bool ParseEnum<TEnum>(string str, out TEnum value) where TEnum : struct
		{
			bool result = Enum.TryParse<TEnum>(str, IgnoreCaseOnEnums, out value);

			if (!result && ExceptionOnEnumParseFail)
				throw new ArgumentException("Parameter is not a member of " + typeof(TEnum), str);

			return result;
		}
		bool ParseEnum<TEnum>(string str, out int int_value) where TEnum : struct, IConvertible
		{
			int_value = 0;

			TEnum value;
			bool result = ParseEnum(str, out value);

			if (result) int_value = EnumToInt(value);

			return result;
		}
		bool ParseEnumOpt<TEnum>(string str, out TEnum value) where TEnum : struct
		{
			bool result = Enum.TryParse<TEnum>(str, IgnoreCaseOnEnums, out value);

			return result;
		}
		bool ParseEnumOpt<TEnum>(string str, out int int_value) where TEnum : struct, IConvertible
		{
			int_value = 0;

			TEnum value;
			bool result = ParseEnumOpt(str, out value);

			if (result) int_value = EnumToInt(value);

			return result;
		}

		// NOTE:
		// We use the constraint "where TEnum : struct" because you can't use "enum" or "System.Enum", for reasons I would like to know...

		// TODO: document that 'ref value' will equal the streamed value or 'null' after returning, depending on success

		#region ReadElement impl
		static string GetInnerText(XmlElement n)
		{
			//return n.InnerText;

			var text_node = n.LastChild;
			return text_node != null ? text_node.Value : null; // TextNode's value
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="n">Node element to read</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref string value)
		{
			value = GetInnerText(n);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="n">Node element to read</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref char value)
		{
			value = GetInnerText(n)[0];
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="n">Node element to read</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref bool value)
		{
			value = Text.Util.ParseBooleanLazy(GetInnerText(n));
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref sbyte value, NumeralBase from_base)
		{
			value = Convert.ToSByte(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref byte value, NumeralBase from_base)
		{
			value = Convert.ToByte(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref short value, NumeralBase from_base)
		{
			value = Convert.ToInt16(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref ushort value, NumeralBase from_base)
		{
			value = Convert.ToUInt16(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref int value, NumeralBase from_base)
		{
			value = Convert.ToInt32(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref uint value, NumeralBase from_base)
		{
			value = Convert.ToUInt32(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref long value, NumeralBase from_base)
		{
			value = Convert.ToInt64(GetInnerText(n), (int)from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="n">Node element to read</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref ulong value, NumeralBase from_base)
		{
			value = Convert.ToUInt64(GetInnerText(n), (int)from_base);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="n">Node element to read</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref float value)
		{
			value = Convert.ToSingle(GetInnerText(n));
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="n">Node element to read</param>
		/// <param name="value">value to receive the data</param>
		void ReadElement(XmlElement n, ref double value)
		{
			value = Convert.ToDouble(GetInnerText(n));
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into the enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="n">Node element to read</param>
		/// <param name="enum_value">value to receive the data</param>
		void ReadElementEnum<TEnum>(XmlElement n, ref TEnum enum_value) where TEnum : struct
		{
			ParseEnum<TEnum>(GetInnerText(n), out enum_value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into the enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="n">Node element to read</param>
		/// <param name="enum_value">value to receive the data</param>
		void ReadElementEnum<TEnum>(XmlElement n, ref int enum_value) where TEnum : struct, IConvertible
		{
			ParseEnum<TEnum>(GetInnerText(n), out enum_value);
		}
		#endregion

		/// <summary>Interpret the Name of <see cref="Cursor"/> as a member of <typeparamref name="TEnum"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="enum_value">value to receive the data</param>
		public void ReadCursorName<TEnum>(ref TEnum enum_value) where TEnum : struct
		{
			ParseEnum<TEnum>(CursorName, out enum_value);
		}

		#region ReadCursor
		/// <summary>Stream out the Value of <see cref="Cursor"/> into <paramref name="value"/></summary>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref string value)
		{
			ReadElement(Cursor, ref value);
		}
		/// <summary>Stream out the Value of <see cref="Cursor"/> into <paramref name="value"/></summary>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref char value)
		{
			ReadElement(Cursor, ref value);
		}
		/// <summary>Stream out the Value of <see cref="Cursor"/> into <paramref name="value"/></summary>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref bool value)
		{
			ReadElement(Cursor, ref value);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref sbyte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref byte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref short value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref ushort value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref int value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref uint value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of <see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref long value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>
		/// Stream out the Value of<see cref="Cursor"/>
		/// using numerical base of <paramref name="base"/> into <paramref name="value"/>
		/// </summary>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref ulong value, NumeralBase from_base = NumeralBase.Decimal)
		{
			ReadElement(Cursor, ref value, from_base);
		}
		/// <summary>Stream out the Value of <see cref="Cursor"/> into <paramref name="value"/></summary>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref float value)
		{
			ReadElement(Cursor, ref value);
		}
		/// <summary>Stream out the Value of <see cref="Cursor"/> into <paramref name="value"/></summary>
		/// <param name="value">value to receive the data</param>
		public void ReadCursor(ref double value)
		{
			ReadElement(Cursor, ref value);
		}
		/// <summary>Stream out the Value of <see cref="Cursor"/> into the enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="enum_value">value to receive the data</param>
		public void ReadCursorEnum<TEnum>(ref TEnum enum_value) where TEnum : struct
		{
			ReadElementEnum(Cursor, ref enum_value);
		}
		/// <summary>Stream out the Value of <see cref="Cursor"/> into the enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="enum_value">value to receive the data</param>
		public void ReadCursorEnum<TEnum>(ref int enum_value) where TEnum : struct, IConvertible
		{
			ReadElementEnum(Cursor, ref enum_value);
		}
		#endregion

		#region ReadElement
		public void ReadElementBegin(string name, out XmlElement old_cursor)
		{
			ValidateReadPermission();

			XmlElement n = m_cursor[name];
			Contract.Assert(n != null);

			old_cursor = Cursor;
			Cursor = n;
		}
		/// <summary>Restore the cursor to what it was before the corresponding call to a <see cref="ReadElementBegin(string, XmlElement&amp;)"/></summary>
		public void ReadElementEnd(ref XmlElement old_cursor)
		{
			Contract.Ensures(Contract.ValueAtReturn(out old_cursor) == null);

			RestoreCursor(ref old_cursor);
		}

		XmlElement GetElement(string name)
		{
			ValidateReadPermission();

			XmlElement n = m_cursor[name];
			Contract.Assert(n != null);
			//Debug.Assert.If(n != null, "Tried to read element '{0}' from node '{1}' in {2}", name, node.Name, m_owner);
			return n;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref sbyte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref byte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref short value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref ushort value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref int value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref uint value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref long value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref ulong value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value, from_base);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadElement(string name, ref double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElement(GetElement(name), ref value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into the enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Element name</param>
		/// <param name="enum_value">value to receive the data</param>
		public void ReadElementEnum<TEnum>(string name, ref TEnum enum_value) where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElementEnum(GetElement(name), ref enum_value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into the enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Element name</param>
		/// <param name="enum_value">value to receive the data</param>
		public void ReadElementEnum<TEnum>(string name, ref int enum_value) where TEnum : struct, IConvertible
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ReadElementEnum(GetElement(name), ref enum_value);
		}
		#endregion

		#region ReadAttribute
		/// <summary>Streams out the attribute data of <paramref name="name"/></summary>
		/// <param name="name">Attribute name</param>
		/// <returns></returns>
		string ReadAttribute(string name)
		{
			ValidateReadPermission();

			XmlNode n = m_cursor.Attributes[name];
			Contract.Assert(n != null);
			//Debug.Assert.If(n != null, "Tried to read attribute '{0}' from node '{1}' in {2}", name, node.Name, m_owner);
			return n.Value;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = ReadAttribute(name);
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = ReadAttribute(name)[0];
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Text.Util.ParseBooleanLazy(ReadAttribute(name));
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref sbyte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToSByte(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref byte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToByte(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref short value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToInt16(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref ushort value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToUInt16(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref int value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToInt32(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref uint value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToUInt32(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref long value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToInt64(ReadAttribute(name), (int)from_base);
		}
		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref ulong value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToUInt64(ReadAttribute(name), (int)from_base);
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToSingle(ReadAttribute(name));
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		public void ReadAttribute(string name, ref double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = Convert.ToDouble(ReadAttribute(name));
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Attribute name</param>
		/// <param name="enum_value">enum value to receive the data</param>
		public void ReadAttributeEnum<TEnum>(string name, ref TEnum enum_value) where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ParseEnum(ReadAttribute(name), out enum_value);
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Attribute name</param>
		/// <param name="enum_value">enum value to receive the data</param>
		public void ReadAttributeEnum<TEnum>(string name, ref int enum_value) where TEnum : struct, IConvertible
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			ParseEnum(ReadAttribute(name), out enum_value);
		}
		#endregion

		#region ReadElementOpt
		/// <summary>Streams out the InnerText of element <paramref name="name"/></summary>
		/// <param name="name">Element name</param>
		/// <returns></returns>
		string ReadElementOpt(string name)
		{
			ValidateReadPermission();

			XmlElement n = m_cursor[name];
			if (n == null) return null;

			string it = GetInnerText(n);
			if (!string.IsNullOrEmpty(it)) return it;

			return null;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = ReadElementOpt(name);
			return value != null;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = str[0]; return true;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Text.Util.ParseBooleanLazy(str); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref sbyte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToSByte(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref byte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToByte(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref short value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToInt16(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref ushort value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToUInt16(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref int value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToInt32(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref uint value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToUInt32(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref long value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToInt64(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the InnerText of element <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref ulong value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToUInt64(str, (int)from_base); return true;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToSingle(str); return true;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Element name</param>
		/// <param name="value">value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementOpt(string name, ref double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToDouble(str); return true;
		}

		/// <summary>Stream out the InnerText of element <paramref name="name"/> into enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Element name</param>
		/// <param name="enum_value">enum value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementEnumOpt<TEnum>(string name, ref TEnum enum_value) where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			return ParseEnumOpt(str, out enum_value);
		}
		/// <summary>Stream out the InnerText of element <paramref name="name"/> into enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Element name</param>
		/// <param name="enum_value">enum value to receive the data</param>
		/// <remarks>If inner text is just an empty string, the stream ignores its existence</remarks>
		/// <returns>true if the value exists</returns>
		public bool ReadElementEnumOpt<TEnum>(string name, ref int enum_value) where TEnum : struct, IConvertible
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadElementOpt(name); if (string.IsNullOrEmpty(str)) return false;
			return ParseEnumOpt(str, out enum_value);
		}
		#endregion

		#region ReadAttributeOpt
		/// <summary>Streams out the attribute data of <paramref name="name"/></summary>
		/// <param name="name">Attribute name</param>
		/// <returns></returns>
		string ReadAttributeOpt(string name)
		{
			ValidateReadPermission();

			XmlNode n = m_cursor.Attributes[name];
			if (n != null)	return n.Value;
			else			return null;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			value = ReadAttributeOpt(name);
			return value != null;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = str[0]; return true;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Text.Util.ParseBooleanLazy(str); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref sbyte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToSByte(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref byte value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToByte(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref short value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToInt16(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref ushort value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToUInt16(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref int value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToInt32(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref uint value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToUInt32(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref long value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToInt64(str, (int)from_base); return true;
		}

		/// <summary>
		/// Stream out the attribute data of <paramref name="name"/>
		/// using numerical base of <paramref name="base"/> into 
		/// <paramref name="value"/>
		/// </summary>
		/// <param name="name">Attribute name</param>
		/// <param name="from_base">numerical base to use</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref ulong value, NumeralBase from_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToUInt64(str, (int)from_base); return true;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToSingle(str); return true;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into <paramref name="value"/></summary>
		/// <param name="name">Attribute name</param>
		/// <param name="value">value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeOpt(string name, ref double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			value = Convert.ToDouble(str); return true;
		}

		/// <summary>Stream out the attribute data of <paramref name="name"/> into enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Attribute name</param>
		/// <param name="enum_value">enum value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeEnumOpt<TEnum>(string name, ref TEnum enum_value) where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			return ParseEnumOpt(str, out enum_value);
		}
		/// <summary>Stream out the attribute data of <paramref name="name"/> into enum <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="name">Attribute name</param>
		/// <param name="enum_value">enum value to receive the data</param>
		/// <returns>true if the value exists</returns>
		public bool ReadAttributeEnumOpt<TEnum>(string name, ref int enum_value) where TEnum : struct, IConvertible
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			string str = ReadAttributeOpt(name); if (string.IsNullOrEmpty(str)) return false;
			return ParseEnumOpt(str, out enum_value);
		}
		#endregion
	};

	/// <summary>
	/// Helper type for exposing the <see cref="XmlElementStream.SaveCursor()">SaveCursor</see> and 
	/// <see cref="XmlElementStream.RestoreCursor()">RestoreCursor</see> in a way which works with the C# "using" statements
	/// </summary>
	public struct XmlElementStreamReadBookmark : IDisposable
	{
		XmlElementStream m_stream;
		XmlElement m_oldCursor;

		/// <summary>Saves the stream's cursor so a new one can be specified, but then later restored to the saved cursor, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		public XmlElementStreamReadBookmark(XmlElementStream stream)
		{
			Contract.Requires<ArgumentNullException>(stream != null);

			(m_stream = stream).SaveCursor(null, out m_oldCursor);
		}
		/// <summary>Saves the stream's cursor and sets <paramref name="new_cursor"/> to be the new cursor for the stream</summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="new_cursor">The new cursor for the stream</param>
		public XmlElementStreamReadBookmark(XmlElementStream stream, XmlElement new_cursor)
		{
			Contract.Requires<ArgumentNullException>(stream != null);
			Contract.Requires<ArgumentNullException>(new_cursor != null);

			(m_stream = stream).SaveCursor(new_cursor, out m_oldCursor);
		}

		/// <summary>Returns the cursor of the underlying stream to the last saved cursor value</summary>
		public void Dispose()
		{
			if (m_stream != null)
			{
				m_stream.RestoreCursor(ref m_oldCursor);
				m_stream = null;
			}
		}
	};
}