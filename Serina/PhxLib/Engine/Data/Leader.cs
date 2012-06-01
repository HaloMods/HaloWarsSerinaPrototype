using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public class BLeader : DatabaseNamedObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Leader")
		{
			DataName = "Name",
			Flags = 0
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "Leaders.xml",
			RootName = kBListXmlParams.RootName
		};

		const string kXmlElementTech = "Tech";
		const string kXmlElementCiv = "Civ";
		const string kXmlElementPower = "Power";
		const string kXmlElementNameID = "NameID";

		// TODO: HW's FlashPortrait elements have an ending " character (sans a starting quote). Becareful!

		const string kXmlElementSupportPower = "SupportPower";
		const string kXmlElementSupportPowerAttrTechPrereq = "TechPrereq"; // proto tech
		// Can have multiple powers...
		const string kXmlElementSupportPowerElementPower = "Power";

		const string kXmlElementStartingUnit = "StartingUnit";
		const string kXmlElementStartingUnitAttrBuildOther = "BuildOther";
		const string kXmlElementStartingSquad = "StartingSquad";
		static readonly XML.BListOfIDsXmlParams kStartingSquadBListXmlParams =
			new XML.BListOfIDsXmlParams(kXmlElementStartingSquad, XML.BDatabaseXmlSerializerBase.StreamSquadID);
		#endregion

		int mTechID = Util.kInvalidInt32;
		public int TechID { get { return mTechID; } }
		int mCivID = Util.kInvalidInt32;
		public int CivID { get { return mCivID; } }
		int mPowerID = Util.kInvalidInt32;
		public int PowerID { get { return mPowerID; } }

		int mNameID = Util.kInvalidInt32;
		public int NameID { get { return mNameID; } }

		/// <summary>Initial resources</summary>
		public Collections.BTypeValuesSingle Resources { get; private set; }

		int mStartingUnitID = Util.kInvalidInt32;
		public int StartingUnitID { get { return mStartingUnitID; } }
		int mStartingUnitBuildOtherID = Util.kInvalidInt32;
		public int StartingUnitBuildOtherID { get { return mStartingUnitBuildOtherID; } }
		public Collections.BListOfIDs StartingSquads { get; private set; }

		public Collections.BTypeValues<BPopulation> Populations { get; private set; }

		// Empty Leaders just have a Civ
		public bool IsEmpty { get { return mTechID == Util.kInvalidInt32; } }

		public BLeader()
		{
			Resources = new Collections.BTypeValuesSingle(BResource.kBListTypeValuesParams);

			StartingSquads = new Collections.BListOfIDs();

			Populations = new Collections.BTypeValues<BPopulation>(BPopulation.kBListParams);
		}

		#region IXmlElementStreamable Members
		bool ShouldStreamStartingUnit(KSoft.IO.XmlElementStream s, FA mode)
		{
			return (mode == FA.Write && mStartingUnitID != Util.kInvalidInt32) || s.ElementsExists(kXmlElementStartingUnit);
		}
		void StreamXmlStartingUnit(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			using (s.EnterCursorBookmark(mode, kXmlElementStartingUnit))
			{
				xs.StreamXmlForDBID(s, mode, null, ref mStartingUnitID, DatabaseObjectKind.Object, false, XML.Util.kSourceCursor);
				xs.StreamXmlForDBID(s, mode, kXmlElementStartingUnitAttrBuildOther, ref mStartingUnitBuildOtherID, DatabaseObjectKind.Object, false, XML.Util.kSourceAttr);
			}
		}
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			xs.StreamXmlForDBID(s, mode, kXmlElementTech, ref mTechID, DatabaseObjectKind.Tech);
			xs.StreamXmlForDBID(s, mode, kXmlElementCiv, ref mCivID, DatabaseObjectKind.Civ);
			xs.StreamXmlForDBID(s, mode, kXmlElementPower, ref mPowerID, DatabaseObjectKind.Power);
			xs.StreamXmlForStringID(s, mode, kXmlElementNameID, ref mNameID);

			XML.Util.Serialize(s, mode, xs, Resources, BResource.kBListTypeValuesXmlParams);
			if(ShouldStreamStartingUnit(s, mode)) StreamXmlStartingUnit(s, mode, xs);
			XML.Util.Serialize(s, mode, xs, StartingSquads, kStartingSquadBListXmlParams);
			XML.Util.Serialize(s, mode, xs, Populations, BPopulation.kBListXmlParams);
		}
		#endregion
	};
}