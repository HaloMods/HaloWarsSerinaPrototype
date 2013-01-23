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

		public abstract void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs);
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
			UndefinedInterface = new ProtoEnumWithUndefinedImpl(this);
		}

		#region Database interfaces
		/// <remarks>Mainly a hack for adding new items dynamically</remarks>
		void PreAdd(T item, string itemName, int id = Util.kInvalidInt32)
		{
			item.AutoID = id != Util.kInvalidInt32 ? id : Count;
			if (itemName != null) item.Data = itemName;
		}
		internal int DynamicAdd(T item, string itemName, int id = Util.kInvalidInt32)
		{
			PreAdd(item, itemName, id);
			if (mDBI != null) mDBI.Add(item.Data, item);
			base.AddItem(item);

			return item.AutoID;
		}

		Dictionary<string, T> mDBI;
		internal void SetupDatabaseInterface(out Dictionary<string, T> dbi)
		{
			mDBI = dbi = new Dictionary<string, T>(Params != null ? Params.InitialCapacity : BCollectionParams.kDefaultCapacity);
		}
		#endregion

		#region IProtoEnum Members
		public int TryGetMemberId(string memberName)
		{
			return mList.FindIndex(n => Util.StrEqualsIgnoreCase(n.Data, memberName));
		}
		public string TryGetMemberName(int memberId)
		{
			return IsValidMemberId(memberId) ? GetMemberName(memberId) : null;
		}
		public bool IsValidMemberId(int memberId)
		{
			return memberId >= 0 && memberId < Count;
		}
		public bool IsValidMemberName(string memberName)
		{
			int index = TryGetMemberId(memberName);

			return index != -1;
		}

		public int GetMemberId(string memberName)
		{
			int index = TryGetMemberId(memberName);

			if (index == -1)
				throw new ArgumentException(kUnregisteredMessage, memberName);

			return index;
		}
		public string GetMemberName(int memberId)
		{
			return this[memberId].Data;
		}

		public int MemberCount { get { return Count; } }
		#endregion

		internal IProtoEnumWithUndefined UndefinedInterface { get; private set; }

		internal void Sort(Comparison<T> comparison)
		{
			mList.Sort(comparison);
		}
	};
}