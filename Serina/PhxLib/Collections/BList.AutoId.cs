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

		public abstract void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs);
		#endregion

		public override string ToString() { return mName; }
	};

	public sealed class BListAutoId<T> : BListBase<T>, IProtoEnum
		// For now, I don't see a reason to support struct types in AutoIds
		// If structs are needed, the streaming logic will need to be adjusted
		where T : class, IListAutoIdObject, new()
	{
		readonly string kUnregisteredMessage;

		static string BuildUnRegisteredMsg()
		{
			return string.Format("Unregistered {0}!", typeof(T).Name);
		}
		public BListAutoId()
		{
			kUnregisteredMessage = BuildUnRegisteredMsg();
		}

		#region Database interfaces
		/// <remarks>Mainly a hack for adding new items dynamically</remarks>
		void PreAdd(T item, string item_name, int id = Util.kInvalidInt32)
		{
			item.AutoID = id != Util.kInvalidInt32 ? id : Count;
			if (item_name != null) item.Data = item_name;
		}
		internal int DynamicAdd(T item, string item_name, int id = Util.kInvalidInt32)
		{
			PreAdd(item, item_name, id);
			if (m_dbi != null) m_dbi.Add(item.Data, item);
			base.Add(item);

			return item.AutoID;
		}

		Dictionary<string, T> m_dbi;
		internal void SetupDatabaseInterface(out Dictionary<string, T> dbi)
		{
			m_dbi = dbi = new Dictionary<string, T>(Params != null ? Params.InitialCapacity : BCollectionParams.kDefaultCapacity);
		}
		#endregion

		#region IProtoEnum Members
		internal int GetMemberIndexByName(string member_name)
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
	};
}