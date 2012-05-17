using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;
using XmlIgnore = System.Xml.Serialization.XmlIgnoreAttribute;

namespace PhxLib.Engine
{
	[Obsolete("Only for documentation purposes, don't use")]
	public enum BProtoSquadDataType
	{
		Invalid = BObjectDataType.Invalid,

		Enable = BObjectDataType.Enable,
		BuildPoints = BObjectDataType.BuildPoints,
		Cost = BObjectDataType.Cost,
		Level = BObjectDataType.Level,
	};

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
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Unit")
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
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs)
		{
			s.StreamAttribute(mode, kXmlAttrCount, KSoft.NumeralBase.Decimal, ref mCount);
			xs.StreamXmlForDBID(s, mode, null, ref mUnitID, DatabaseObjectKind.Object, false, XML.Util.kSourceCursor);
		}
		#endregion
	};
	public class BProtoSquad : DatabaseIdObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Squad")
		{
			DataName = DatabaseNamedObject.kXmlAttrName,
			Flags = XML.BCollectionXmlParamsFlags.ToLowerDataNames |
				XML.BCollectionXmlParamsFlags.RequiresDataNamePreloading |
				XML.BCollectionXmlParamsFlags.SupportsUpdating
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Game,
			Directory = GameDirectory.Data,
			FileName = "Squads.xml",
			RootName = kBListXmlParams.RootName
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfoUpdate = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Update,
			Directory = GameDirectory.Data,
			FileName = "Squads_Update.xml",
			RootName = kBListXmlParams.RootName
		};

		static readonly Collections.CodeEnum<BProtoSquadFlags> kFlagsProtoEnum = new Collections.CodeEnum<BProtoSquadFlags>();
		static readonly Collections.BBitSetParams kFlagsParams = new Collections.BBitSetParams(() => kFlagsProtoEnum);

		const string kXmlElementCanAttackWhileMoving = "CanAttackWhileMoving";
		#endregion

		public Collections.BListArray<BProtoSquadUnit> Units { get; private set; }

		public Collections.BBitSet Flags { get; private set; }

		/// <summary>Is this Squad just made up of a single Unit?</summary>
		public bool SquadIsUnit { get {
			return Units.Count == 1 && Units[0].Count == 1;
		}}

		public BProtoSquad() : base(BResource.kBListTypeValuesParams, BResource.kBListTypeValuesXmlParams_CostLowercaseType)
		{
			Units = new Collections.BListArray<BProtoSquadUnit>();

			Flags = new Collections.BBitSet(kFlagsParams);
		}

		#region IXmlElementStreamable Members
		bool ShouldStreamUnits(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) return s.ElementsExists(BProtoSquadUnit.kBListXmlParams.RootName);
			else if (mode == FA.Write) return !Units.IsEmpty;

			return false;
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs)
		{
			base.StreamXml(s, mode, xs);

			if (ShouldStreamUnits(s, mode)) XML.Util.Serialize(s, mode, xs, Units, BProtoSquadUnit.kBListXmlParams);

			XML.Util.Serialize(s, mode, xs, Flags, XML.BBitSetXmlParams.kFlagsSansRoot);
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