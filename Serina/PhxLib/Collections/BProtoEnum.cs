using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

namespace PhxLib.Collections
{
	#region IProtoEnum
	[Contracts.ContractClass(typeof(IProtoEnumContract))]
	public interface IProtoEnum
	{
		[Contracts.Pure]
		int TryGetMemberId(string member_name);
		[Contracts.Pure]
		string TryGetMemberName(int member_id);

		[Contracts.Pure]
		bool IsValidMemberId(int member_id);
		[Contracts.Pure]
		bool IsValidMemberName(string member_name);

		[Contracts.Pure]
		int GetMemberId(string member_name);
		[Contracts.Pure]
		string GetMemberName(int member_id);

		/// <summary>Number of members</summary>
		[Contracts.Pure]
		int MemberCount { get; }
	};
	[Contracts.ContractClassFor(typeof(IProtoEnum))]
	abstract class IProtoEnumContract : IProtoEnum
	{
		#region IProtoEnum Members
		int IProtoEnum.TryGetMemberId(string member_name)
		{
			Contract.Ensures(Contract.Result<int>() >= -1);

			throw new NotImplementedException();
		}
		public abstract string TryGetMemberName(int member_id);
		public abstract bool IsValidMemberId(int member_id);
		public abstract bool IsValidMemberName(string member_name);
		public abstract int GetMemberId(string member_name);
		string IProtoEnum.GetMemberName(int member_id)
		{
			Contract.Requires<ArgumentOutOfRangeException>(IsValidMemberId(member_id));

			throw new NotImplementedException();
		}

		int IProtoEnum.MemberCount { get {
			Contract.Ensures(Contract.Result<int>() >= 0);

			throw new NotImplementedException();
		} }
		#endregion
	};
	#endregion

	#region IProtoEnumWithUndefined
	[Contracts.ContractClass(typeof(IProtoEnumWithUndefinedContract))]
	public interface IProtoEnumWithUndefined : IProtoEnum
	{
		int TryGetMemberIdOrUndefined(string member_name);

		[Contracts.Pure]
		int GetMemberIdOrUndefined(string member_name);
		[Contracts.Pure]
		string GetMemberNameOrUndefined(int member_id);

		/// <summary>Number of members that are undefined</summary>
		[Contracts.Pure]
		int MemberUndefinedCount { get; }

		IEnumerable<string> UndefinedMembers { get; }
	};
	[Contracts.ContractClassFor(typeof(IProtoEnumWithUndefined))]
	abstract class IProtoEnumWithUndefinedContract : IProtoEnumWithUndefined
	{
		public abstract int TryGetMemberId(string member_name);
		public abstract string TryGetMemberName(int member_id);
		public abstract bool IsValidMemberId(int member_id);
		public abstract bool IsValidMemberName(string member_name);
		public abstract int GetMemberId(string member_name);
		public abstract string GetMemberName(int member_id);
		public abstract int MemberCount { get; }

		public abstract int TryGetMemberIdOrUndefined(string member_name);
		public abstract int GetMemberIdOrUndefined(string member_name);
		public abstract string GetMemberNameOrUndefined(int member_id);

		int IProtoEnumWithUndefined.MemberUndefinedCount { get {
			Contract.Ensures(Contract.Result<int>() >= 0);

			throw new NotImplementedException();
		} }
		IEnumerable<string> IProtoEnumWithUndefined.UndefinedMembers { get {
			Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

			throw new NotImplementedException();
		} }
	};
	#endregion

	internal class ProtoEnumWithUndefinedImpl : IProtoEnumWithUndefined
	{
		IProtoEnum mRoot;
		List<string> mUndefined;

		public ProtoEnumWithUndefinedImpl(IProtoEnum root)
		{
			Contract.Requires(root != null);

			mRoot = root;
		}

		void InitializeUndefined()
		{
			if (mUndefined == null) mUndefined = new List<string>();
		}

		#region IProtoEnum Members
		public int TryGetMemberId(string member_name)		{ return mRoot.TryGetMemberId(member_name); }
		public string TryGetMemberName(int member_id)		{ return mRoot.TryGetMemberName(member_id); }
		public bool IsValidMemberId(int member_id)			{ return mRoot.IsValidMemberId(member_id); }
		public bool IsValidMemberName(string member_name)	{ return mRoot.IsValidMemberName(member_name); }
		public int GetMemberId(string member_name)			{ return mRoot.GetMemberId(member_name); }
		public string GetMemberName(int member_id)			{ return mRoot.GetMemberName(member_id); }
		public int MemberCount						{ get	{ return mRoot.MemberCount; } }
		#endregion

		#region IProtoEnumWithUndefined Members
		public int TryGetMemberIdOrUndefined(string member_name)
		{
			int id = TryGetMemberId(member_name);

			if (id == -1 && MemberUndefinedCount != 0)
			{
				id = mUndefined.FindIndex(str => Util.StrEqualsIgnoreCase(str, member_name));
				if (id != -1)
					id = Util.GetUndefinedReferenceHandle(id);
			}

			return id;
		}

		public int GetMemberIdOrUndefined(string member_name)
		{
			int id = TryGetMemberIdOrUndefined(member_name);

			if (id == -1)
			{
				InitializeUndefined();

				id = mUndefined.Count;
				mUndefined.Add(member_name);
				id = Util.GetUndefinedReferenceHandle(id);
			}

			return id;
		}

		public string GetMemberNameOrUndefined(int member_id)
		{
			string name;
			
			if(Util.IsUndefinedReferenceHandle(member_id))
				name = mUndefined[Util.GetUndefinedReferenceDataIndex(member_id)];
			else
				name = GetMemberName(member_id);

			return name;
		}

		public int MemberUndefinedCount { get { return mUndefined != null ? mUndefined.Count : 0; } }

		public IEnumerable<string> UndefinedMembers { get { return mUndefined; } }
		#endregion
	};

	public class CodeEnum<TEnum> : IProtoEnum
		where TEnum : struct
	{
		static readonly Type kEnumType;
		static readonly string[] kNames;
		static readonly string kUnregisteredMessage;

		static CodeEnum()
		{
			kEnumType = typeof(TEnum);

			kNames = Enum.GetNames(kEnumType);

			kUnregisteredMessage = string.Format("Unregistered {0}!", kEnumType.Name);
		}

		#region IProtoEnum Members
		public int TryGetMemberId(string member_name)
		{
			return Array.FindIndex(kNames, n => Util.StrEqualsIgnoreCase(n, member_name));
		}
		public string TryGetMemberName(int member_id)
		{
			return IsValidMemberId(member_id) ? GetMemberName(member_id) : null;
		}
		public bool IsValidMemberId(int member_id)
		{
			return member_id >= 0 && member_id < kNames.Length;
		}
		public bool IsValidMemberName(string member_name)
		{
			int index = TryGetMemberId(member_name);

			return index != -1;
		}

		public int GetMemberId(string member_name)
		{
			int index = TryGetMemberId(member_name);

			if (index == -1)
				throw new ArgumentException(kUnregisteredMessage, member_name);

			return index;
		}
		public string GetMemberName(int member_id)
		{
			return kNames[member_id];
		}

		public int MemberCount { get { return kNames.Length; } }
		#endregion
	};
}