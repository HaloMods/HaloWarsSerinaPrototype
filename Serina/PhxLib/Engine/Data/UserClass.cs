using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public class BUserClassField : Collections.BListAutoIdObject
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams()
		{
			ElementName = "Fields",
			DataName = "Name",
		};

		const string kXmlAttrType = "Type";
		#endregion

		BTriggerVarType mType;

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttribute(mode, kXmlAttrType, ref mType);
		}
		#endregion
	};
	public class BUserClass : Collections.BListAutoIdObject, IDatabaseIdObject
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("UserClass")
		{
			DataName = "Name",
			Flags = 0
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "UserClasses.xml",
			RootName = kBListParams.RootName
		};

		const string kXmlAttrDbId = "DBID";
		#endregion

		int mDbId;
		public int DbId { get { return mDbId; } }

		public Collections.BListAutoId<BUserClassField> Fields { get; private set; }

		public BUserClass()
		{
			mDbId = Util.kInvalidInt32;
			Fields = new Collections.BListAutoId<BUserClassField>(BUserClassField.kBListParams);
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttribute(mode, kXmlAttrDbId, KSoft.NumeralBase.Decimal, ref mDbId);
		}
		#endregion
	};
}