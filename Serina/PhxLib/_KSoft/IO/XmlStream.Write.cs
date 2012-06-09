using System;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

namespace KSoft.IO
{
	partial class XmlElementStream
	{
		void AppendElement(XmlElement e)
		{
			// if there is a node in scope, add the element after it and use it as the new scope
			if(m_cursor != null)
				m_cursor.AppendChild(e);
			else // if there is no XML node in scope, assume we're adding to the root
				m_root.AppendChild(e);
		}

		void NestElement(XmlElement e, out XmlElement old_cursor)
		{
			old_cursor = null;

			if (m_cursor != null)
			{
				m_cursor.AppendChild(e);

				old_cursor = Cursor;
				Cursor = e;
			}
			else // if there is no XML node in scope, assume we're adding to the root
			{
				m_root.DocumentElement.AppendChild(e);
				m_cursor = e;
			}
		}

		#region WriteElement impl
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, string value)
		{
			n.InnerText = value;

			//var text = m_root.CreateTextNode(value);
			//n.AppendChild(text);
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, char value)
		{
			WriteElement(n, new string(value, 1));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, bool value)
		{
			WriteElement(n, value.ToString());
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, sbyte value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, byte value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, short value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, ushort value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, int value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, uint value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, long value, NumeralBase to_base)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		// TODO: implement to_base for ulong
		void WriteElement(XmlElement n, ulong value, NumeralBase to_base)
		{
			if (to_base != NumeralBase.Decimal) throw new NotImplementedException("TODO: implement NumeralBase for ulong");

			WriteElement(n, Convert.ToString(value/*, (int)to_base*/));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, float value)
		{
			WriteElement(n, value.ToString());
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, double value)
		{
			WriteElement(n, value.ToString());
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <param name="is_flags">Is <paramref name="enum_value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		void WriteElementEnum<TEnum>(XmlElement n, TEnum value, bool is_flags) where TEnum : struct, IFormattable
		{
			WriteElement(n, is_flags ?
				Text.Util.EnumToFlagsString(value) :
				Text.Util.EnumToString(value));
		}
		#endregion

		#region WriteCursor
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(string value)
		{
			Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(value));

			WriteElement(Cursor, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(char value)
		{
			WriteElement(Cursor, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(bool value)
		{
			WriteElement(Cursor, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(sbyte value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(byte value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(short value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(ushort value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(int value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(uint value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(long value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(ulong value, NumeralBase to_base = NumeralBase.Decimal)
		{
			WriteElement(Cursor, value, to_base);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(float value)
		{
			WriteElement(Cursor, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(double value)
		{
			WriteElement(Cursor, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		/// <param name="is_flags">Is <paramref name="enum_value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		public void WriteCursorEnum<TEnum>(TEnum value, bool is_flags = false) where TEnum : struct, IFormattable
		{
			WriteElementEnum(Cursor, value, is_flags);
		}
		#endregion

		#region WriteElement
		XmlElement WriteElementAppend(string name)
		{
			ValidateWritePermission();

			XmlElement e = m_root.CreateElement(name);
			AppendElement(e);

			return e;
		}

		XmlElement WriteElementNest(string name, out XmlElement old_cursor)
		{
			ValidateWritePermission();

			XmlElement e = m_root.CreateElement(name);
			NestElement(e, out old_cursor);

			return e;
		}

		/// <summary>Create a new element under <see cref="Cursor"/> or the root element in the underlying <see cref="XmlDocument"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="old_cursor">On return, contains the previous <see cref="Cursor"/> value</param>
		public void WriteElementBegin(string name, out XmlElement old_cursor)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElementNest(name, out old_cursor);
		}
		/// <summary>Restore the cursor to what it was before the corresponding call to a <see cref="WriteElementBegin(string, XmlElement&amp;)"/></summary>
		public void WriteElementEnd(ref XmlElement old_cursor)
		{
			Contract.Ensures(Contract.ValueAtReturn(out old_cursor) == null);

			RestoreCursor(ref old_cursor);
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElementAppend(name);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(value));

			WriteElement(WriteElementAppend(name), value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, sbyte value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, byte value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, short value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, ushort value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, int value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, uint value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, long value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, ulong value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, to_base);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <param name="is_flags">Is <paramref name="enum_value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElementEnum<TEnum>(string name, TEnum value, bool is_flags = false) where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElementEnum(WriteElementAppend(name), value, is_flags);
		}
		#endregion

		const string kContract_CursorNullMsg = "Element cursor must not be null when writing an attribute.";

		#region WriteAttribute
		void CursorWriteAttribute(string name, string value)
		{
			ValidateWritePermission();

			m_cursor.SetAttribute(name, value);
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(value));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, value);
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, char value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, new string(value, 1));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, bool value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, value.ToString());
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, sbyte value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, byte value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, short value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, ushort value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, int value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, uint value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, long value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the attribute text to</param>
		// TODO: implement to_base for ulong
		public void WriteAttribute(string name, ulong value, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			if (to_base != NumeralBase.Decimal) throw new NotImplementedException("TODO: implement NumeralBase for ulong");

			CursorWriteAttribute(name, Convert.ToString(value/*, (int)to_base*/));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, float value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, value.ToString());
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, double value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, value.ToString());
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <param name="is_flags">Is <paramref name="enum_value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		public void WriteAttributeEnum<TEnum>(string name, TEnum value, bool is_flags = false) where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, is_flags ?
				Text.Util.EnumToFlagsString(value) :
				Text.Util.EnumToString(value));
		}
		#endregion

		// NOTE: the Predicate used to be the first argument but that was soon realized to be just nasty, especially 
		// when using anonymous delegates or lambda functions. So the function signatures were refracted but I can't 
		// be arsed to update the ordering of the documentation or Contract calls

		#region WriteElementOpt
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, string value, Predicate<string> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			//Contract.Requires<ArgumentNullException>(value != null);

			bool result = predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, string value, Predicate<string> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			//Contract.Requires<ArgumentNullException>(value != null);
			if (predicate != string.IsNullOrEmpty && value == null)
				throw new ArgumentNullException("value");

			bool result = !predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, char value, Predicate<char> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, char value, Predicate<char> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, bool value, Predicate<bool> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, bool value, Predicate<bool> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, sbyte value, Predicate<sbyte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, sbyte value, Predicate<sbyte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, byte value, Predicate<byte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, byte value, Predicate<byte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, short value, Predicate<short> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, short value, Predicate<short> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, ushort value, Predicate<ushort> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, ushort value, Predicate<ushort> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, int value, Predicate<int> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, int value, Predicate<int> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, uint value, Predicate<uint> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, uint value, Predicate<uint> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, long value, Predicate<long> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, long value, Predicate<long> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, ulong value, Predicate<ulong> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, ulong value, Predicate<ulong> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value, to_base);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, float value, Predicate<float> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, float value, Predicate<float> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, double value, Predicate<double> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, double value, Predicate<double> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementEnumOptOnTrue<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = predicate(value);

			if (result) WriteElementEnum(name, value, is_flags);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementEnumOptOnFalse<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = !predicate(value);

			if (result) WriteElementEnum(name, value, is_flags);

			return result;
		}
		#endregion

		#region WriteAttributeOpt
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, string value, Predicate<string> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			//Contract.Requires<ArgumentNullException>(value != null);
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, string value, Predicate<string> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			//Contract.Requires<ArgumentNullException>(value != null);
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			if (predicate != string.IsNullOrEmpty && value == null)
				throw new ArgumentNullException("value");

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, char value, Predicate<char> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(predicate != null);
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, char value, Predicate<char> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(predicate != null);
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, bool value, Predicate<bool> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(predicate != null);
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, bool value, Predicate<bool> predicate)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(predicate != null);
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, sbyte value, Predicate<sbyte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, sbyte value, Predicate<sbyte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, byte value, Predicate<byte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, byte value, Predicate<byte> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, short value, Predicate<short> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, short value, Predicate<short> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, ushort value, Predicate<ushort> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, ushort value, Predicate<ushort> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, int value, Predicate<int> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, int value, Predicate<int> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, uint value, Predicate<uint> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, uint value, Predicate<uint> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, long value, Predicate<long> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, long value, Predicate<long> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, ulong value, Predicate<ulong> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, ulong value, Predicate<ulong> predicate, NumeralBase to_base = NumeralBase.Decimal)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, to_base);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, float value, Predicate<float> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, float value, Predicate<float> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, double value, Predicate<double> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, double value, Predicate<double> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeEnumOptOnTrue<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = predicate(value);

			if (result) WriteAttributeEnum(name, value, is_flags);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeEnumOptOnFalse<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct, IFormattable
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = !predicate(value);

			if (result) WriteAttributeEnum(name, value, is_flags);

			return result;
		}
		#endregion
	};

	/// <summary>
	/// Helper type for exposing the <see cref="XmlElementStream.WriteElementBegin(string)">WriteElementBegin</see> and 
	/// <see cref="XmlElementStream.WriteElementEnd()">WriteElementEnd</see> in a way which works with the C# "using" statements
	/// </summary>
	public struct XmlElementStreamWriteBookmark : IDisposable
	{
		XmlElementStream m_stream;
		XmlElement m_oldCursor;

		/// <summary>Saves the stream's cursor so a new one can be specified, but then later restored to the saved cursor, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="element_name"></param>
		public XmlElementStreamWriteBookmark(XmlElementStream stream, string element_name)
		{
			Contract.Requires<ArgumentNullException>(stream != null);
			Contract.Requires<ArgumentNullException>(element_name != null);

			(m_stream = stream).WriteElementBegin(element_name, out m_oldCursor);
		}

		/// <summary>Returns the cursor of the underlying stream to the last saved cursor value</summary>
		public void Dispose()
		{
			if (m_stream != null)
			{
				m_stream.WriteElementEnd(ref m_oldCursor);
				m_stream = null;
			}
		}
	};
}