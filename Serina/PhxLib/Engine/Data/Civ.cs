﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public class BCiv : DatabaseNamedObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Civ")
		{
			DataName = "Name",
			Flags = XML.BCollectionXmlParamsFlags.UseElementForData
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "Civs.xml",
			RootName = kBListXmlParams.RootName
		};

		const string kXmlElementTech = "CivTech";

		const string kXmlElementPowerFromHero = "PowerFromHero";
		#endregion

		int mTechID = Util.kInvalidInt32;
		public int TechID { get { return mTechID; } }

		bool mPowerFromHero;
		public bool PowerFromHero { get { return mPowerFromHero; } }

		// Empty Civs just have a name
		public bool IsEmpty { get { return mTechID != Util.kInvalidInt32; } }

		public BCiv()
		{
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs)
		{
			base.StreamXml(s, mode, xs);

			xs.StreamXmlForDBID(s, mode, kXmlElementTech, ref mTechID, DatabaseObjectKind.Tech);
			s.StreamElementOpt(mode, kXmlElementPowerFromHero, ref mPowerFromHero, Util.kNotFalsePredicate);
		}
		#endregion
	};
}