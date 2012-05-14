﻿using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	partial class Util
	{
		public static void Serialize<TContext>(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BListOfIDs<TContext> list, BListOfIDsXmlParams<TContext> @params)
			where TContext : class
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BListOfIDsXmlSerializer<TContext>(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
		}

		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BListOfIDs list, BListOfIDsXmlParams @params)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BListOfIDsXmlSerializer(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
		}
	};

	public class BListOfIDsXmlParams<TContext> : BListXmlParams
		where TContext : class
	{
		public delegate void StreamDelegate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs,
			BListOfIDsXmlParams<TContext> @params, TContext ctxt, ref int id);
		public delegate TContext GetContextDelegate(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs);

		/// <summary>Method for streaming an ID based on a cursor's value</summary>
		public readonly StreamDelegate kStreamID;
		public readonly GetContextDelegate kGetContext;

		public BListOfIDsXmlParams(string element_name, StreamDelegate stream_id, GetContextDelegate get_ctxt = null) : base(element_name)
		{
			Contract.Requires<ArgumentNullException>(stream_id != null);

			RootName = null;
			Flags = 0;

			kStreamID = stream_id;
			kGetContext = get_ctxt;
		}
	};
	public class BListOfIDsXmlParams : BListOfIDsXmlParams<object>
	{
		public BListOfIDsXmlParams(string element_name, StreamDelegate stream_id, GetContextDelegate get_ctxt = null)
			: base(element_name, stream_id, get_ctxt)
		{
			Contract.Requires<ArgumentNullException>(stream_id != null);
		}
	};

	internal class BListOfIDsXmlSerializer<TContext> : BListXmlSerializerBase<int>
		where TContext : class
	{
		protected BListOfIDsXmlParams<TContext> mParams;
		Collections.BListOfIDs<TContext> mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<int> List { get { return mList; } }

		public BListOfIDsXmlSerializer(BListOfIDsXmlParams<TContext> @params, Collections.BListOfIDs<TContext> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}

		#region IXmlElementStreamable Members
		TContext mStreamCtxt;

		void SetupContext(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			if (mParams.kGetContext == null) return;

			mStreamCtxt = mParams.kGetContext(s, mode, xs);
		}
		protected override void ReadXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int iteration)
		{
			int id = PhxLib.Util.kInvalidInt32;

			mParams.kStreamID(s, FA.Read, xs, mParams, mStreamCtxt, ref id);

			mList.Add(id);
		}

		protected override void ReadXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			SetupContext(s, FA.Read, xs);

			base.ReadXmlNodes(s, xs);

			mStreamCtxt = null;
		}

		protected override void WriteXml(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs, int id)
		{
			mParams.kStreamID(s, FA.Write, xs, mParams, mStreamCtxt, ref id);
		}

		protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			SetupContext(s, FA.Write, xs);

			base.WriteXmlNodes(s, xs);

			mStreamCtxt = null;
		}
		#endregion
	};
	internal class BListOfIDsXmlSerializer : BListOfIDsXmlSerializer<object>
	{
		public BListOfIDsXmlSerializer(BListOfIDsXmlParams @params, Collections.BListOfIDs list) : base(@params, list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);
		}
	};
}