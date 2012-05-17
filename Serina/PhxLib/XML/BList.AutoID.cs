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
		internal class _BListAutoId<T>
			where T : class, Collections.IListAutoIdObject, new()
		{
			internal static System.Threading.ThreadLocal<BListAutoIdXmlSerializer<T>> sXmlSerializer =
				new System.Threading.ThreadLocal<BListAutoIdXmlSerializer<T>>(BListAutoIdXmlSerializer<T>.kNewFactory);
		};
#endif
	};

	partial class Util
	{
		public static IBListAutoIdXmlSerializer CreateXmlSerializer<T>(Collections.BListAutoId<T> list, BListXmlParams @params)
			where T : class, Collections.IListAutoIdObject, new()
		{
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = 
#if NO_TLS_STREAMING
				new BListAutoIdXmlSerializer<T>(@params, list);
#else
				BDatabaseXmlSerializerBase._BListAutoId<T>.sXmlSerializer.Value.Reset(@params, list);
#endif

			return xs;
		}

		public static void Serialize<T>(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase db,
			Collections.BListAutoId<T> list, BListXmlParams @params, bool force_no_root_element_streaming = false)
			where T : class, Collections.IListAutoIdObject, new()
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			if (force_no_root_element_streaming) @params.SetForceNoRootElementStreaming(true);
			using(var xs = CreateXmlSerializer(list, @params))
			{
				xs.StreamXml(s, mode, db);
			}
			if (force_no_root_element_streaming) @params.SetForceNoRootElementStreaming(false);
		}

		public static void SerializePreload(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase db,
			IBListAutoIdXmlSerializer xs, bool force_no_root_element_streaming = false)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(xs != null);
			Contract.Requires(!xs.IsDisposed);

			if (force_no_root_element_streaming) xs.Params.SetForceNoRootElementStreaming(true);
			xs.StreamXmlPreload(s, db);
			if (force_no_root_element_streaming) xs.Params.SetForceNoRootElementStreaming(false);
		}
		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase db,
			IBListAutoIdXmlSerializer xs, bool force_no_root_element_streaming = false)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(xs != null);
			Contract.Requires(!xs.IsDisposed);

			if (force_no_root_element_streaming) xs.Params.SetForceNoRootElementStreaming(true);
			xs.StreamXml(s, mode, db);
			if (force_no_root_element_streaming) xs.Params.SetForceNoRootElementStreaming(false);
		}
		public static void SerializeUpdate(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase db,
			IBListAutoIdXmlSerializer xs, bool force_no_root_element_streaming = false)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(xs != null);
			Contract.Requires(!xs.IsDisposed);

			if (force_no_root_element_streaming) xs.Params.SetForceNoRootElementStreaming(true);
			xs.StreamXmlUpdate(s, db);
			if (force_no_root_element_streaming) xs.Params.SetForceNoRootElementStreaming(false);
		}
	};

	[Contracts.ContractClass(typeof(IBListAutoIdXmlSerializerContract))]
	public interface IBListAutoIdXmlSerializer : IDisposable, IO.IPhxXmlStreamable
	{
		BListXmlParams Params { get; }

		bool IsDisposed { get; }

		void StreamXmlPreload(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs);
		void StreamXmlUpdate(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs);
	};
	[Contracts.ContractClassFor(typeof(IBListAutoIdXmlSerializer))]
	abstract class IBListAutoIdXmlSerializerContract : IBListAutoIdXmlSerializer
	{
		#region IBListAutoIdXmlSerializer Members
		public abstract BListXmlParams Params { get; }
		public abstract bool IsDisposed { get; }

		public void StreamXmlPreload(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			Contract.Requires(Params.RequiresDataNamePreloading);

			throw new NotImplementedException();
		}

		public void StreamXmlUpdate(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			Contract.Requires(Params.SupportsUpdating);

			throw new NotImplementedException();
		}
		#endregion

		#region IDisposable Members
		public abstract void Dispose();
		#endregion

		#region IPhxXmlStreamable Members
		public abstract void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs);
		#endregion
	};

	internal sealed class BListAutoIdXmlSerializer<T> : BListXmlSerializerBase<T>,
		IBListAutoIdXmlSerializer
		where T : class, Collections.IListAutoIdObject, new()
	{
		BListXmlParams mParams;
		Collections.BListAutoId<T> mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<T> List { get { return mList; } }

#if NO_TLS_STREAMING
		public BListAutoIdXmlSerializer(BListXmlParams @params, Collections.BListAutoId<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}
#endif

		#region TLS & re-use interface
#if !NO_TLS_STREAMING
		public static readonly Func<BListAutoIdXmlSerializer<T>> kNewFactory = () => new BListAutoIdXmlSerializer<T>();
		BListAutoIdXmlSerializer() { }

		public BListAutoIdXmlSerializer<T> Reset(BListXmlParams @params, Collections.BListAutoId<T> list)
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

		bool mIsPreloaded;
		bool RequiresDataNamePreloading { get { return Params.RequiresDataNamePreloading; } }

		int mCountBeforeUpdate;
		bool mIsUpdating;

		#region Database interfaces
		bool SetupItem(out T item, string item_name, int iteration)
		{
			bool stream_item = !RequiresDataNamePreloading ||(RequiresDataNamePreloading && mIsPreloaded);

			if (mIsUpdating)
			{
				// The update system in HW is fucked...just because the "update" attribute is true or left out, doesn't mean the value existed before or is not a new value
				// So just try
				int idx = mList.GetMemberIndexByName(item_name);
				if (idx != -1)
				{
					item = mList[idx];
					return stream_item;
				}

				iteration += mCountBeforeUpdate;
			}

			if (RequiresDataNamePreloading && mIsPreloaded)
			{
				item = mList[iteration];
				return stream_item;
			}

			mList.DynamicAdd(item = new T(), item_name, iteration);

			return stream_item;
		}
		#endregion

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs, int iteration)
		{
			const FA k_mode = FA.Read;

			string item_name = null;
			Params.StreamDataName(s, k_mode, ref item_name);

			T item;
			if(SetupItem(out item, item_name, iteration))
				item.StreamXml(s, k_mode, xs);
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs, T data)
		{
			const FA k_mode = FA.Write;

			string item_name = data.Data;
			if (item_name != null) Params.StreamDataName(s, k_mode, ref item_name);

			data.StreamXml(s, k_mode, xs);
		}

		void PreloadXml(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs)
		{
			mIsPreloaded = false;

			StreamXml(s, FA.Read, xs);

			mIsPreloaded = true;
		}
		public void StreamXmlPreload(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs)
		{
			PreloadXml(s, xs);
		}
		public void StreamXmlUpdate(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs)
		{
			mIsUpdating = true;
			mCountBeforeUpdate = mList.Count;

			if (RequiresDataNamePreloading)
				PreloadXml(s, xs);
			StreamXml(s, FA.Read, xs);

			mIsUpdating = false;
			//mCountBeforeUpdate = 0;
		}
		#endregion
	};
}