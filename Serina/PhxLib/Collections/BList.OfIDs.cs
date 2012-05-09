using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BListOfIDsParams<TContext> : BListParams
		where TContext : class
	{
		public delegate void StreamDelegate(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db, 
			BListOfIDsParams<TContext> @params, TContext ctxt, ref int id);
		public delegate TContext GetContextDelegate(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db);

		/// <summary>Method for streaming an ID based on a cursor's value</summary>
		public readonly StreamDelegate kStreamID;
		public readonly GetContextDelegate kGetContext;

		public BListOfIDsParams(string element_name, StreamDelegate stream_id, GetContextDelegate get_ctxt = null) : base(element_name)
		{
			Contract.Requires<ArgumentNullException>(stream_id != null);

			RootName = null;
			Flags = 0;

			kStreamID = stream_id;
			kGetContext = get_ctxt;
		}
	};
	public class BListOfIDsParams : BListOfIDsParams<object>
	{
		public BListOfIDsParams(string element_name, StreamDelegate stream_id, GetContextDelegate get_ctxt = null)
			: base(element_name, stream_id, get_ctxt)
		{
			Contract.Requires<ArgumentNullException>(stream_id != null);
		}
	};

	public class BListOfIDs<TContext> : BListBase<int>
		where TContext : class
	{
		BListOfIDsParams<TContext> IDsParams { get { return Params as BListOfIDsParams<TContext>; } }

		public BListOfIDs(BListOfIDsParams<TContext> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		TContext mStreamCtxt;

		void SetupContext(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db)
		{
			if (IDsParams.kGetContext == null) return;

			mStreamCtxt = IDsParams.kGetContext(s, mode, db);
		}
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int id = Util.kInvalidInt32;

			IDsParams.kStreamID(s, FA.Read, db, IDsParams, mStreamCtxt, ref id);

			Add(id);
		}

		protected override void ReadXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			SetupContext(s, FA.Read, db);

			base.ReadXmlNodes(s, db);

			mStreamCtxt = null;
		}

		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int id)
		{
			IDsParams.kStreamID(s, FA.Write, db, IDsParams, mStreamCtxt, ref id);
		}

		protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			SetupContext(s, FA.Write, db);

			base.WriteXmlNodes(s, db);

			mStreamCtxt = null;
		}
		#endregion
	};
	public class BListOfIDs : BListOfIDs<object>
	{
		public BListOfIDs(BListOfIDsParams @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};
}