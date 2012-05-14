using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BTypeNames : BListBase<string>, IProtoEnum
	{
		readonly string kUnregisteredMessage;

		static string BuildUnRegisteredMsg()
		{
			return string.Format("Unregistered {0}!", "BTypeName");
		}
		public BTypeNames()
		{
			kUnregisteredMessage = BuildUnRegisteredMsg();
		}

		#region IProtoEnum Members
		protected virtual int GetMemberIndexByName(string member_name)
		{
			return FindIndex(n => Util.StrEqualsIgnoreCase(n, member_name));
		}
		public bool IsValidMemberId(int member_id)
		{
			return member_id >= 0 && member_id < MemberCount;
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
		public virtual string GetMemberName(int member_id)
		{
			return this[member_id];
		}

		public virtual int MemberCount { get { return Count; } }
		#endregion
	};
	public class BTypeNamesWithCode : BTypeNames
	{
		IProtoEnum mCodeTypes;

		public BTypeNamesWithCode(IProtoEnum CodeTypes)
		{
			Contract.Requires<ArgumentNullException>(CodeTypes != null);

			mCodeTypes = CodeTypes;
		}

		#region IProtoEnum Members
		protected override int GetMemberIndexByName(string member_name)
		{
			int idx = base.GetMemberIndexByName(member_name);

			if (idx == Util.kInvalidInt32)
			{
				idx = mCodeTypes.GetMemberId(member_name);
				if (idx != Util.kInvalidInt32) idx += Count;
			}

			return idx;
		}

		public override string GetMemberName(int member_id)
		{
			if (member_id < Count)
				return base.GetMemberName(member_id);

			member_id -= Count;
			return mCodeTypes.GetMemberName(member_id);
		}

		public override int MemberCount { get { return Count + mCodeTypes.MemberCount; } }
		#endregion
	};

	public class BTypeValuesParams<T> : BListExplicitIndexParams<T>
	{
		/// <summary>Get the source IProtoEnum from an engine's main database</summary>
		public readonly Func<Engine.BDatabaseBase, IProtoEnum> kGetProtoEnumFromDB;

		/// <summary></summary>
		/// <param name="proto_enum_getter"></param>
		/// <param name="flags"></param>
		public BTypeValuesParams(Func<Engine.BDatabaseBase, IProtoEnum> proto_enum_getter, Collections.BCollectionParamsFlags flags = 0)
		{
			kGetProtoEnumFromDB = proto_enum_getter;
			Flags = flags;
		}
	};

	public abstract class BTypeValuesBase<T> : BListExplicitIndexBase<T>
	{
		internal BTypeValuesParams<T> TypeValuesParams { get { return Params as BTypeValuesParams<T>; } }

		protected BTypeValuesBase(BTypeValuesParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};
	public class BTypeValues<T> : BTypeValuesBase<T>
		where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
	{
		public BTypeValues(BTypeValuesParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};

	public class BTypeValuesInt32 : BTypeValuesBase<int>
	{
		public BTypeValuesInt32(BTypeValuesParams<int> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};
	public class BTypeValuesSingle : BTypeValuesBase<float>
	{
		public BTypeValuesSingle(BTypeValuesParams<float> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};
	public class BTypeValuesString : BTypeValuesBase<string>
	{
		public BTypeValuesString(BTypeValuesParams<string> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}
	};
}