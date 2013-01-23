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
		int TryGetMemberId(string memberName);
		[Contracts.Pure]
		string TryGetMemberName(int memberId);

		[Contracts.Pure]
		bool IsValidMemberId(int memberId);
		[Contracts.Pure]
		bool IsValidMemberName(string memberName);

		[Contracts.Pure]
		int GetMemberId(string memberName);
		[Contracts.Pure]
		string GetMemberName(int memberId);

		/// <summary>Number of members</summary>
		[Contracts.Pure]
		int MemberCount { get; }
	};
	[Contracts.ContractClassFor(typeof(IProtoEnum))]
	abstract class IProtoEnumContract : IProtoEnum
	{
		#region IProtoEnum Members
		int IProtoEnum.TryGetMemberId(string memberName)
		{
			Contract.Ensures(Contract.Result<int>() >= -1);

			throw new NotImplementedException();
		}
		public abstract string TryGetMemberName(int memberId);
		public abstract bool IsValidMemberId(int memberId);
		public abstract bool IsValidMemberName(string memberName);
		public abstract int GetMemberId(string memberName);
		string IProtoEnum.GetMemberName(int memberId)
		{
			Contract.Requires<ArgumentOutOfRangeException>(IsValidMemberId(memberId));

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
		int TryGetMemberIdOrUndefined(string memberName);

		[Contracts.Pure]
		int GetMemberIdOrUndefined(string memberName);
		[Contracts.Pure]
		string GetMemberNameOrUndefined(int memberId);

		/// <summary>Number of members that are undefined</summary>
		[Contracts.Pure]
		int MemberUndefinedCount { get; }

		IEnumerable<string> UndefinedMembers { get; }
	};
	[Contracts.ContractClassFor(typeof(IProtoEnumWithUndefined))]
	abstract class IProtoEnumWithUndefinedContract : IProtoEnumWithUndefined
	{
		#region IProtoEnum Members
		public abstract int TryGetMemberId(string memberName);
		public abstract string TryGetMemberName(int memberId);
		public abstract bool IsValidMemberId(int memberId);
		public abstract bool IsValidMemberName(string memberName);
		public abstract int GetMemberId(string memberName);
		public abstract string GetMemberName(int memberId);
		public abstract int MemberCount { get; }
		#endregion

		#region IProtoEnumWithUndefined
		public abstract int TryGetMemberIdOrUndefined(string memberName);
		public abstract int GetMemberIdOrUndefined(string memberName);
		public abstract string GetMemberNameOrUndefined(int member_id);

		int IProtoEnumWithUndefined.MemberUndefinedCount { get {
			Contract.Ensures(Contract.Result<int>() >= 0);

			throw new NotImplementedException();
		} }
		IEnumerable<string> IProtoEnumWithUndefined.UndefinedMembers { get {
			Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

			throw new NotImplementedException();
		}
		}
		#endregion
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
		public int TryGetMemberId(string memberName)		{ return mRoot.TryGetMemberId(memberName); }
		public string TryGetMemberName(int memberId)		{ return mRoot.TryGetMemberName(memberId); }
		public bool IsValidMemberId(int memberId)			{ return mRoot.IsValidMemberId(memberId); }
		public bool IsValidMemberName(string memberName)	{ return mRoot.IsValidMemberName(memberName); }
		public int GetMemberId(string memberName)			{ return mRoot.GetMemberId(memberName); }
		public string GetMemberName(int memberId)			{ return mRoot.GetMemberName(memberId); }
		public int MemberCount						{ get	{ return mRoot.MemberCount; } }
		#endregion

		#region IProtoEnumWithUndefined Members
		public int TryGetMemberIdOrUndefined(string memberName)
		{
			int id = TryGetMemberId(memberName);

			if (id == -1 && MemberUndefinedCount != 0)
			{
				id = mUndefined.FindIndex(str => Util.StrEqualsIgnoreCase(str, memberName));
				if (id != -1)
					id = Util.GetUndefinedReferenceHandle(id);
			}

			return id;
		}

		public int GetMemberIdOrUndefined(string memberName)
		{
			int id = TryGetMemberIdOrUndefined(memberName);

			if (id == -1)
			{
				InitializeUndefined();

				id = mUndefined.Count;
				mUndefined.Add(memberName);
				id = Util.GetUndefinedReferenceHandle(id);
			}

			return id;
		}

		public string GetMemberNameOrUndefined(int memberId)
		{
			string name;
			
			if(Util.IsUndefinedReferenceHandle(memberId))
				name = mUndefined[Util.GetUndefinedReferenceDataIndex(memberId)];
			else
				name = GetMemberName(memberId);

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
		public int TryGetMemberId(string memberName)
		{
			return Array.FindIndex(kNames, n => Util.StrEqualsIgnoreCase(n, memberName));
		}
		public string TryGetMemberName(int memberId)
		{
			return IsValidMemberId(memberId) ? GetMemberName(memberId) : null;
		}
		public bool IsValidMemberId(int memberId)
		{
			return memberId >= 0 && memberId < kNames.Length;
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
			return kNames[memberId];
		}

		public int MemberCount { get { return kNames.Length; } }
		#endregion
	};
}