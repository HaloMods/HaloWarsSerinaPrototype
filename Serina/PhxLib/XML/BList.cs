using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	partial class BDatabaseXmlSerializerBase
	{
#if !NO_TLS_STREAMING
		internal class _BListArray<T>
			where T : IO.IPhxXmlStreamable, new()
		{
			internal static System.Threading.ThreadLocal<BListArrayXmlSerializer<T>> sXmlSerializer =
				new System.Threading.ThreadLocal<BListArrayXmlSerializer<T>>(BListArrayXmlSerializer<T>.kNewFactory);
		};
#endif
	};

	partial class Util
	{
		public static void Serialize<T>(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xsi,
			Collections.BListArray<T> list, BListXmlParams @params)
			where T : IO.IPhxXmlStreamable, new()
		{
			Contract.Requires(s != null);
			Contract.Requires(xsi != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			using(var xs = 
#if NO_TLS_STREAMING
				new BListArrayXmlSerializer<T>(@params, list)
#else
				BDatabaseXmlSerializerBase._BListArray<T>.sXmlSerializer.Value.Reset(@params, list)
#endif
			)
			{
				xs.StreamXml(s, mode, xsi);
			}
		}
	};

	public class BListXmlParams : BCollectionXmlParams
	{
		public /*readonly*/ string DataName;

		#region Flags
		[Contracts.Pure]
		public bool InternDataNames { get { return HasFlag(BCollectionXmlParamsFlags.InternDataNames); } }
		[Contracts.Pure]
		public bool UseInnerTextForData { get { return HasFlag(BCollectionXmlParamsFlags.UseInnerTextForData); } }
		[Contracts.Pure]
		public bool UseElementForData { get { return HasFlag(BCollectionXmlParamsFlags.UseElementForData); } }
		[Contracts.Pure]
		public bool ToLowerDataNames { get { return HasFlag(BCollectionXmlParamsFlags.ToLowerDataNames); } }
		[Contracts.Pure]
		public bool RequiresDataNamePreloading { get { return HasFlag(BCollectionXmlParamsFlags.RequiresDataNamePreloading); } }
		[Contracts.Pure]
		public bool SupportsUpdating { get { return HasFlag(BCollectionXmlParamsFlags.SupportsUpdating); } }
		#endregion

		public BListXmlParams() { }
		/// <summary>Sets RootName to plural of ElementName and sets UseInnerTextForData</summary>
		/// <param name="element_name"></param>
		/// <param name="additional_flags"></param>
		public BListXmlParams(string element_name, BCollectionXmlParamsFlags additional_flags = 0) : base(element_name)
		{
			Flags = additional_flags;
			Flags |= BCollectionXmlParamsFlags.UseInnerTextForData;
		}

		public void StreamDataName(KSoft.IO.XmlElementStream s, FA mode, ref string name)
		{
			BCollectionXmlParams.StreamValue(s, mode, DataName, ref name,
				UseInnerTextForData, UseElementForData, InternDataNames,
				ToLowerDataNames);
		}
	};

	internal abstract class BListXmlSerializerBase<T> : IDisposable, IO.IPhxXmlStreamable
	{
		public abstract BListXmlParams Params { get; }
		public abstract Collections.BListBase<T> List { get; }

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		protected abstract void FinishTlsStreaming();
#endif
		#endregion

		#region IXmlElementStreamable Members
		protected abstract void ReadXml(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs, int iteration);
		protected abstract void WriteXml(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs, T data);

		protected virtual void ReadXmlDetermineListSize(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs)
		{
			int xml_node_count = s.Cursor.ChildNodes.Count;
			if (List.Capacity < xml_node_count)
				List.Capacity = xml_node_count;
		}
		protected virtual bool ReadXmlShouldSkipNode(XmlNode n)
		{
			if (Params.UseElementName)
				return n.Name != Params.ElementName;

			return false;
		}
		protected virtual void ReadXmlNodes(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs)
		{
			ReadXmlDetermineListSize(s, xs);

			int x = 0;
			foreach (XmlNode n in s.Cursor.ChildNodes)
			{
				if (ReadXmlShouldSkipNode(n)) continue;

				using (s.EnterCursorBookmark(n as XmlElement))
					ReadXml(s, xs, x++);
			}

			List.OptimizeStorage();
		}
		protected virtual string WriteXmlGetElementName(T data)
		{
			return Params.ElementName;
		}
		protected virtual void WriteXmlNodes(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs)
		{
			foreach (T data in List)
				using (s.EnterCursorBookmark(WriteXmlGetElementName(data)))
					WriteXml(s, xs, data);
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs)
		{
			bool should_stream = true;
			string root_name = Params.GetOptionalRootName();

			if (mode == FA.Read) // If the stream doesn't have the expected element, don't try to stream
				should_stream = root_name == null || s.ElementsExists(root_name);

			if (should_stream) using (s.EnterCursorBookmark(mode, root_name))
			{
					 if (mode == FA.Read)	ReadXmlNodes(s, xs);
				else if (mode == FA.Write)	WriteXmlNodes(s, xs);
			}
		}
		#endregion

		#region IDisposable Members
		public bool IsDisposed { get { return List == null; } }

		public virtual void Dispose()
		{
#if !NO_TLS_STREAMING
			FinishTlsStreaming();
#endif
		}
		#endregion
	};

	internal class BListArrayXmlSerializer<T> : BListXmlSerializerBase<T>
		where T : IO.IPhxXmlStreamable, new()
	{
		BListXmlParams mParams;
		Collections.BListArray<T> mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<T> List { get { return mList; } }

#if NO_TLS_STREAMING
		public BListArrayXmlSerializer(BListXmlParams @params, Collections.BListArray<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BListArrayXmlSerializer<T>> kNewFactory = () => new BListArrayXmlSerializer<T>();
		BListArrayXmlSerializer() { }

		public BListArrayXmlSerializer<T> Reset(BListXmlParams @params, Collections.BListArray<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;

			return this;
		}

		protected override void FinishTlsStreaming()
		{
			mParams = null;
			mList = null;
		}
#endif
		#endregion

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs, int iteration)
		{
			T item = new T();
			item.StreamXml(s, FA.Read, xs);

			List.Add(item);
		}

		protected override void WriteXml(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs, T data)
		{
			data.StreamXml(s, FA.Write, xs);
		}
		#endregion
	};
}