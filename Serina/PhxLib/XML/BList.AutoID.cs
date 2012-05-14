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
		public static void Serialize<T>(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase db,
			Collections.BListAutoId<T> list, BListXmlParams @params, bool force_no_root_element_streaming = false)
			where T : class, Collections.IListAutoIdObject, new()
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			if (force_no_root_element_streaming) @params.SetForceNoRootElementStreaming(true);
			var xs = new BListAutoIdXmlSerializer<T>(@params, list);
			{
				xs.StreamXml(s, mode, db);
			}
			if (force_no_root_element_streaming) @params.SetForceNoRootElementStreaming(false);
		}

		public static IBListAutoIdXmlSerializer CreateXmlSerializer<T>(Collections.BListAutoId<T> list, BListXmlParams @params)
			where T : class, Collections.IListAutoIdObject, new()
		{
			Contract.Requires(list != null);
			Contract.Requires(@params != null);

			var xs = new BListAutoIdXmlSerializer<T>(@params, list);

			return xs;
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

	public interface IBListAutoIdXmlSerializer : IDisposable, IO.IPhxXmlStreamable
	{
		BListXmlParams Params { get; }

		bool IsDisposed { get; }

		void StreamXmlPreload(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs);
		void StreamXmlUpdate(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs);
	};

	internal sealed class BListAutoIdXmlSerializer<T> : BListXmlSerializerBase<T>,
		IBListAutoIdXmlSerializer
		where T : class, Collections.IListAutoIdObject, new()
	{
		BListXmlParams mParams;
		Collections.BListAutoId<T> mList;

		public override BListXmlParams Params { get { return mParams; } }
		public override Collections.BListBase<T> List { get { return mList; } }

		public BListAutoIdXmlSerializer(BListXmlParams @params, Collections.BListAutoId<T> list)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(list != null);

			mParams = @params;
			mList = list;
		}

		#region IDisposable Members
		public bool IsDisposed { get { return mList == null; } }

		public void Dispose()
		{
			mParams = null;
			mList = null;
		}
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
			Contract.Requires(Params.RequiresDataNamePreloading);

			PreloadXml(s, xs);
		}
		public void StreamXmlUpdate(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs)
		{
			Contract.Requires(Params.SupportsUpdating);

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