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
		void WriteElement(XmlElement n, NumeralBase to_base, sbyte value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, NumeralBase to_base, byte value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, NumeralBase to_base, short value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, NumeralBase to_base, ushort value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, NumeralBase to_base, int value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, NumeralBase to_base, uint value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		void WriteElement(XmlElement n, NumeralBase to_base, long value)
		{
			WriteElement(n, Convert.ToString(value, (int)to_base));
		}
		/// <summary></summary>
		/// <param name="n">Node element to write</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		// TODO: implement to_base for ulong
		void WriteElement(XmlElement n, NumeralBase to_base, ulong value)
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
		void WriteElement<TEnum>(XmlElement n, TEnum value, bool is_flags = false) where TEnum : struct
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
		public void WriteCursor(NumeralBase to_base, sbyte value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, byte value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, short value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, ushort value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, int value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, uint value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, long value)
		{
			WriteElement(Cursor, to_base, value);
		}
		/// <summary>Set <see cref="Cursor"/>'s value to <paramref name="value"/></summary>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the <see cref="Cursor"/> to</param>
		public void WriteCursor(NumeralBase to_base, ulong value)
		{
			WriteElement(Cursor, to_base, value);
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
		public void WriteCursor<TEnum>(TEnum value, bool is_flags = false) where TEnum : struct
		{
			WriteElement(Cursor, value, is_flags);
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
		public void WriteElement(string name, NumeralBase to_base, sbyte value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, byte value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, short value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, ushort value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, int value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, uint value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, long value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		public void WriteElement(string name, NumeralBase to_base, ulong value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), to_base, value);
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
		public void WriteElement<TEnum>(string name, TEnum value, bool is_flags = false) where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));

			WriteElement(WriteElementAppend(name), value, is_flags);
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
		public void WriteAttribute(string name, NumeralBase to_base, sbyte value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, NumeralBase to_base, byte value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, NumeralBase to_base, short value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, NumeralBase to_base, ushort value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, NumeralBase to_base, int value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, NumeralBase to_base, uint value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			CursorWriteAttribute(name, Convert.ToString(value, (int)to_base));
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		public void WriteAttribute(string name, NumeralBase to_base, long value)
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
		public void WriteAttribute(string name, NumeralBase to_base, ulong value)
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
		public void WriteAttribute<TEnum>(string name, TEnum value, bool is_flags = false) where TEnum : struct
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
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, sbyte value, Predicate<sbyte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, sbyte value, Predicate<sbyte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, byte value, Predicate<byte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, byte value, Predicate<byte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, short value, Predicate<short> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, short value, Predicate<short> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, ushort value, Predicate<ushort> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, ushort value, Predicate<ushort> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, int value, Predicate<int> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, int value, Predicate<int> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, uint value, Predicate<uint> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, uint value, Predicate<uint> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, long value, Predicate<long> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, long value, Predicate<long> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnTrue(string name, NumeralBase to_base, ulong value, Predicate<ulong> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = predicate(value);

			if (result) WriteElement(name, to_base, value);

			return result;
		}
		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <remarks>Does not change <see cref="Cursor"/></remarks>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse(string name, NumeralBase to_base, ulong value, Predicate<ulong> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));

			bool result = !predicate(value);

			if (result) WriteElement(name, to_base, value);

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
		public bool WriteElementOptOnTrue<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = predicate(value);

			if (result) WriteElement(name, value, is_flags);

			return result;
		}

		/// <summary>Create a new element in the underlying <see cref="XmlDocument"/>, relative to <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">The <see cref="XmlElement"/>'s name</param>
		/// <param name="value">Data to set the element's <see cref="XmlElement.InnerText"/> to</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteElementOptOnFalse<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = !predicate(value);

			if (result) WriteElement(name, value, is_flags);

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
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, sbyte value, Predicate<sbyte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, sbyte value, Predicate<sbyte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, byte value, Predicate<byte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, byte value, Predicate<byte> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, short value, Predicate<short> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, short value, Predicate<short> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, ushort value, Predicate<ushort> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, ushort value, Predicate<ushort> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, int value, Predicate<int> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, int value, Predicate<int> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, uint value, Predicate<uint> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, uint value, Predicate<uint> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, long value, Predicate<long> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, long value, Predicate<long> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>is</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnTrue(string name, NumeralBase to_base, ulong value, Predicate<ulong> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = predicate(value);

			if (result) WriteAttribute(name, to_base, value);

			return result;
		}
		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="to_base">Numerical base to use (IE 2, 8, 10, 16). Currently unused.</param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse(string name, NumeralBase to_base, ulong value, Predicate<ulong> predicate)
		{
			Contract.Requires(predicate != null);
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, to_base, value);

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
		public bool WriteAttributeOptOnTrue<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = predicate(value);

			if (result) WriteAttribute(name, value, is_flags);

			return result;
		}

		/// <summary>Create a new attribute for <see cref="Cursor"/></summary>
		/// <param name="predicate">Predicate that defines the conditions for when <paramref name="value"/> <b>isn't</b> written</param>
		/// <param name="name">Name of the <see cref="XmlAttribute"/></param>
		/// <param name="value">Data to set the attribute text to</param>
		/// <param name="is_flags">Is <paramref name="value"/> a <see cref="FlagsAttribute"/> based Enum?</param>
		/// <returns>True if <paramref name="value"/> was written</returns>
		public bool WriteAttributeOptOnFalse<TEnum>(string name, TEnum value, Predicate<TEnum> predicate, bool is_flags = false)
			where TEnum : struct
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(Cursor != null, kContract_CursorNullMsg);
			Contract.Requires(predicate != null);

			bool result = !predicate(value);

			if (result) WriteAttribute(name, value, is_flags);

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