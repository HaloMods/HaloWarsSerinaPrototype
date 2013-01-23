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
		internal class _BListOfIDs<TContext>
			where TContext : class
		{
			internal static System.Threading.ThreadLocal<BListOfIDsXmlSerializer<TContext>> sXmlSerializer =
				new System.Threading.ThreadLocal<BListOfIDsXmlSerializer<TContext>>(BListOfIDsXmlSerializer<TContext>.kNewFactory);
		};
		internal static System.Threading.ThreadLocal<BListOfIDsXmlSerializer> sBListOfIDsXmlSerializer =
			new System.Threading.ThreadLocal<BListOfIDsXmlSerializer>(BListOfIDsXmlSerializer.kNewFactory);
#endif
	};

	partial class Util
	{
		public static void Serialize<TContext>(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xsi,
			Collections.BListOfIDs<TContext> list, BListOfIDsXmlParams<TContext> @params)
			where TContext : class
		{
			Contract.Requires(s != null);
			Contract.Requires(xsi != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			using(var xs = 
#if NO_TLS_STREAMING
				new BListOfIDsXmlSerializer<TContext>(@params, list)
#else
				BDatabaseXmlSerializerBase._BListOfIDs<TContext>.sXmlSerializer.Value.Reset(@params, list)
#endif
			)
			{
				xs.StreamXml(s, mode, xsi);
			}
		}

		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xsi,
			Collections.BListOfIDs list, BListOfIDsXmlParams @params)
		{
			Contract.Requires(s != null);
			Contract.Requires(xsi != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			using(var xs = 
#if NO_TLS_STREAMING
				new BListOfIDsXmlSerializer(@params, list)
#else
				BDatabaseXmlSerializerBase.sBListOfIDsXmlSerializer.Value.Reset(@params, list)
#endif
			)
			{
				xs.StreamXml(s, mode, xsi);
			}
		}
	};

	public class BListOfIDsXmlParams<TContext> : BListXmlParams
		where TContext : class
	{
		public delegate void StreamDelegate(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs,
			BListOfIDsXmlParams<TContext> @params, TContext ctxt, ref int id);
		public delegate TContext GetContextDelegate(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs);

		/// <summary>Method for streaming an ID based on a cursor's value</summary>
		public readonly StreamDelegate kStreamID;
		public readonly GetContextDelegate kGetContext;

		public BListOfIDsXmlParams(string elementName, StreamDelegate streamId, GetContextDelegate getCtxt = null) : base(elementName)
		{
			Contract.Requires<ArgumentNullException>(streamId != null);

			RootName = null;
			Flags = 0;

			kStreamID = streamId;
			kGetContext = getCtxt;
		}
	};
	public class BListOfIDsXmlParams : BListOfIDsXmlParams<object>
	{
		public BListOfIDsXmlParams(string elementName, StreamDelegate streamId, GetContextDelegate getCtxt = null)
			: base(elementName, streamId, getCtxt)
		{
			Contract.Requires<ArgumentNullException>(streamId != null);
		}
	};

	internal class BListOfIDsXmlSerializer<TContext> : BListXmlSerializerBase<int>
		where TContext : class
	{
		protected BListOfIDsXmlParams<TContext> mParams;
		Collections.BListOfIDs<TContext> mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<int> List { get { return mList; } }

#if NO_TLS_STREAMING
		public BListOfIDsXmlSerializer(BListOfIDsXmlParams<TContext> @params, Collections.BListOfIDs<TContext> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BListOfIDsXmlSerializer<TContext>> kNewFactory = () => new BListOfIDsXmlSerializer<TContext>();
		protected BListOfIDsXmlSerializer() { }

		public BListOfIDsXmlSerializer<TContext> Reset(BListOfIDsXmlParams<TContext> @params, Collections.BListOfIDs<TContext> list)
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
		TContext mStreamCtxt;

		void SetupContext(KSoft.IO.XmlElementStream s, FA mode, BXmlSerializerInterface xs)
		{
			if (mParams.kGetContext == null) return;

			mStreamCtxt = mParams.kGetContext(s, mode, xs);
		}
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs, int iteration)
		{
			int id = PhxLib.Util.kInvalidInt32;

			mParams.kStreamID(s, FA.Read, xs, mParams, mStreamCtxt, ref id);

			mList.AddItem(id);
		}

		protected override void ReadXmlNodes(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs)
		{
			SetupContext(s, FA.Read, xs);

			base.ReadXmlNodes(s, xs);

			mStreamCtxt = null;
		}

		protected override void WriteXml(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs, int id)
		{
			mParams.kStreamID(s, FA.Write, xs, mParams, mStreamCtxt, ref id);
		}

		protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, BXmlSerializerInterface xs)
		{
			SetupContext(s, FA.Write, xs);

			base.WriteXmlNodes(s, xs);

			mStreamCtxt = null;
		}
		#endregion
	};
	internal class BListOfIDsXmlSerializer : BListOfIDsXmlSerializer<object>
	{
#if NO_TLS_STREAMING
		public BListOfIDsXmlSerializer(BListOfIDsXmlParams @params, Collections.BListOfIDs list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public new static readonly Func<BListOfIDsXmlSerializer> kNewFactory = () => new BListOfIDsXmlSerializer();
		BListOfIDsXmlSerializer() { }

		public BListOfIDsXmlSerializer Reset(BListOfIDsXmlParams @params, Collections.BListOfIDs list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			base.Reset(@params, list);

			return this;
		}
#endif
		#endregion
	};
}