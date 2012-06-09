using System;
using System.IO;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace KSoft.IO
{
	partial class XmlElementStream
	{
		#region Stream Cursor
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref string)"/>
		/// <seealso cref="WriteCursor(string, string)"/>
		public void StreamCursor(FA mode, ref string value)
		{
				 if (mode == FA.Read) ReadCursor(ref value);
			else if (mode == FA.Write) WriteCursor(value);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref char)"/>
		/// <seealso cref="WriteCursor(string, char)"/>
		public void StreamCursor(FA mode, ref char value)
		{
				 if (mode == FA.Read) ReadCursor(ref value);
			else if (mode == FA.Write) WriteCursor(value);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref bool)"/>
		/// <seealso cref="WriteCursor(string, bool)"/>
		public void StreamCursor(FA mode, ref bool value)
		{
				 if (mode == FA.Read) ReadCursor(ref value);
			else if (mode == FA.Write) WriteCursor(value);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref sbyte)"/>
		/// <seealso cref="WriteCursor(string, sbyte)"/>
		public void StreamCursor(FA mode, ref sbyte value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref byte)"/>
		/// <seealso cref="WriteCursor(string, byte)"/>
		public void StreamCursor(FA mode, ref byte value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref short)"/>
		/// <seealso cref="WriteCursor(string, short)"/>
		public void StreamCursor(FA mode, ref short value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref ushort)"/>
		/// <seealso cref="WriteCursor(string, ushort)"/>
		public void StreamCursor(FA mode, ref ushort value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref int)"/>
		/// <seealso cref="WriteCursor(string, int)"/>
		public void StreamCursor(FA mode, ref int value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref uint)"/>
		/// <seealso cref="WriteCursor(string, uint)"/>
		public void StreamCursor(FA mode, ref uint value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref long)"/>
		/// <seealso cref="WriteCursor(string, long)"/>
		public void StreamCursor(FA mode, ref long value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref ulong)"/>
		/// <seealso cref="WriteCursor(string, ulong)"/>
		public void StreamCursor(FA mode, ref ulong value, NumeralBase num_base = NumeralBase.Decimal)
		{
				 if (mode == FA.Read) ReadCursor(ref value, num_base);
			else if (mode == FA.Write) WriteCursor(value, num_base);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref float)"/>
		/// <seealso cref="WriteCursor(string, float)"/>
		public void StreamCursor(FA mode, ref float value)
		{
				 if (mode == FA.Read) ReadCursor(ref value);
			else if (mode == FA.Write) WriteCursor(value);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadCursor(string, ref double)"/>
		/// <seealso cref="WriteCursor(string, double)"/>
		public void StreamCursor(FA mode, ref double value)
		{
				 if (mode == FA.Read) ReadCursor(ref value);
			else if (mode == FA.Write) WriteCursor(value);
		}
		/// <summary>Stream the Value of <see cref="Cursor"/> to or from <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="mode">Read or Write</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <seealso cref="ReadCursor{TEnum}(string, ref TEnum)"/>
		/// <seealso cref="WriteCursor(string, Enum, bool)"/>
		public void StreamCursorEnum<TEnum>(FA mode, ref TEnum value, bool is_flags = false) where TEnum : struct, IFormattable
		{
				 if (mode == FA.Read) ReadCursorEnum(ref value);
			else if (mode == FA.Write) WriteCursorEnum(value, is_flags);
		}
		#endregion

		#region Stream Element
		public void StreamElementBegin(FA mode, string name, out XmlElement old_cursor)
		{
			old_cursor = null;

				 if (mode == FA.Read) ReadElementBegin(name, out old_cursor);
			else if (mode == FA.Write) WriteElementBegin(name, out old_cursor);

			Contract.Assert(old_cursor != null);
		}
		public void StreamElementEnd(FA mode, ref XmlElement old_cursor)
		{
				 if (mode == FA.Read) ReadElementEnd(ref old_cursor);
			else if (mode == FA.Write) WriteElementEnd(ref old_cursor);
		}

		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, ref string)"/>
		/// <seealso cref="WriteElement(string, string)"/>
		public void StreamElement(FA mode, string name, ref string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value);
			else if (mode == FA.Write) WriteElement(name, value);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, ref char)"/>
		/// <seealso cref="WriteElement(string, char)"/>
		public void StreamElement(FA mode, string name, ref char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value);
			else if (mode == FA.Write) WriteElement(name, value);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, ref bool)"/>
		/// <seealso cref="WriteElement(string, bool)"/>
		public void StreamElement(FA mode, string name, ref bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value);
			else if (mode == FA.Write) WriteElement(name, value);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref sbyte)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, sbyte)"/>
		public void StreamElement(FA mode, string name, ref sbyte value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref byte)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, byte)"/>
		public void StreamElement(FA mode, string name, ref byte value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref short)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, short)"/>
		public void StreamElement(FA mode, string name, ref short value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref ushort)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, ushort)"/>
		public void StreamElement(FA mode, string name, ref ushort value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref int)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, int)"/>
		public void StreamElement(FA mode, string name, ref int value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref uint)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, uint)"/>
		public void StreamElement(FA mode, string name, ref uint value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref long)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, long)"/>
		public void StreamElement(FA mode, string name, ref long value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, NumeralBase, ref ulong)"/>
		/// <seealso cref="WriteElement(string, NumeralBase, ulong)"/>
		public void StreamElement(FA mode, string name, ref ulong value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value, num_base);
			else if (mode == FA.Write) WriteElement(name, value, num_base);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, ref float)"/>
		/// <seealso cref="WriteElement(string, float)"/>
		public void StreamElement(FA mode, string name, ref float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value);
			else if (mode == FA.Write) WriteElement(name, value);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadElement(string, ref double)"/>
		/// <seealso cref="WriteElement(string, double)"/>
		public void StreamElement(FA mode, string name, ref double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElement(name, ref value);
			else if (mode == FA.Write) WriteElement(name, value);
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <seealso cref="ReadElement{TEnum}(string, ref TEnum)"/>
		/// <seealso cref="WriteElement(string, Enum, bool)"/>
		public void StreamElementEnum<TEnum>(FA mode, string name, ref TEnum value, bool is_flags = false) where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadElementEnum(name, ref value);
			else if (mode == FA.Write) WriteElementEnum(name, value, is_flags);
		}
		#endregion

		#region StreamElementOpt
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, ref string)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, string, Predicate{string})"/>
		public bool StreamElementOpt(FA mode, string name, ref string value, Predicate<string> predicate = null)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, ref char)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, char, Predicate{char})"/>
		public bool StreamElementOpt(FA mode, string name, ref char value, Predicate<char> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, ref bool)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, bool, Predicate{bool})"/>
		public bool StreamElementOpt(FA mode, string name, ref bool value, Predicate<bool> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref sbyte)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, sbyte, Predicate{sbyte})"/>
		public bool StreamElementOpt(FA mode, string name, ref sbyte value, Predicate<sbyte> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref byte)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, byte, Predicate{byte})"/>
		public bool StreamElementOpt(FA mode, string name, ref byte value, Predicate<byte> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref short)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, short, Predicate{short})"/>
		public bool StreamElementOpt(FA mode, string name, ref short value, Predicate<short> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref ushort)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, ushort, Predicate{ushort})"/>
		public bool StreamElementOpt(FA mode, string name, ref ushort value, Predicate<ushort> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref int)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, int, Predicate{int})"/>
		public bool StreamElementOpt(FA mode, string name, ref int value, Predicate<int> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref uint)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, uint, Predicate{uint})"/>
		public bool StreamElementOpt(FA mode, string name, ref uint value, Predicate<uint> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref long)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, long, Predicate{long})"/>
		public bool StreamElementOpt(FA mode, string name, ref long value, Predicate<long> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, NumeralBase, ref ulong)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, NumeralBase, ulong, Predicate{ulong})"/>
		public bool StreamElementOpt(FA mode, string name, ref ulong value, Predicate<ulong> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, ref float)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, float, Predicate{float})"/>
		public bool StreamElementOpt(FA mode, string name, ref float value, Predicate<float> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadElementOpt(string, ref double)"/>
		/// <seealso cref="WriteElementOptOnTrue(string, double, Predicate{double})"/>
		public bool StreamElementOpt(FA mode, string name, ref double value, Predicate<double> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteElementOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of element <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Element name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsElement"/> based Enum?</param>
		/// <seealso cref="ReadElementOpt{TEnum}(string, ref TEnum)"/>
		/// <seealso cref="WriteElementOptOnTrue{TEnum}(string, Enum, Predicate{TEnum}, bool)"/>
		public bool StreamElementEnumOpt<TEnum>(FA mode, string name, ref TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadElementEnumOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteElementEnumOptOnTrue(name, value, predicate, is_flags);
			return executed;
		}
		#endregion

		#region Stream Attribute
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, ref string)"/>
		/// <seealso cref="WriteAttribute(string, string)"/>
		public void StreamAttribute(FA mode, string name, ref string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value);
			else if (mode == FA.Write) WriteAttribute(name, value);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, ref char)"/>
		/// <seealso cref="WriteAttribute(string, char)"/>
		public void StreamAttribute(FA mode, string name, ref char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value);
			else if (mode == FA.Write) WriteAttribute(name, value);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, ref bool)"/>
		/// <seealso cref="WriteAttribute(string, bool)"/>
		public void StreamAttribute(FA mode, string name, ref bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value);
			else if (mode == FA.Write) WriteAttribute(name, value);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref sbyte)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, sbyte)"/>
		public void StreamAttribute(FA mode, string name, ref sbyte value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref byte)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, byte)"/>
		public void StreamAttribute(FA mode, string name, ref byte value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref short)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, short)"/>
		public void StreamAttribute(FA mode, string name, ref short value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref ushort)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, ushort)"/>
		public void StreamAttribute(FA mode, string name, ref ushort value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref int)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, int)"/>
		public void StreamAttribute(FA mode, string name, ref int value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref uint)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, uint)"/>
		public void StreamAttribute(FA mode, string name, ref uint value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref long)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, long)"/>
		public void StreamAttribute(FA mode, string name, ref long value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, NumeralBase, ref ulong)"/>
		/// <seealso cref="WriteAttribute(string, NumeralBase, ulong)"/>
		public void StreamAttribute(FA mode, string name, ref ulong value, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value, num_base);
			else if (mode == FA.Write) WriteAttribute(name, value, num_base);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, ref float)"/>
		/// <seealso cref="WriteAttribute(string, float)"/>
		public void StreamAttribute(FA mode, string name, ref float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value);
			else if (mode == FA.Write) WriteAttribute(name, value);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <seealso cref="ReadAttribute(string, ref double)"/>
		/// <seealso cref="WriteAttribute(string, double)"/>
		public void StreamAttribute(FA mode, string name, ref double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttribute(name, ref value);
			else if (mode == FA.Write) WriteAttribute(name, value);
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <seealso cref="ReadElement{TEnum}(string, ref TEnum)"/>
		/// <seealso cref="WriteAttribute(string, Enum, bool)"/>
		public void StreamAttributeEnum<TEnum>(FA mode, string name, ref TEnum value, bool is_flags = false) where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

				 if (mode == FA.Read) ReadAttributeEnum(name, ref value);
			else if (mode == FA.Write) WriteAttributeEnum(name, value, is_flags);
		}
		#endregion

		#region StreamAttributeOpt
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, ref string)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, string, Predicate{string})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref string value, Predicate<string> predicate = null)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, ref char)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, char, Predicate{char})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref char value, Predicate<char> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, ref bool)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, bool, Predicate{bool})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref bool value, Predicate<bool> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref sbyte)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, sbyte, Predicate{sbyte})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref sbyte value, Predicate<sbyte> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref byte)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, byte, Predicate{byte})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref byte value, Predicate<byte> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref short)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, short, Predicate{short})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref short value, Predicate<short> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref ushort)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, ushort, Predicate{ushort})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref ushort value, Predicate<ushort> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref int)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, int, Predicate{int})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref int value, Predicate<int> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref uint)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, uint, Predicate{uint})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref uint value, Predicate<uint> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref long)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, long, Predicate{long})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref long value, Predicate<long> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="num_base">numerical base to use</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, NumeralBase, ref ulong)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, NumeralBase, ulong, Predicate{ulong})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref ulong value, Predicate<ulong> predicate, NumeralBase num_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value, num_base);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate, num_base);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, ref float)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, float, Predicate{float})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref float value, Predicate<float> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <seealso cref="ReadAttributeOpt(string, ref double)"/>
		/// <seealso cref="WriteAttributeOptOnTrue(string, double, Predicate{double})"/>
		public bool StreamAttributeOpt(FA mode, string name, ref double value, Predicate<double> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteAttributeOptOnTrue(name, value, predicate);
			return executed;
		}
		/// <summary>Stream the Value of attribute <paramref name="name"/> to or from <paramref name="value"/></summary>
		/// <typeparam name="TEnum">Enumeration type</typeparam>
		/// <param name="mode">Read or Write</param>
		/// <param name="name">Attribute name</param>
		/// <param name="value">Source or destination value</param>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <returns>True if <paramref name="value"/> was read/written from/to stream</returns>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <seealso cref="ReadAttributeOpt{TEnum}(string, ref TEnum)"/>
		/// <seealso cref="WriteAttributeOptOnTrue{TEnum}(string, Enum, Predicate{TEnum}, bool)"/>
		public bool StreamAttributeEnumOpt<TEnum>(FA mode, string name, ref TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			if (predicate == null) predicate = x => true;

			bool executed = false;
				 if (mode == FA.Read) executed = ReadAttributeEnumOpt(name, ref value);
			else if (mode == FA.Write) executed = WriteAttributeEnumOptOnTrue(name, value, predicate, is_flags);
			return executed;
		}
		#endregion
	};
}