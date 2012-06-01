using System;
using System.IO;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using Debug = PhxLib.Debug;

namespace KSoft.IO
{
	#region IXmlElementStreamable
	[Contracts.ContractClass(typeof(IXmlElementStreamableContract))]
	public interface IXmlElementStreamable
	{
		void StreamXml(XmlElementStream s, System.IO.FileAccess stream_mode);
	};
	[Contracts.ContractClassFor(typeof(IXmlElementStreamable))]
	abstract class IXmlElementStreamableContract : IXmlElementStreamable
	{
		public void StreamXml(XmlElementStream s, System.IO.FileAccess stream_mode)
		{
			Contract.Requires(s != null);
			Contract.Requires(stream_mode != System.IO.FileAccess.ReadWrite);
		}
	};
	#endregion

	public sealed partial class XmlElementStream : /*IKSoftStream,*/ IDisposable
	{
		/// <summary>XmlNodes which we support explicit streaming on</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[Contracts.Pure]
		public static bool StreamSourceIsValid(XmlNodeType type)
		{
			switch (type)
			{
				case XmlNodeType.Element:
				case XmlNodeType.Attribute:
				case XmlNodeType.Text: // aka, Cursor
					return true;

				default: return false;
			}
		}
		/// <summary>Does the requested type require an associated name in the Xml?</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[Contracts.Pure]
		public static bool StreamSourceRequiresName(XmlNodeType type)
		{
			Contract.Requires(StreamSourceIsValid(type));

			return type != XmlNodeType.Text; // aka, Cursor
		}

		#region Owner
		/// <summary>Owner of this stream</summary>
		public object Owner { get; set; }

		/// <summary></summary>
		/// <param name="new_owner"></param>
		/// <returns></returns>
		public /*IDisposable*/IKSoftStreamOwnerBookmark EnterOwnerBookmark(object new_owner = null)
		{
			return new IKSoftStreamOwnerBookmark(this, new_owner);
		}
		#endregion

		#region StreamName
		/// <summary></summary>
		/// <remarks>If this is for a file, this is the file name this stream is handling</remarks>
		public string StreamName { get; private set; }
		#endregion

		#region Root document
		XmlDocument m_root;
		/// <summary>Backing XmlDocument for this element stream</summary>
		public XmlDocument Document { get { return m_root; } }
		#endregion

		#region Cursor
		XmlElement m_cursor;
		/// <summary>Element data we are streaming data to and from</summary>
		public XmlElement Cursor
		{
			get { return m_cursor; }
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				m_cursor = value;
			}
		}
		/// <summary>Element's qualified name, or null if <see cref="Cursor"/> is null</summary>
		public string CursorName { get { return m_cursor != null ? m_cursor.Name : null; } }

		/// <summary>
		/// Initializes the <see cref="Cursor"/> to the underlying <see cref="XmlDocument"/>'s 
		/// <see cref="XmlDocument.DocumentElement"/>
		/// </summary>
		public void InitializeAtRootElement()
		{
			Cursor = m_root.DocumentElement;
		}

		/// <summary>
		/// Saves the current stream cursor in <paramref name="old_cursor"/> and sets <paramref name="new_cursor"/> to be the new cursor for the stream
		/// </summary>
		/// <param name="new_cursor">If not null, will be the new value of <see cref="Cursor"/></param>
		/// <param name="old_cursor">On return, contains the value of <see cref="Cursor"/> before the call to this method</param>
		public void SaveCursor(XmlElement new_cursor, out XmlElement old_cursor)
		{
			old_cursor = Cursor;
			if(new_cursor != null) Cursor = new_cursor;
		}
		/// <summary>Returns the cursor to a previously saved cursor value</summary>
		/// <param name="old_cursor">Previously saved cursor. Set to null before the method returns</param>
		public void RestoreCursor(ref XmlElement old_cursor)
		{
			Contract.Ensures(Contract.ValueAtReturn(out old_cursor) == null);
			Contract.Assert(old_cursor != null, "Can't restore a cursor that wasn't saved!");

			Cursor = old_cursor;
			old_cursor = null;
		}

		/// <summary>Enter a new xml element <b>(for reading)</b></summary>
		/// <param name="new_cursor"></param>
		/// <returns></returns>
		public /*IDisposable*/XmlElementStreamReadBookmark EnterCursorBookmark(XmlElement new_cursor)
		{
			Contract.Requires<ArgumentNullException>(new_cursor != null);

			return new XmlElementStreamReadBookmark(this, new_cursor);
		}
		/// <summary>Enter a new xml element <b>(for writing)</b></summary>
		/// <param name="element_name"></param>
		/// <returns></returns>
		public /*IDisposable*/XmlElementStreamWriteBookmark EnterCursorBookmark(string element_name)
		{
			Contract.Requires<ArgumentNullException>(element_name != null);

			return new XmlElementStreamWriteBookmark(this, element_name);
		}
		/// <summary>(Optionally) enter an xml element</summary>
		/// <param name="mode">Are we reading an existing element or writing a new one?</param>
		/// <param name="element_name">If null, no bookmarking is actually performed</param>
		/// <returns></returns>
		public /*IDisposable*/XmlElementStreamBookmark EnterCursorBookmark(FA mode, string element_name)
		{
			return new XmlElementStreamBookmark(this, mode, element_name);
		}
		#endregion

		#region Permissions
		/// <summary>Supported access permissions for this stream</summary>
		public FileAccess AccessPermissions { get; private set; }

		void ValidateReadPermission()
		{
			if (!AccessPermissions.CanRead())
				throw new NotSupportedException("Stream permissions do not support reading");
		}
		void ValidateWritePermission()
		{
			if (!AccessPermissions.CanWrite())
				throw new NotSupportedException("Stream permissions do not support writing");
		}
		void ValidatePermission(FA mode)
		{
			if((AccessPermissions & mode) == 0)
				throw new NotSupportedException("Stream permissions do not support the requested streaming mode");
		}
		#endregion

		#region Constructor
		XmlElementStream()
		{
		}

		/// <summary>Initialize an element stream from the XML file <paramref name="file_info"/> with <see cref="owner"/> as the initial owner object</summary>
		/// <param name="filename">Name of the XML file we're to load</param>
		/// <param name="access">Supported access permissions for this stream</param>
		/// <param name="owner">Initial owner object</param>
		public XmlElementStream(string filename, FileAccess access = FileAccess.ReadWrite, object owner = null)
		{
			Contract.Requires<ArgumentNullException>(filename != null);

			if (!File.Exists(filename)) throw new FileNotFoundException("", filename);

			m_root = new XmlDocument();
			m_root.Load(this.StreamName = filename);

			AccessPermissions = access;

			this.Owner = owner;
		}

		/// <summary>
		/// Initialize an element stream from the XML nodes <paramref name="document"/> 
		/// and <paramref name="cursor"/> with <paramref name="owner"/> as the initial owner object
		/// </summary>
		/// <param name="document"><paramref name="cursor"/>'s owner document</param>
		/// <param name="cursor">Starting element cursor</param>
		/// <param name="access">Supported access permissions for this stream</param>
		/// <param name="owner">Initial owner object</param>
		public XmlElementStream(XmlDocument document, XmlElement cursor, FileAccess access = FileAccess.ReadWrite, object owner = null)
		{
			Contract.Requires<ArgumentNullException>(document != null);
			Contract.Requires(object.ReferenceEquals(cursor.OwnerDocument, document));

			m_root = document;
			m_cursor = cursor;

			this.StreamName = string.Format("XmlDocument:{0}", document.Name);

			AccessPermissions = access;

			this.Owner = owner;
		}

		/// <summary>Initialize a new element stream with write permissions</summary>
		/// <param name="owner">Initial owner object</param>
		/// <param name="root_name">Name of the document element</param>
		/// <returns></returns>
		public static XmlElementStream CreateForWrite(string root_name, object owner = null)
		{
			Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(root_name));

			var root = new XmlDocument();
			root.AppendChild(root.CreateElement(root_name));

			XmlElementStream @this = new XmlElementStream();
			@this.m_root = root;

			@this.AccessPermissions = FileAccess.Write;

			@this.Owner = owner;

			return @this;
		}
		#endregion

		#region Util
		/// <summary>Checks to see if the current scope has a fully defined attribute named <paramref name="name"/></summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool AttributeExists(string name)
		{
			if (Cursor == null) return false;
			XmlNode n = Cursor.Attributes[name];
			if (n != null) return true;
			
			return false;
		}

		/// <summary>Checks to see if the current scope has a fully defined element named <paramref name="name"/></summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool ElementsExists(string name)
		{
			if (Cursor == null) return false;
			XmlElement n = Cursor[name];

			return n != null && n.Value != string.Empty;
		}

		/// <summary>Checks to see if the current scope has elements</summary>
		/// <returns></returns>
		public bool ElementsExist { get { return Cursor != null && Cursor.HasChildNodes; } }
		#endregion

		public void Dispose()
		{
			m_cursor = null;

			Owner = null;
		}
	};

	/// <summary>
	/// Helper type for exposing the <see cref="XmlElementStream.StreamElementBegin(FileAccess,string)">StreamElementBegin</see> and 
	/// <see cref="XmlElementStream.StreamElementEnd()">StreamElementEnd</see> in a way which works with the C# "using" statements
	/// </summary>
	/// <remarks>If a null element name is given, skips the bookmarking process entiring</remarks>
	public struct XmlElementStreamBookmark : IDisposable
	{
		XmlElementStream m_stream;
		FA m_mode;
		XmlElement m_oldCursor;

		/// <summary>Saves the stream's cursor so a new one can be specified, but then later restored to the saved cursor, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="mode"></param>
		/// <param name="element_name">If null, no bookmarking is actually performed</param>
		public XmlElementStreamBookmark(XmlElementStream stream, FA mode, string element_name)
		{
			Contract.Requires<ArgumentNullException>(stream != null);

			m_stream = null;
			m_mode = mode;
			m_oldCursor = null;

			if(element_name != null)
				(m_stream = stream).StreamElementBegin(mode, element_name, out m_oldCursor);
		}

		/// <summary>Returns the cursor of the underlying stream to the last saved cursor value</summary>
		public void Dispose()
		{
			if (m_stream != null)
			{
				m_stream.StreamElementEnd(m_mode, ref m_oldCursor);
				m_stream = null;
			}
		}
	};

	public struct IKSoftStreamOwnerBookmark : IDisposable
	{
		XmlElementStream m_stream;
		object m_oldOwner;

		/// <summary>Saves the stream's owner so a new one can be specified, but is then later restored to the previous owner, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="new_owner"></param>
		public IKSoftStreamOwnerBookmark(XmlElementStream stream, object new_owner)
		{
			Contract.Requires<ArgumentNullException>(stream != null);

			m_oldOwner = (m_stream = stream).Owner;
			m_stream.Owner = new_owner;
		}

		/// <summary>Returns the owner of the underlying stream to the previous owner</summary>
		public void Dispose()
		{
			if (m_stream != null)
			{
				m_stream.Owner = m_oldOwner;
				m_stream = null;
			}
		}
	};
}