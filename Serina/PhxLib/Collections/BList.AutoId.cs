using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public interface IListAutoIdObject : IO.IPhxXmlStreamable
	{
		/// <summary>Generated AutoID for this object</summary>
		int AutoID { get; set; }

		/// <summary>Main, ie key, data</summary>
		/// <example>If this were a Resource, then this could be "Supplies"</example>
		string Data { get; set; }
	};
	public abstract class BListAutoIdObject : IListAutoIdObject
	{
		protected string mName;
		public string Name { get { return mName; } }

		protected BListAutoIdObject()
		{
			mName = Engine.BDatabaseBase.kInvalidString;
		}

		#region IListAutoIdObject Members
		public int AutoID { get; set; }

		string IListAutoIdObject.Data
		{
			get { return mName; }
			set { mName = value; }
		}

		public abstract void StreamXml(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db);
		#endregion

		public override string ToString() { return mName; }
	};

	public sealed class BListAutoId<T> : BListBase<T>, IProtoEnum
		// For now, I don't see a reason to support struct types in AutoIds
		// If structs are needed, the streaming logic will need to be adjusted
		where T : class, IListAutoIdObject, new()
	{
		readonly string kUnregisteredMessage;

		public BListAutoId(BListParams @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			kUnregisteredMessage = string.Format("Unregistered {0}!", Params.ElementName);
		}

		bool mIsPreloaded;
		bool RequiresDataNamePreloading { get { return Params.RequiresDataNamePreloading; } }

		int mCountBeforeUpdate;
		bool mIsUpdating;

		#region Database interfaces
		/// <remarks>Mainly a hack for adding new items dynamically</remarks>
		void PreAdd(T item, string item_name, int id = Util.kInvalidInt32)
		{
			item.AutoID = id != Util.kInvalidInt32 ? id : Count;
			if (item_name != null) item.Data = item_name;
		}
		internal void DynamicAdd(T item, string item_name)
		{
			PreAdd(item, item_name);
			if (m_dbi != null) m_dbi.Add(item.Data, item);
			base.Add(item);
		}

		Dictionary<string, T> m_dbi;
		internal void SetupDatabaseInterface(out Dictionary<string, T> dbi)
		{
			m_dbi = dbi = new Dictionary<string, T>(Params.InitialCapacity);
		}
		bool SetupItem(out T item, string item_name, int iteration)
		{
			bool stream_item = !RequiresDataNamePreloading ||(RequiresDataNamePreloading && mIsPreloaded);

			if (mIsUpdating)
			{
				// The update system in HW is fucked...just because the "update" attribute is true or left out, doesn't mean the value existed before or is not a new value
				// So just try
				int idx = GetMemberIndexByName(item_name);
				if (idx != -1)
				{
					item = base[idx];
					return stream_item;
				}

				iteration += mCountBeforeUpdate;
			}

			if (RequiresDataNamePreloading && mIsPreloaded)
			{
				item = base[iteration];
				return stream_item;
			}

			item = new T();
			PreAdd(item, item_name, iteration);
			if (m_dbi != null) m_dbi.Add(item.Data, item);

			base.Add(item);
			return stream_item;
		}
		#endregion

		#region IProtoEnum Members
		int GetMemberIndexByName(string member_name)
		{
			return FindIndex(n => Util.StrEqualsIgnoreCase(n.Data, member_name));
		}
		public bool IsValidMemberId(int member_id)
		{
			return member_id >= 0 && member_id < Count;
		}
		public bool IsValidMemberName(string member_name)
		{
			int index = GetMemberIndexByName(member_name);

			return index != -1;
		}

		public int GetMemberId(string member_name)
		{
			int index = GetMemberIndexByName(member_name);

			if (index == -1)
				throw new ArgumentException(kUnregisteredMessage, member_name);

			return index;
		}
		public string GetMemberName(int member_id)
		{
			return this[member_id].Data;
		}

		public int MemberCount { get { return Count; } }
		#endregion

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			const FA k_mode = FA.Read;

			string item_name = null;
			Params.StreamDataName(s, k_mode, ref item_name);

			T item;
			if(SetupItem(out item, item_name, iteration))
				item.StreamXml(s, k_mode, db);
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, T data)
		{
			const FA k_mode = FA.Write;

			string item_name = data.Data;
			if (item_name != null) Params.StreamDataName(s, k_mode, ref item_name);

			data.StreamXml(s, k_mode, db);
		}

// 		protected override void ReadXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
// 		{
// 			base.ReadXmlNodes(s, db);
// 		}

		void PreloadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			mIsPreloaded = false;

			StreamXml(s, FA.Read, db);

			mIsPreloaded = true;
		}
		public void StreamXmlPreload(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			Contract.Requires(Params.RequiresDataNamePreloading);

			PreloadXml(s, db);
		}
		public void StreamXmlUpdate(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			Contract.Requires(Params.SupportsUpdating);

			mIsUpdating = true;
			mCountBeforeUpdate = Count;

			if (RequiresDataNamePreloading)
				PreloadXml(s, db);
			StreamXml(s, FA.Read, db);

			mIsUpdating = false;
			//mCountBeforeUpdate = 0;
		}
		#endregion
	};
}