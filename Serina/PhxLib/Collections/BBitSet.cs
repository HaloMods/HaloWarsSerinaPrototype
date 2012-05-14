using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BBitSetParams
	{
		/// <summary>Get the source IProtoEnum from a global object</summary>
		public readonly Func<IProtoEnum> kGetProtoEnum;
		/// <summary>Get the source IProtoEnum from an engine's main database</summary>
		public readonly Func<Engine.BDatabaseBase, IProtoEnum> kGetProtoEnumFromDB;

		/// <summary></summary>
		/// <param name="proto_enum_getter"></param>
		public BBitSetParams(Func<Engine.BDatabaseBase, IProtoEnum> proto_enum_getter)
		{
			kGetProtoEnumFromDB = proto_enum_getter;
		}
		/// <summary></summary>
		/// <param name="proto_enum_getter"></param>
		public BBitSetParams(Func<IProtoEnum> proto_enum_getter)
		{
			kGetProtoEnum = proto_enum_getter;
		}
	};

	public class BBitSet
	{
		// TODO: implement a custom BitArray that supports fastforwarding to the first bit that is set, etc
		// In the case of Phx, most bit-sets will be sparsely populated
		// http://docs.oracle.com/javase/1.4.2/docs/api/java/util/BitSet.html
		// C:\Mount\B\SourceCode\sscli20\clr\src\bcl\system\collections\bitarray.cs
		// C:\Mount\B\SourceCode\sscli20\fx\src\compmod\system\collections\specialized\bitvector32.cs
		System.Collections.BitArray mBits;

		public int Count { get { return IsEmpty ? 0 : mBits.Count; } }
		public int EnabledCount { get; private set; }

		/// <summary>Parameters that dictate the functionality of this list</summary>
		public BBitSetParams Params { get; private set; }

		public BBitSet(BBitSetParams @params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			EnabledCount = 0;
			Params = @params;

			InitializeFromEnum(null);
		}

		public bool IsEmpty { get { return mBits == null; } }
		internal void OptimizeStorage()
		{
			if (EnabledCount == 0)
				mBits = null;
		}

		internal IProtoEnum InitializeFromEnum(Engine.BDatabaseBase db)
		{
			IProtoEnum penum = null;

			if (Params.kGetProtoEnum != null)	penum = Params.kGetProtoEnum();
			else if(db != null)					penum = Params.kGetProtoEnumFromDB(db);

			if(penum != null)
				mBits = new System.Collections.BitArray(penum.MemberCount);

			return penum;
		}

		public bool this[int bit_index]
		{
			get { return IsEmpty ? false : mBits[bit_index]; }
			set
			{
				if (IsEmpty) return;

				bool original = mBits[bit_index];
				if (original != value)
				{
					mBits[bit_index] = value;

					if (value == false) EnabledCount--;
					else EnabledCount++;
				}
			}
		}
	};
}