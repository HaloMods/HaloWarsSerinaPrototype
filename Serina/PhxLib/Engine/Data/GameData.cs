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
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Resource");

		public static readonly Collections.BTypeValuesParams<float> kBListTypeValuesParams = new
			Collections.BTypeValuesParams<float>("Resource", "Type", db => db.GameData.Resources) { kTypeGetInvalid = Util.kGetInvalidSingle };

		public static readonly Collections.BTypeValuesParams<float> kBListTypeValuesParams_Cost = new
			Collections.BTypeValuesParams<float>("Cost", "ResourceType", db => db.GameData.Resources) { kTypeGetInvalid = Util.kGetInvalidSingle };
		public static readonly Collections.BTypeValuesParams<float> kBListTypeValuesParams_CostLowercaseType = !kUseLowercaseCostTypeHack ? kBListTypeValuesParams_Cost : new
			Collections.BTypeValuesParams<float>("Cost", "ResourceType".ToLower(), db => db.GameData.Resources) { kTypeGetInvalid = Util.kGetInvalidSingle };

		public static readonly Collections.BTypeValuesParams<float> kBListTypeValuesParams_AddResource = new
			Collections.BTypeValuesParams<float>("AddResource", null, db => db.GameData.Resources)
			{
				kTypeGetInvalid = Util.kGetInvalidSingle,
				Flags = Collections.BCollectionParamsFlags.UseInnerTextForData
			};

		const string kXmlAttrDeductable = "Deductable";
		#endregion

		bool mDeductable;
		public bool Deductable { get { return mDeductable; } }

		#region BListAutoIdObject Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
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
			Collections.BTypeValuesParams<BPopulation>("Pop", "Type", db => db.GameData.Populations)
			{
				kTypeGetInvalid = () => BPopulation.kInvalid
			};
		public static readonly Collections.BTypeValuesParams<float> kBListParamsSingle = new
			Collections.BTypeValuesParams<float>("Pop", "Type", db => db.GameData.Populations)
			{
				kTypeGetInvalid = Util.kGetInvalidSingle
			};
		public static readonly Collections.BTypeValuesParams<float> kBListParamsSingle_LowerCase = new
			Collections.BTypeValuesParams<float>("Pop", "Type".ToLower(), db => db.GameData.Populations)
			{
				kTypeGetInvalid = Util.kGetInvalidSingle
			};
		public static readonly Collections.BTypeValuesParams<float> kBListParamsSingle_CapAddition = new
			Collections.BTypeValuesParams<float>("PopCapAddition", "Type", db => db.GameData.Populations)
			{
				kTypeGetInvalid = Util.kGetInvalidSingle
			};

		const string kXmlAttrMax = "Max";
		#endregion

		static readonly BPopulation kInvalid = new BPopulation(Util.kInvalidSingle, Util.kInvalidSingle);

		float mMax;
		public float Max { get { return mMax; } }

		float mCount;
		public float Count { get { return mCount; } }

		BPopulation(float max, float count) { mMax = max; mCount = count; }

		#region IXmlElementStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
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

		static readonly Collections.BListParams kRatesParams = new Collections.BListParams("Rate");
		public static readonly Collections.BTypeValuesParams<float> kRatesBListTypeValuesParams = new
			Collections.BTypeValuesParams<float>("Rate", "Rate", db => db.GameData.Rates) { kTypeGetInvalid = Util.kGetInvalidSingle };

		static readonly Collections.BListParams kPopsParams = new Collections.BListParams("Pop");
		static readonly Collections.BListParams kRefCountsParams = new Collections.BListParams("RefCount");
		static readonly Collections.BListParams kHUDItemsParams = new Collections.BListParams("HUDItem");
		static readonly Collections.BListParams kFlashableItemsParams = new PhxLib.Collections.BListParams
		{
			RootName = "FlashableItems",
			ElementName = "Item",
			Flags = PhxLib.Collections.BCollectionParamsFlags.UseInnerTextForData,
		};
		static readonly Collections.BListParams kUnitFlagsParams = new Collections.BListParams("UnitFlag");
		static readonly Collections.BListParams kSquadFlagsParams = new Collections.BListParams("SquadFlag");
		static readonly Collections.BTypeValuesParams<string> kCodeProtoObjectsParams = new Collections.BTypeValuesParams<string>("CodeProtoObject", "Type",
			db => db.GameProtoObjectTypes, Collections.BCollectionParamsFlags.ToLowerDataNames)
		{
			RootName = "CodeProtoObjects",
		};
		static readonly Collections.BTypeValuesParams<string> kCodeObjectTypesParams = new Collections.BTypeValuesParams<string>("CodeObjectType", "Type",
			db => db.GameObjectTypes)
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
		public float mDamageReceivedXPFactor = Util.kInvalidSingle;
		public float DamageReceivedXPFactor { get { return mDamageReceivedXPFactor; } }
		#endregion

		/// <summary>Get how much it costs, in total, to tribute a resource to another player</summary>
		public float TotalTributeCost { get { return (mTributeAmount * mTributeCost) + mTributeAmount; } }

		public BGameData()
		{
			Resources = new Collections.BListAutoId<BResource>(BResource.kBListParams);
			Rates = new Collections.BTypeNames(kRatesParams);
			Populations = new Collections.BTypeNames(kPopsParams);
			RefCounts = new Collections.BTypeNames(kRefCountsParams);
			#region Nonsense
			HUDItems = new Collections.BTypeNames(kHUDItemsParams);
			FlashableItems = new Collections.BTypeNames(kFlashableItemsParams);
			UnitFlags = new Collections.BTypeNames(kUnitFlagsParams);
			SquadFlags = new Collections.BTypeNames(kSquadFlagsParams);
			#endregion
			CodeProtoObjects = new Collections.BTypeValuesString(kCodeProtoObjectsParams);
			CodeObjectTypes = new Collections.BTypeValuesString(kCodeObjectTypesParams);
		}

		#region IXmlElementStreamable Members
		/// <remarks>For streaming directly from gamedata.xml</remarks>
		internal void StreamGameXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			Resources.StreamXml(s, mode, db);
			Rates.StreamXml(s, mode, db);
			Populations.StreamXml(s, mode, db);
			RefCounts.StreamXml(s, mode, db);
			#region Nonsense
			HUDItems.StreamXml(s, mode, db);
			FlashableItems.StreamXml(s, mode, db);
			UnitFlags.StreamXml(s, mode, db);
			SquadFlags.StreamXml(s, mode, db);
			#endregion
			CodeProtoObjects.StreamXml(s, mode, db);
			CodeObjectTypes.StreamXml(s, mode, db);

			#region Misc values
			s.StreamElementOpt(mode, kXmlElementGarrisonDamageMultiplier, ref mGarrisonDamageMultiplier, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementConstructionDamageMultiplier, ref mConstructionDamageMultiplier, Util.kNotInvalidSinglePredicate);
			//
			s.StreamElementOpt(mode, kXmlElementShieldRegenDelay, ref mShieldRegenDelay, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementShieldRegenTime, ref mShieldRegenTime, Util.kNotInvalidSinglePredicate);
			//
			s.StreamElementOpt(mode, kXmlElementAttackRatingMultiplier, ref mAttackRatingMultiplier, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementDefenseRatingMultiplier, ref mDefenseRatingMultiplier, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementGoodAgainstMinAttackRating, ref mGoodAgainstMinAttackRating, Util.kNotInvalidSinglePredicate);
			//
			s.StreamElementOpt(mode, kXmlElementChanceToRocket, ref mChanceToRocket, Util.kNotInvalidSinglePredicate);
			//
			s.StreamElementOpt(mode, kXmlElementTributeAmount, ref mTributeAmount, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementTributeCost, ref mTributeCost, Util.kNotInvalidSinglePredicate);
			//
			s.StreamElementOpt(mode, kXmlElementDamageReceivedXPFactor, ref mDamageReceivedXPFactor, Util.kNotInvalidSinglePredicate);
			#endregion
		}

		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			using (s.EnterCursorBookmark(mode, kXmlRoot))
				StreamGameXml(s, mode, db);
		}
		#endregion
	};
}