using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BCollectibleSkullEffectType
	{
		Invalid,

		Score,
		GruntTank,
		GruntConfetti,
		Physics,
		ScarabBeam,
		MinimapDisable,
		Weakness,
		HitpointMod,
		DamageMod,
		Veterancy,
		AbilityRecharge,
		DeathExplode,
		TrainMod,
		SupplyMod,
		PowerRecharge,
		UnitModWarthog,
		UnitModWraith,
	};
	public enum BCollectibleSkullTarget
	{
		None,

		PlayerUnits,
		NonPlayerUnits,
		OwnerOnly,
	};
	public enum BCollectibleSkullFlags
	{
		// 0x3C
		// 0
		OnFromBeginning, // 1
		// 2
		Hidden, // 3
		// 4
		SelfActive, // 5
		// 6
		Active, // 7
	};
	public class BCollectibleSkullEffect : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			ElementName = "Effect",
		};

		const string kXmlAttrValue = "value";
		const string kXmlAttrTarget = "target";
		#endregion

		BCollectibleSkullEffectType mType = BCollectibleSkullEffectType.Invalid;
		BCollectibleSkullTarget mTarget = BCollectibleSkullTarget.None;
		float mValue = Util.kInvalidSingle;

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamCursor(mode, ref mType);
			s.StreamAttributeOpt(mode, kXmlAttrTarget, ref mTarget, e => e != BCollectibleSkullTarget.None);
			s.StreamAttributeOpt(mode, kXmlAttrValue, ref mValue, Util.kNotInvalidPredicateSingle);
		}
		#endregion
	};
	public class BCollectibleSkull : DatabaseNamedObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			ElementName = "Skull",
			DataName = DatabaseIdObject.kXmlAttrName,
		};

		const string kXmlAttrObjectDBID = "objectdbid";
		const string kXmlElementDescriptionID = "DescriptionID";

		const string kXmlElementHidden = "Hidden";
		#endregion

		int mObjectDBID = Util.kInvalidInt32;
		public Collections.BListArray<BCollectibleSkullEffect> Effects { get; private set; }
		int mDescriptionID = Util.kInvalidInt32;
		bool mHidden;

		public BCollectibleSkull()
		{
			Effects = new Collections.BListArray<BCollectibleSkullEffect>();
		}

		#region IPhxXmlStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrObjectDBID, ref mObjectDBID);
			XML.Util.Serialize(s, mode, xs, Effects, BCollectibleSkullEffect.kBListXmlParams);
			xs.StreamXmlForStringID(s, mode, kXmlElementDescriptionID, ref mDescriptionID);
			XML.Util.StreamElementNamedFlag(s, mode, kXmlElementHidden, ref mHidden);
		}
		#endregion
	};
	public class BCollectiblesSkullManager : IO.IPhxXmlStreamable
	{
		public Collections.BListAutoId<BCollectibleSkull> Skulls { get; private set; }

		// bool RocketAllGrunts, MinimapHidden
		// int BonusSquadLevels, DeathExplodeObjectType, DeathExplodeProtoObject
		// float DeathExplodeChance

		public BCollectiblesSkullManager()
		{
			Skulls = new Collections.BListAutoId<BCollectibleSkull>();
		}

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			XML.Util.Serialize(s, mode, xs, Skulls, BCollectibleSkull.kBListXmlParams);
		}
		#endregion
	};

	public class BCollectiblesManager : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public const string kXmlRootName = "CollectiblesDefinitions";

		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "Skulls.xml",
			RootName = kXmlRootName
		};

		const string kXmlElementXMLVersion = "CollectiblesXMLVersion";
		#endregion

		int mXmlVersion = Util.kInvalidInt32;
		public BCollectiblesSkullManager SkullManager { get; private set; }

		public BCollectiblesManager()
		{
			SkullManager = new BCollectiblesSkullManager();
		}

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamElementOpt(mode, null, ref mXmlVersion, Util.kNotInvalidPredicate);
			SkullManager.StreamXml(s, mode, xs);
		}
		#endregion
	};
}