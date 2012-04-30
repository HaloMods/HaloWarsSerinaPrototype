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
		public bool IsValidMemberId(int member_id)
		{
			throw new NotImplementedException();
		}
		bool IProtoEnum.IsValidMemberName(string member_name)
		{
			throw new NotImplementedException();
		}
		int IProtoEnum.GetMemberId(string member_name)
		{
			throw new NotImplementedException();
		}
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
		int GetMemberIndexByName(string member_name)
		{
			return Array.FindIndex(kNames, n => Util.StrEqualsIgnoreCase(n, member_name));
		}
		public bool IsValidMemberId(int member_id)
		{
			return member_id >= 0 && member_id < kNames.Length;
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
			return kNames[member_id];
		}

		public int MemberCount { get { return kNames.Length; } }
		#endregion
	};
}