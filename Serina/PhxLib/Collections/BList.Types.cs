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

		public BTypeNames(BListParams @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			kUnregisteredMessage = string.Format("Unregistered {0}!", Params.ElementName);
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

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			string name = null;
			Params.StreamDataName(s, FA.Read, ref name);

			Add(name);
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, string name)
		{
			Params.StreamDataName(s, FA.Write, ref name);
		}
		#endregion
	};
	public class BTypeNamesWithCode : BTypeNames
	{
		IProtoEnum mCodeTypes;

		public BTypeNamesWithCode(BListParams @params, IProtoEnum CodeTypes) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
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

		/// <summary>Sets ElementName and DataName (which defaults to XML attribute usage)</summary>
		/// <param name="element_name"></param>
		/// <param name="type_name">Name of the xml node which represents the type (enum) value</param>
		/// <param name="proto_enum_getter"></param>
		/// <param name="flags"></param>
		public BTypeValuesParams(string element_name, string type_name, 
			Func<Engine.BDatabaseBase, IProtoEnum> proto_enum_getter, Collections.BCollectionParamsFlags flags = 0)
		{
			ElementName = element_name;
			DataName = type_name;
			kGetProtoEnumFromDB = proto_enum_getter;
			Flags = flags;
		}
	};

	public abstract class BTypeValuesBase<T> : BListExplicitIndexBase<T>
	{
		protected BTypeValuesParams<T> TypeValuesParams { get { return Params as BTypeValuesParams<T>; } }

		protected BTypeValuesBase(BTypeValuesParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override int ReadExplicitIndex(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			string name = null;
			Params.StreamDataName(s, FA.Read, ref name);

			int index = TypeValuesParams.kGetProtoEnumFromDB(db).GetMemberId(name);

			return index;
		}
		protected override void WriteExplicitIndex(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int index)
		{
			string name = TypeValuesParams.kGetProtoEnumFromDB(db).GetMemberName(index);

			Params.StreamDataName(s, FA.Write, ref name);
		}

		/// <summary>Not Implemented</summary>
		/// <param name="s"></param>
		/// <exception cref="NotImplementedException" />
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration) { throw new NotImplementedException(); }
		/// <summary>Not Implemented</summary>
		/// <param name="s"></param>
		/// <param name="data"></param>
		/// <exception cref="NotImplementedException" />
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, T data) { throw new NotImplementedException(); }
		#endregion
	};
	public class BTypeValues<T> : BTypeValuesBase<T>
		where T : IEqualityComparer<T>, IO.IPhxXmlStreamable, new()
	{
		public BTypeValues(BTypeValuesParams<T> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int index = ReadExplicitIndex(s, db);

			InitializeItem(index);
			T data = new T();
			data.StreamXml(s, FA.Read, db);
			this[index] = data;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, T data)
		{
			data.StreamXml(s, FA.Write, db);
		}
		#endregion
	};

	public class BTypeValuesInt32 : BTypeValuesBase<int>
	{
		public BTypeValuesInt32(BTypeValuesParams<int> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int index = ReadExplicitIndex(s, db);

			InitializeItem(index);
			int value = 0;
			s.ReadCursor(KSoft.NumeralBase.Decimal, ref value);
			this[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int data)
		{
			s.WriteCursor(KSoft.NumeralBase.Decimal, data);
		}
		#endregion
	};
	public class BTypeValuesSingle : BTypeValuesBase<float>
	{
		public BTypeValuesSingle(BTypeValuesParams<float> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int index = ReadExplicitIndex(s, db);

			InitializeItem(index);
			float value = 0;
			s.ReadCursor(ref value);
			this[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, float data)
		{
			s.WriteCursor(data);
		}
		#endregion
	};
	public class BTypeValuesString : BTypeValuesBase<string>
	{
		public BTypeValuesString(BTypeValuesParams<string> @params) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int index = ReadExplicitIndex(s, db);

			InitializeItem(index);
			string value = null;
			s.ReadCursor(ref value);
			this[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, string data)
		{
			s.WriteCursor(data);
		}
		#endregion
	};

	/// <summary>
	/// Lame hack for type value maps which store their type name in the InnerText and the value in a fucking attribute
	/// </summary>
	public class BTypeValuesSingleAttrHack : BTypeValuesSingle
	{
		readonly string kAttrName;

		public BTypeValuesSingleAttrHack(BTypeValuesParams<float> @params, string attr_name) : base(@params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			kAttrName = attr_name;
		}

		#region IXmlElementStreamable Members
		protected override void ReadXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, int iteration)
		{
			int index = ReadExplicitIndex(s, db);

			InitializeItem(index);
			float value = 0;
			s.ReadAttribute(kAttrName, ref value);
			this[index] = value;
		}
		protected override void WriteXml(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db, float data)
		{
			s.WriteAttribute(kAttrName, data);
		}
		#endregion
	};
}