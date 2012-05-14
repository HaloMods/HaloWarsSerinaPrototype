using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	partial class Util
	{
		public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase db,
			Collections.BBitSet bits, BBitSetXmlParams @params)
		{
			Contract.Requires(s != null);
			Contract.Requires(db != null);
			Contract.Requires(bits != null);
			Contract.Requires(@params != null);

			var xs = new BBitSetXmlSerializer(@params, bits);
			{
				xs.StreamXml(s, mode, db);
			}
		}
	};

	public class BBitSetXmlParams : BListXmlParams
	{
		/// <summary>Sets ElementName, defaults to InnerText data usage and data interning</summary>
		/// <param name="element_name">Name of the xml element which represents the type (enum) value</param>
		public BBitSetXmlParams(string element_name)
		{
			ElementName = element_name;

			Flags = 0;
			Flags |= BCollectionXmlParamsFlags.UseInnerTextForData;
			Flags |= BCollectionXmlParamsFlags.InternDataNames;
		}

		public static readonly BBitSetXmlParams kFlagsSansRoot = new BBitSetXmlParams("Flag");
	};

	internal class BBitSetXmlSerializer
	{
		public BListXmlParams Params { get; private set; }
		public Collections.BBitSet Bits { get; private set; }

		public BBitSetXmlSerializer(BListXmlParams @params, Collections.BBitSet bits)
		{
			Contract.Requires<ArgumentNullException>(@params != null);
			Contract.Requires<ArgumentNullException>(bits != null);

			Params = @params;
			Bits = bits;
		}

		#region IXmlElementStreamable Members
		Collections.IProtoEnum GetProtoEnum(Engine.BDatabaseBase db)
		{
			if (Bits.Params.kGetProtoEnum != null)
				return Bits.Params.kGetProtoEnum();

			return Bits.Params.kGetProtoEnumFromDB(db);
		}

		void ReadXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			Collections.IProtoEnum penum = Bits.InitializeFromEnum(xs.Database);

			foreach (XmlNode n in s.Cursor.ChildNodes)
			{
				if (n.Name != Params.ElementName) continue;

				using (s.EnterCursorBookmark(n as XmlElement))
				{
					string name = null;
					Params.StreamDataName(s, FA.Read, ref name);

					int id = penum.GetMemberId(name);
					Bits[id] = true;
				}
			}

			Bits.OptimizeStorage();
		}
		void WriteXmlNodes(KSoft.IO.XmlElementStream s, BDatabaseXmlSerializerBase xs)
		{
			if (Bits.EnabledCount == 0) return;

			Collections.IProtoEnum penum = GetProtoEnum(xs.Database);

			for (int x = 0; x < Bits.Count; x++)
				if (Bits[x])
					using (s.EnterCursorBookmark(Params.ElementName))
					{
						string name = penum.GetMemberName(x);
						Params.StreamDataName(s, FA.Write, ref name);
					}
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseXmlSerializerBase xs)
		{
			using (s.EnterCursorBookmark(mode, Params.GetOptionalRootName()))
			{
					 if (mode == FA.Read)	ReadXmlNodes(s, xs);
				else if (mode == FA.Write)	WriteXmlNodes(s, xs);
			}
		}
		#endregion
	};
}