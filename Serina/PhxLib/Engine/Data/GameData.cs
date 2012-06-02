using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BDifficultyType
	{
		Easy,
		Normal,
		Hard,
		Legendary,
		Custom,
		Automatic,
		Default,
	};

	public class BResource : Collections.BListAutoIdObject
	{
		// fucking squads.xml and techs.xml uses a lower-case type name :|
		const bool kUseLowercaseCostTypeHack = true;

		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Resource");

		public static readonly Collections.BTypeValuesParams<float> kBListTypeValuesParams = new
			Collections.BTypeValuesParams<float>(db => db.GameData.Resources) { kTypeGetInvalid = Util.kGetInvalidSingle };
		public static readonly XML.BTypeValuesXmlParams<float> kBListTypeValuesXmlParams = new
			XML.BTypeValuesXmlParams<float>("Resource", "Type");
		public static readonly XML.BTypeValuesXmlParams<float> kBListTypeValuesXmlParams_Cost = new
			XML.BTypeValuesXmlParams<float>("Cost", "ResourceType");
		public static readonly XML.BTypeValuesXmlParams<float> kBListTypeValuesXmlParams_CostLowercaseType = !kUseLowercaseCostTypeHack ? kBListTypeValuesXmlParams_Cost : new
			XML.BTypeValuesXmlParams<float>("Cost", "ResourceType".ToLower());
		public static readonly XML.BTypeValuesXmlParams<float> kBListTypeValuesXmlParams_AddResource = new
			XML.BTypeValuesXmlParams<float>("AddResource", null, XML.BCollectionXmlParamsFlags.UseInnerTextForData);

		const string kXmlAttrDeductable = "Deductable";
		#endregion

		bool mDeductable;
		public bool Deductable { get { return mDeductable; } }

		public BResource() { }
		internal BResource(bool deductable) { mDeductable = deductable; }

		#region BListAutoIdObject Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrDeductable, ref mDeductable);
		}
		#endregion
	};

	public struct BPopulation : IO.IPhxXmlStreamable, IComparable<BPopulation>, IEqualityComparer<BPopulation>
	{
		class _EqualityComparer : IEqualityComparer<BPopulation>
		{
			#region IEqualityComparer<BPopulation> Members
			public bool Equals(BPopulation x, BPopulation y)
			{
				return x.Max == y.Max && x.Count == y.Count;
			}

			public int GetHashCode(BPopulation obj)
			{
				return obj.Max.GetHashCode() ^ obj.Count.GetHashCode();
			}
			#endregion
		};
		public static readonly IEqualityComparer<BPopulation> kEqualityComparer = new _EqualityComparer();

		#region Xml constants
		public static readonly Collections.BTypeValuesParams<BPopulation> kBListParams = new
			Collections.BTypeValuesParams<BPopulation>(db => db.GameData.Populations)
			{
				kTypeGetInvalid = () => BPopulation.kInvalid
			};
		public static readonly XML.BTypeValuesXmlParams<BPopulation> kBListXmlParams = new
			XML.BTypeValuesXmlParams<BPopulation>("Pop", "Type");

		public static readonly Collections.BTypeValuesParams<float> kBListParamsSingle = new
			Collections.BTypeValuesParams<float>(db => db.GameData.Populations)
			{
				kTypeGetInvalid = Util.kGetInvalidSingle
			};
		public static readonly XML.BTypeValuesXmlParams<float> kBListXmlParamsSingle = new
			XML.BTypeValuesXmlParams<float>("Pop", "Type");
		public static readonly XML.BTypeValuesXmlParams<float> kBListXmlParamsSingle_LowerCase = new
			XML.BTypeValuesXmlParams<float>("Pop", "Type".ToLower());
		public static readonly XML.BTypeValuesXmlParams<float> kBListXmlParamsSingle_CapAddition = new
			XML.BTypeValuesXmlParams<float>("PopCapAddition", "Type");

		const string kXmlAttrMax = "Max";
		#endregion

		static readonly BPopulation kInvalid = new BPopulation(Util.kInvalidSingle, Util.kInvalidSingle);

		float mMax;
		public float Max { get { return mMax; } }

		float mCount;
		public float Count { get { return mCount; } }

		BPopulation(float max, float count) { mMax = max; mCount = count; }

		#region IXmlElementStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrMax, ref mMax);
			s.StreamCursor(mode, ref mCount);
		}
		#endregion

		#region IComparable<T> Members
		int IComparable<BPopulation>.CompareTo(BPopulation other)
		{
			if (this.Max == other.Max)
				return this.Count.CompareTo(other.Count);
			else
				return this.Max.CompareTo(other.Max);
		}
		#endregion

		#region IEqualityComparer<BPopulation> Members
		public bool Equals(BPopulation x, BPopulation y)
		{
			return kEqualityComparer.Equals(x, y);
		}

		public int GetHashCode(BPopulation obj)
		{
			return kEqualityComparer.GetHashCode(obj);
		}
		#endregion
	};

	public class BInfectionMap : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			RootName = "InfectionMap",
			ElementName = "InfectionMapEntry",
		};

		const string kXmlAttrBase = "base";
		const string kXmlAttrInfected = "infected";
		const string kXmlAttrInfectedSquad = "infectedSquad";
		#endregion

		int mBaseObjectID = Util.kInvalidInt32;
		int mInfectedObjectID = Util.kInvalidInt32;
		int mInfectedSquadID = Util.kInvalidInt32;

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			xs.StreamXmlForDBID(s, mode, kXmlAttrBase, ref mBaseObjectID, DatabaseObjectKind.Object, false, XML.Util.kSourceAttr);
			xs.StreamXmlForDBID(s, mode, kXmlAttrInfected, ref mInfectedObjectID, DatabaseObjectKind.Object, false, XML.Util.kSourceAttr);
			if(xs.Database.Engine.Build != PhxEngineBuild.Alpha)
				xs.StreamXmlForDBID(s, mode, kXmlAttrInfectedSquad, ref mInfectedSquadID, DatabaseObjectKind.Squad, false, XML.Util.kSourceAttr);
		}
		#endregion
	};

	public class BGameData : IO.IPhxXmlStreamable
	{
		#region Xml constants
		const string kXmlRoot = "GameData";

		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "GameData.xml",
			RootName = kXmlRoot
		};

		public static readonly Collections.BTypeValuesParams<float> kRatesBListTypeValuesParams = new
			Collections.BTypeValuesParams<float>(db => db.GameData.Rates) { kTypeGetInvalid = Util.kGetInvalidSingle };
		static readonly XML.BListXmlParams kRatesXmlParams = new XML.BListXmlParams("Rate");
		public static readonly XML.BTypeValuesXmlParams<float> kRatesBListTypeValuesXmlParams = new
			XML.BTypeValuesXmlParams<float>("Rate", "Rate"); // oiy, really? name the 'type' attribute with the same name as the element?

		const string kXmlElementDifficultyEasy = "DifficultyEasy";					// 59A0
		const string kXmlElementDifficultyNormal = "DifficultyNormal";				// 59A4
		const string kXmlElementDifficultyHard = "DifficultyHard";					// 59A8
		const string kXmlElementDifficultyLegendary = "DifficultyLegendary";		// 59AC
		const string kXmlElementDifficultyDefault = "DifficultyDefault";			// 59B0
		const string kXmlElementDifficultySPCAIDefault = "DifficultySPCAIDefault";	// 59B4
		static readonly XML.BListXmlParams kPopsXmlParams = new XML.BListXmlParams("Pop");			// 28D8
		static readonly XML.BListXmlParams kRefCountsXmlParams = new XML.BListXmlParams("RefCount");// 30F8
		static readonly XML.BListXmlParams kHUDItemsXmlParams = new XML.BListXmlParams("HUDItem");
		static readonly XML.BListXmlParams kFlashableItemsXmlParams = new XML.BListXmlParams
		{
			RootName = "FlashableItems",
			ElementName = "Item",
			Flags = XML.BCollectionXmlParamsFlags.UseInnerTextForData,
		};
		static readonly XML.BListXmlParams kUnitFlagsXmlParams = new XML.BListXmlParams("UnitFlag");
		static readonly XML.BListXmlParams kSquadFlagsXmlParams = new XML.BListXmlParams("SquadFlag");

		static readonly Collections.BTypeValuesParams<string> kCodeProtoObjectsParams = new Collections.BTypeValuesParams<string>(db => db.GameProtoObjectTypes);
		static readonly XML.BTypeValuesXmlParams<string> kCodeProtoObjectsXmlParams = new XML.BTypeValuesXmlParams<string>("CodeProtoObject", "Type",
			XML.BCollectionXmlParamsFlags.ToLowerDataNames)
		{
			RootName = "CodeProtoObjects",
		};
		static readonly Collections.BTypeValuesParams<string> kCodeObjectTypesParams = new Collections.BTypeValuesParams<string>(db => db.GameObjectTypes);
		static readonly XML.BTypeValuesXmlParams<string> kCodeObjectTypesXmlParams = new XML.BTypeValuesXmlParams<string>("CodeObjectType", "Type")
		{
			RootName = "CodeObjectTypes",
		};

		const string kXmlElementGarrisonDamageMultiplier = "GarrisonDamageMultiplier";
		const string kXmlElementConstructionDamageMultiplier = "ConstructionDamageMultiplier";
		//
		const string kXmlElementShieldRegenDelay = "ShieldRegenDelay";
		const string kXmlElementShieldRegenTime = "ShieldRegenTime";
		//
		const string kXmlElementAttackRatingMultiplier = "AttackRatingMultiplier";
		const string kXmlElementDefenseRatingMultiplier = "DefenseRatingMultiplier";
		const string kXmlElementGoodAgainstMinAttackRating = "GoodAgainstMinAttackRating";
		//
		const string kXmlElementChanceToRocket = "ChanceToRocket";
		//
		const string kXmlElementTributeAmount = "TributeAmount";
		const string kXmlElementTributeCost = "TributeCost";
		//
		const string kXmlElementDamageReceivedXPFactor = "DamageReceivedXPFactor";
		//
		const string kXmlElementHeroDownedLOS = "HeroDownedLOS";// 59B8
		const string kXmlElementHeroHPRegenTime = "HeroHPRegenTime";// 59BC
		const string kXmlElementHeroRevivalDistance = "HeroRevivalDistance";// 59C0
		const string kXmlElementHeroPercentHPRevivalThreshhold = "HeroPercentHPRevivalThreshhold";// 59C4
		const string kXmlElementMaxDeadHeroTransportDist = "MaxDeadHeroTransportDist";// 59C8
		const string kXmlElementTransportClearRadiusScale = "TransportClearRadiusScale";// 59CC
		const string kXmlElementTransportMaxSearchRadiusScale = "TransportMaxSearchRadiusScale";// 59D0
		const string kXmlElementTransportMaxSearchLocations = "TransportMaxSearchLocations";// 59D4
		const string kXmlElementTransportBlockTime = "TransportBlockTime";// 59D8
		const string kXmlElementTransportLoadBlockTime = "TransportLoadBlockTime";// 59DC

		static readonly XML.BListXmlParams kPlayerStatesXmlParams = new XML.BListXmlParams("PlayerState"); // 3918
		#endregion

		public Collections.BListAutoId<BResource> Resources { get; private set; }
		public Collections.BTypeNames Rates { get; private set; }
		public Collections.BTypeNames Populations { get; private set; }
		public Collections.BTypeNames RefCounts { get; private set; }
		#region Nonsense
		/// <remarks>Engine doesn't process these, but some trigger scripts use these dynamic types, so keep them on record</remarks>
		public Collections.BTypeNames HUDItems { get; private set; }
		/// <remarks>Engine doesn't process these, but some trigger scripts use these dynamic types, so keep them on record</remarks>
		public Collections.BTypeNames FlashableItems { get; private set; }
		/// <remarks>Engine doesn't process these, but some trigger scripts use these dynamic types, so keep them on record</remarks>
		public Collections.BTypeNames UnitFlags { get; private set; }
		/// <remarks>Engine doesn't process these, but some trigger scripts use these dynamic types, so keep them on record</remarks>
		public Collections.BTypeNames SquadFlags { get; private set; }
		#endregion
		public Collections.BTypeValuesString CodeProtoObjects { get; private set; }
		public Collections.BTypeValuesString CodeObjectTypes { get; private set; }

		#region Misc values
		float mGarrisonDamageMultiplier = Util.kInvalidSingle;
		public float GarrisonDamageMultiplier { get { return mGarrisonDamageMultiplier; } }
		float mConstructionDamageMultiplier = Util.kInvalidSingle;
		public float ConstructionDamageMultiplier { get { return mConstructionDamageMultiplier; } }
		//
		float mShieldRegenDelay = Util.kInvalidSingle;
		public float ShieldRegenDelay { get { return mShieldRegenDelay; } }
		float mShieldRegenTime = Util.kInvalidSingle;
		public float ShieldRegenTime { get { return mShieldRegenTime; } }
		//
		float mAttackRatingMultiplier = Util.kInvalidSingle;
		public float AttackRatingMultiplier { get { return mAttackRatingMultiplier; } }
		float mDefenseRatingMultiplier = Util.kInvalidSingle;
		public float DefenseRatingMultiplier { get { return mDefenseRatingMultiplier; } }
		float mGoodAgainstMinAttackRating = Util.kInvalidSingle;
		public float GoodAgainstMinAttackRating { get { return mGoodAgainstMinAttackRating; } }
		//
		float mChanceToRocket = Util.kInvalidSingle;
		public float ChanceToRocket { get { return mChanceToRocket; } }
		//
		float mTributeAmount = Util.kInvalidSingle;
		public float TributeAmount { get { return mTributeAmount; } }
		float mTributeCost = Util.kInvalidSingle;
		public float TributeCost { get { return mTributeCost; } }
		//
		public Collections.BListArray<BInfectionMap> InfectionMap { get; private set; }
		//
		public float mDamageReceivedXPFactor = Util.kInvalidSingle;
		public float DamageReceivedXPFactor { get { return mDamageReceivedXPFactor; } }
		#endregion

		public Collections.BTypeNames PlayerStates { get; private set; }

		/// <summary>Get how much it costs, in total, to tribute a resource to another player</summary>
		public float TotalTributeCost { get { return (mTributeAmount * mTributeCost) + mTributeAmount; } }

		public BGameData()
		{
			Resources = new Collections.BListAutoId<BResource>();
			Rates = new Collections.BTypeNames();
			Populations = new Collections.BTypeNames();
			RefCounts = new Collections.BTypeNames();
			#region Nonsense
			HUDItems = new Collections.BTypeNames();
			FlashableItems = new Collections.BTypeNames();
			UnitFlags = new Collections.BTypeNames();
			SquadFlags = new Collections.BTypeNames();
			#endregion
			CodeProtoObjects = new Collections.BTypeValuesString(kCodeProtoObjectsParams);
			CodeObjectTypes = new Collections.BTypeValuesString(kCodeObjectTypesParams);

			InfectionMap = new Collections.BListArray<BInfectionMap>();

			PlayerStates = new Collections.BTypeNames();
		}

		#region IXmlElementStreamable Members
		/// <remarks>For streaming directly from gamedata.xml</remarks>
		internal void StreamGameXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			XML.Util.Serialize(s, mode, xs, Resources, BResource.kBListXmlParams);
			XML.Util.Serialize(s, mode, xs, Rates, kRatesXmlParams);
			XML.Util.Serialize(s, mode, xs, Populations, kPopsXmlParams);
			XML.Util.Serialize(s, mode, xs, RefCounts, kRefCountsXmlParams);
			#region Nonsense
			XML.Util.Serialize(s, mode, xs, HUDItems, kHUDItemsXmlParams);
			XML.Util.Serialize(s, mode, xs, FlashableItems, kFlashableItemsXmlParams);
			XML.Util.Serialize(s, mode, xs, UnitFlags, kUnitFlagsXmlParams);
			XML.Util.Serialize(s, mode, xs, SquadFlags, kSquadFlagsXmlParams);
			#endregion
			XML.Util.Serialize(s, mode, xs, CodeProtoObjects, kCodeProtoObjectsXmlParams);
			XML.Util.Serialize(s, mode, xs, CodeObjectTypes, kCodeObjectTypesXmlParams);

			#region Misc values
			s.StreamElementOpt(mode, kXmlElementGarrisonDamageMultiplier, ref mGarrisonDamageMultiplier, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementConstructionDamageMultiplier, ref mConstructionDamageMultiplier, Util.kNotInvalidPredicateSingle);
			//
			s.StreamElementOpt(mode, kXmlElementShieldRegenDelay, ref mShieldRegenDelay, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementShieldRegenTime, ref mShieldRegenTime, Util.kNotInvalidPredicateSingle);
			//
			s.StreamElementOpt(mode, kXmlElementAttackRatingMultiplier, ref mAttackRatingMultiplier, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementDefenseRatingMultiplier, ref mDefenseRatingMultiplier, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementGoodAgainstMinAttackRating, ref mGoodAgainstMinAttackRating, Util.kNotInvalidPredicateSingle);
			//
			s.StreamElementOpt(mode, kXmlElementChanceToRocket, ref mChanceToRocket, Util.kNotInvalidPredicateSingle);
			//
			s.StreamElementOpt(mode, kXmlElementTributeAmount, ref mTributeAmount, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementTributeCost, ref mTributeCost, Util.kNotInvalidPredicateSingle);
			//
			XML.Util.Serialize(s, mode, xs, InfectionMap, BInfectionMap.kBListXmlParams);
			//
			s.StreamElementOpt(mode, kXmlElementDamageReceivedXPFactor, ref mDamageReceivedXPFactor, Util.kNotInvalidPredicateSingle);
			#endregion

			XML.Util.Serialize(s, mode, xs, PlayerStates, kPlayerStatesXmlParams);
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			using (s.EnterCursorBookmark(mode, kXmlRoot))
				StreamGameXml(s, mode, xs);
		}
		#endregion
	};
}