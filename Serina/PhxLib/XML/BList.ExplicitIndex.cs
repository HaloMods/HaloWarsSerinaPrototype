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
		internal class _BListExplicitIndex<T>
			where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
		{
			internal static System.Threading.ThreadLocal<BListExplicitIndexXmlSerializer<T>> sXmlSerializer =
				new System.Threading.ThreadLocal<BListExplicitIndexXmlSerializer<T>>(BListExplicitIndexXmlSerializer<T>.kNewFactory);
		};
#endif
	};

	partial class Util
	{
		public static void Serialize<T>(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BListExplicitIndex<T> list, BListExplicitIndexXmlParams<T> @params)
			where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			using(var xs =
#if NO_TLS_STREAMING
				new BListExplicitIndexXmlSerializer<T>(@params, list)
#else
				BDatabaseXmlSerializerBase._BListExplicitIndex<T>.sXmlSerializer.Value.Reset(@params, list)
#endif
			)
			{
				xs.StreamXml(s, mode, db);
			}
		}
	};

	public class BListExplicitIndexXmlParams<T> : BListXmlParams
	{
		/// <summary>The index base offset as it appears in the XML</summary>
		/// <example>If this is 1, then the XML values are 1, 2, 3, etc.</example>
		/// <remarks>In-memory, everything is always at base-0</remarks>
		public int IndexBase = 1;

		public BListExplicitIndexXmlParams() { }
		/// <summary>Sets ElementName and sets DataName (defaults to attribute usage)</summary>
		/// <param name="element_name"></param>
		/// <param name="index_name"></param>
		public BListExplicitIndexXmlParams(string element_name, string index_name) : base(element_name)
		{
			RootName = null;
			DataName = index_name;
			Flags = 0;
		}

		public void StreamExplicitIndex(KSoft.IO.XmlElementStream s, FA mode, ref int index)
		{
			// 'rebase' the index to how the XML defs expect it
			if (mode == FA.Write) index += IndexBase;

			BCollectionXmlParams.StreamValue(s, mode, DataName, ref index, 
				UseInnerTextForData, UseElementForData);

			// Undo any rebasing
			/*if (mode == FA.Read)*/ index -= IndexBase;
		}
	};

	internal abstract class BListExplicitIndexXmlSerializerBase<T> : BListXmlSerializerBase<T>
	{
		BListExplicitIndexXmlParams<T> mParams;

		public abstract Collections.BListExplicitIndexBase<T> ListExplicitIndex { get; }

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<T> List { get { return ListExplicitIndex; } }

#if NO_TLS_STREAMING
		protected BListExplicitIndexXmlSerializerBase(BListExplicitIndexXmlParams<T> @params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			mParams = @params;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		protected BListExplicitIndexXmlSerializerBase() { }

		protected void Reset(BListExplicitIndexXmlParams<T> @params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			mParams = @params;
		}

		protected override void FinishTlsStreaming()
		{
			mParams = null;
		}
#endif
		#endregion

		#region IXmlElementStreamable Members
		protected virtual int ReadExplicitIndex(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			int index = -1;
			mParams.StreamExplicitIndex(s, FA.Read, ref index);

			return index;
		}
		protected virtual void WriteExplicitIndex(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int index)
		{
			mParams.StreamExplicitIndex(s, FA.Write, ref index);
		}

		protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			var eip = ListExplicitIndex.ExplicitIndexParams;
			T k_invalid = eip.kTypeGetInvalid();

			int index = 0;
			foreach (T data in ListExplicitIndex)
			{
				if (eip.kComparer.Compare(data, k_invalid) != 0)
				{
					using (s.EnterCursorBookmark(Params.ElementName))
					{
						WriteExplicitIndex(s, xs, index);
						WriteXml(s, xs, data);
					}
				}

				index++;
			}
		}
		#endregion
	};

	internal class BListExplicitIndexXmlSerializer<T> : BListExplicitIndexXmlSerializerBase<T>
		where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
	{
		Collections.BListExplicitIndex<T> mList;

		public override Collections.BListExplicitIndexBase<T> ListExplicitIndex { get { return mList; } }

#if NO_TLS_STREAMING
		public BListExplicitIndexXmlSerializer(BListExplicitIndexXmlParams<T> @params, Collections.BListExplicitIndex<T> list) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BListExplicitIndexXmlSerializer<T>> kNewFactory = () => new BListExplicitIndexXmlSerializer<T>();
		BListExplicitIndexXmlSerializer() { }

		public BListExplicitIndexXmlSerializer<T> Reset(BListExplicitIndexXmlParams<T> @params, Collections.BListExplicitIndex<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params);
			mList = list;

			return this;
		}

		protected override void FinishTlsStreaming()
		{
			base.FinishTlsStreaming();
			mList = null;
		}
#endif
		#endregion

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int index = ReadExplicitIndex(s, xs);

			mList.InitializeItem(index);
			T data = new T();
			data.StreamXml(s, FA.Read, xs);
			mList[index] = data;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, T data)
		{
			data.StreamXml(s, FA.Write, xs);
		}
		#endregion
	};
}