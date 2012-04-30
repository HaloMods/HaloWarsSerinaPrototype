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
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Leader")
		{
			DataName = "Name",
			Flags = 0
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "Leaders.xml",
			RootName = kBListParams.RootName
		};

		const string kXmlElementTech = "Tech";
		const string kXmlElementCiv = "Civ";
		const string kXmlElementPower = "Power";
		const string kXmlElementNameID = "NameID";

		// TODO: HW's FlashPortrait elements have an ending " character (sans a starting quote). Becareful!

		const string kXmlElementSupportPower = "SupportPower";
		const string kXmlElementSupportPowerAttrTechPrereq = "TechPrereq";
		// Can have multiple powers...
		const string kXmlElementSupportPowerElementPower = "Power";

		const string kXmlElementStartingUnit = "StartingUnit";
		const string kXmlElementStartingUnitAttrBuildOther = "BuildOther";
		const string kXmlElementStartingSquad = "StartingSquad";
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
		// TODO: HEY! Leaders can have MORE THAN ONE starting squad
		int mStartingSquadID = Util.kInvalidInt32;
		public int StartingSquadID { get { return mStartingSquadID; } }

		public Collections.BTypeValues<BPopulation> Populations { get; private set; }

		// Empty Leaders just have a Civ
		public bool IsEmpty { get { return mTechID == Util.kInvalidInt32; } }

		public BLeader()
		{
			Resources = new Collections.BTypeValuesSingle(BResource.kBListTypeValuesParams);
			Populations = new Collections.BTypeValues<BPopulation>(BPopulation.kBListParams);
		}

		#region IXmlElementStreamable Members
		bool ShouldStreamStartingUnit(KSoft.IO.XmlElementStream s, FA mode)
		{
			return (mode == FA.Write && mStartingUnitID != Util.kInvalidInt32) || s.ElementsExists(kXmlElementStartingUnit);
		}
		void StreamXmlStartingUnit(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			using (s.EnterCursorBookmark(mode, kXmlElementStartingUnit))
			{
				db.StreamXmlForDBID(s, mode, null, ref mStartingUnitID, DatabaseObjectKind.Object, false, Util.kSourceCursor);
				db.StreamXmlForDBID(s, mode, kXmlElementStartingUnitAttrBuildOther, ref mStartingUnitBuildOtherID, DatabaseObjectKind.Object, false, Util.kSourceAttr);
			}
		}
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			base.StreamXml(s, mode, db);

			db.StreamXmlForDBID(s, mode, kXmlElementTech, ref mTechID, DatabaseObjectKind.Tech);
			db.StreamXmlForDBID(s, mode, kXmlElementCiv, ref mCivID, DatabaseObjectKind.Civ);
			db.StreamXmlForDBID(s, mode, kXmlElementPower, ref mPowerID, DatabaseObjectKind.Power);
			Util.StreamStringID(s, mode, kXmlElementNameID, ref mNameID, db);

			Resources.StreamXml(s, mode, db);
			if(ShouldStreamStartingUnit(s, mode)) StreamXmlStartingUnit(s, mode, db);
			db.StreamXmlForDBID(s, mode, kXmlElementStartingSquad, ref mStartingSquadID, DatabaseObjectKind.Squad);
			Populations.StreamXml(s, mode, db);
		}
		#endregion
	};
}