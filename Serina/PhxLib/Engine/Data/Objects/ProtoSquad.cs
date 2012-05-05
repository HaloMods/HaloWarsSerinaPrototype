using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;
using XmlIgnore = System.Xml.Serialization.XmlIgnoreAttribute;

namespace PhxLib.Engine
{
	public enum BProtoSquadFormationType
	{
		Invalid,

		Flock,
		Gaggle,
		Line,
	};

	public enum BProtoSquadFlags
	{
		// 0x68
		KBAware, // 1<<2
		OneTimeSpawnUsed, // 1<<4

		// 0x144
		AlwaysAttackReviveUnits, // 1<<0
		InstantTrainWithRecharge, // 1<<1
		ForceToGaiaPlayer, // 1<<2,
		CreateAudioReactions, // 1<<3
		Chatter, // 1<<4
		Repairable, // 1<<5

		// 0x145
		Rebel, // 1<<0
		Forerunner, // 1<<1
		Flood, // 1<<2
		FlyingFlood, // 1<<3
		DiesWhenFrozen, // 1<<4
		OnlyShowBobbleHeadWhenContained, // 1<<5
		AlwaysRenderSelectionDecal, // 1<<6
		ScaredByRoar, // 1<<7

		// 0x146
		AlwaysShowHPBar, // 1<<3
		NoPlatoonMerge, // 1<<6

		[Obsolete, XmlIgnore] JoinAll,
	};

	public struct BProtoSquadUnit : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Unit")
		{
			Flags = 0
		};

		const string kXmlAttrCount = "count";
		#endregion

		int mCount;
		public int Count { get { return mCount; } }
		int mUnitID;
		public int UnitID { get { return mUnitID; } }

		#region IXmlElementStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttribute(mode, kXmlAttrCount, KSoft.NumeralBase.Decimal, ref mCount);
			db.StreamXmlForDBID(s, mode, null, ref mUnitID, DatabaseObjectKind.Object, false, Util.kSourceCursor);
		}
		#endregion
	};
	public class BProtoSquad : DatabaseIdObject
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Squad")
		{
			DataName = DatabaseNamedObject.kXmlAttrName,
			Flags = Collections.BCollectionParamsFlags.ToLowerDataNames |
				Collections.BCollectionParamsFlags.RequiresDataNamePreloading |
				Collections.BCollectionParamsFlags.SupportsUpdating
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Game,
			Directory = GameDirectory.Data,
			FileName = "Squads.xml",
			RootName = kBListParams.RootName
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfoUpdate = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Update,
			Directory = GameDirectory.Data,
			FileName = "Squads_Update.xml",
			RootName = kBListParams.RootName
		};

		static readonly Collections.CodeEnum<BProtoSquadFlags> kFlagsProtoEnum = new Collections.CodeEnum<BProtoSquadFlags>();
		static readonly Collections.BBitSetParams kFlagsParams = new Collections.BBitSetParams("Flag",
			db => kFlagsProtoEnum);

		const string kXmlElementCanAttackWhileMoving = "CanAttackWhileMoving";
		#endregion

		public Collections.BListArray<BProtoSquadUnit> Units { get; private set; }

		public Collections.BBitSet Flags { get; private set; }

		/// <summary>Is this Squad just made up of a single Unit?</summary>
		public bool SquadIsUnit { get {
			return Units.Count == 1 && Units[0].Count == 1;
		}}

		public BProtoSquad() : base(BResource.kBListTypeValuesParams_CostLowercaseType)
		{
			Units = new Collections.BListArray<BProtoSquadUnit>(BProtoSquadUnit.kBListParams);

			Flags = new Collections.BBitSet(kFlagsParams);
		}

		#region IXmlElementStreamable Members
		bool ShouldStreamUnits(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) return s.ElementsExists(BProtoSquadUnit.kBListParams.RootName);
			else if (mode == FA.Write) return Units.Count != 0;

			return false;
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			base.StreamXml(s, mode, db);

			if (ShouldStreamUnits(s, mode))
				Units.StreamXml(s, mode, db);

			Flags.StreamXml(s, mode, db);
		}
		#endregion
	};

	// 828E3B38
	public class BProtoMergedSquads
	{
		//List<int> mMergedSquads;

		int mSquadID = Util.kInvalidInt32;
		public int SquadID { get { return mSquadID; } }
	};
	// 828E3CF8
	public class BProtoShieldBubbleTypes
	{
		// Target, ShieldBubble
		//Dictionary<int, int> mSquadToShieldBubbleMap;

		int mSquadID = Util.kInvalidInt32;
		public int SquadID { get { return mSquadID; } }
	};
}