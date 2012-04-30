using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BBitSetParams : BListParams
	{
		/// <summary>Get the source IProtoEnum from a global object</summary>
		public readonly Func<IProtoEnum> kGetProtoEnum;
		/// <summary>Get the source IProtoEnum from an engine's main database</summary>
		public readonly Func<Engine.BDatabaseBase, IProtoEnum> kGetProtoEnumFromDB;

		BBitSetParams(string element_name)
		{
			ElementName = element_name;

			Flags = 0;
			Flags |= BCollectionParamsFlags.UseInnerTextForData;
			Flags |= BCollectionParamsFlags.InternDataNames;
		}
		/// <summary>Sets ElementName, defaults to InnerText data usage and data interning</summary>
		/// <param name="element_name">Name of the xml element which represents the type (enum) value</param>
		/// <param name="proto_enum_getter"></param>
		public BBitSetParams(string element_name, Func<Engine.BDatabaseBase, IProtoEnum> proto_enum_getter) : this(element_name)
		{
			kGetProtoEnumFromDB = proto_enum_getter;
		}
		/// <summary>Sets ElementName, defaults to InnerText data usage and data interning</summary>
		/// <param name="element_name">Name of the xml element which represents the type (enum) value</param>
		/// <param name="proto_enum_getter"></param>
		public BBitSetParams(string element_name, Func<IProtoEnum> proto_enum_getter) : this(element_name)
		{
			kGetProtoEnum = proto_enum_getter;
		}
	};

	public class BBitSet : IO.IPhxXmlStreamable
	{
		// TODO: implement a custom BitArray that supports fastforwarding to the first bit that is set, etc
		// In the case of Phx, most bit-sets will be sparsely populated
		// http://docs.oracle.com/javase/1.4.2/docs/api/java/util/BitSet.html
		// C:\Mount\B\SourceCode\sscli20\clr\src\bcl\system\collections\bitarray.cs
		// C:\Mount\B\SourceCode\sscli20\fx\src\compmod\system\collections\specialized\bitvector32.cs
		System.Collections.BitArray mBits;

		/// <summary>Parameters that dictate the functionality of this list</summary>
		public BBitSetParams Params { get; private set; }

		public BBitSet(BBitSetParams @params)
		{
			Contract.Requires<ArgumentNullException>(@params != null);

			Params = @params;

			if (Params.kGetProtoEnum != null)
				InitializeBits(Params.kGetProtoEnum());
		}

		void InitializeBits(IProtoEnum penum)
		{
			mBits = new System.Collections.BitArray(penum.MemberCount);
		}

		#region IXmlElementStreamable Members
		IProtoEnum GetProtoEnum(Engine.BDatabaseBase db)
		{
			if (Params.kGetProtoEnum != null)
				return Params.kGetProtoEnum();

			return Params.kGetProtoEnumFromDB(db);
		}

		void ReadXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			IProtoEnum penum;
			if (mBits != null)
				penum = Params.kGetProtoEnum();
			else
			{
				penum = Params.kGetProtoEnumFromDB(db);
				InitializeBits(penum);
			}

			foreach (XmlNode n in s.Cursor.ChildNodes)
			{
				if (n.Name != Params.ElementName) continue;

				using (s.EnterCursorBookmark(n as XmlElement))
				{
					string name = null;
					Params.StreamDataName(s, FA.Read, ref name);

					int id = penum.GetMemberId(name);
					mBits[id] = true;
				}
			}
		}
		void WriteXmlNodes(KSoft.IO.XmlElementStream s, Engine.BDatabaseBase db)
		{
			IProtoEnum penum = GetProtoEnum(db);

			for (int x = 0; x < mBits.Count; x++)
				if (mBits[x])
					using (s.EnterCursorBookmark(Params.ElementName))
					{
						string name = penum.GetMemberName(x);
						Params.StreamDataName(s, FA.Write, ref name);
					}
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, Engine.BDatabaseBase db)
		{
			using (s.EnterCursorBookmark(mode, Params.GetOptionalRootName()))
			{
					 if (mode == FA.Read)	ReadXmlNodes(s, db);
				else if (mode == FA.Write)	WriteXmlNodes(s, db);
			}
		}
		#endregion
	};
}