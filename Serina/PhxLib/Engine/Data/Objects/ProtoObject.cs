﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BProtoObjectMovementType
	{
		Invalid,

		Land,
		Air,
		Flood,
		Scarab,
		Hover,
	};

	public enum BProtoObjectCommandType
	{
		Research,
		TrainUnit,
		Build,
		TrainSquad,
		Unload,
		Reinforce,
		ChangeMode,
		Ability,
		Kill,
		CancelKill,
		Tribute,
		CustomCommand,
		Power,
		BuildOther,
		TrainLock,
		TrainUnlock,
		RallyPoint,
		ClearRallyPoint,
		DestroyBase,
		CancelDestroyBase,
		ReverseHotDrop,
	};
	public enum BProtoObjectClassType
	{
		Invalid,

		Squad,
		Building,
		Unit,
		Projectile,
	};
	[Flags]
	public enum BProtoObjectFlags
	{
		// 0x88

		// 0x89
		SoundBehindFOW = 1<<0,
		VisibleToAll = 1<<1,
		Doppled = 1<<2,
		GrayMapDoppled = 1<<3,
		NoTieToGround = 1<<4,
		ForceToGaiaPlayer = 1<<5,

		// 0x8A
		Neutral = 1<<7,

		// 0x348

		// 0x349

		// 0x34A

		// 0x34B

		// 0x34C

		// 0x34D
		TargetsFootOfUnit = 1<<1,
		//1<<2?

		// 0x34E
		Repairable = 1<<0,
		NoRender = 1<<1,
		Obscurable = 1<<2,
		AlwaysVisibleOnMinimap = 1<<3,

		// 0x34F
		//1<<0?
		DamageLinkedSocketsFirst = 1<<1,
		NoBuildUnderAttack = 1<<2,
		//1<<3?
		AirMovement = 1<<4,
		WalkToTurn = 1<<5,
		//1<<7?

		// 0x350
		IsSticky = 1<<0,
		ExpireOnTimer = 1<<1,
		ExplodeOnTimer = 1<<2,
		CommandableByAnyPlayer = 1<<3,
		SingleSocketBuilding = 1<<4,
		AlwaysAttackReviveUnits = 1<<5,
		DontAutoAttackMe = 1<<6,
		//1<<7?

		// 0x351
		DamagedDeathReplacement = 1<<3,
		HasPivotingEngines = 1<<4,
		OverridesRevive = 1<<5,
		LinearCostEscalation = 1<<6,
		IsNeedler = 1<<7,

		// 0x352
		ForceUpdateContainedUnits = 1<<0,
		IsFlameEffect = 1<<1,
		SingleStick = 1<<2,

		// 0x353
		TriggersBattleMusicWhenAttacked = 1<<0,
		NoRenderForOwner = 1<<1,
		NoCorpse = 1<<2,
		ShowRescuedCount = 1<<3,
		MustOwnToSelect = 1<<4,
		AbilityAttacksMeleeOnly = 1<<5,
		RegularAttacksMeleeOnly = 1<<6,
		FlattenTerrain = 1<<7,

		// 0x354
		CanSetAsRallyPoint = 1<<0,
		IgnoreSquadAI = 1<<1,
		NotSelectableWhenChildObject = 1<<2,
		Teleporter = 1<<3,
		OneSquadContainment = 1<<4,
		ProjectileTumbles = 1<<5,
		ProjectileObstructable = 1<<6,
		AutoExplorationGroup = 1<<7,

		// 0x355
		//1<<0
		PhysicsDetonateOnDeath = 1<<1,
		ObstructsAir = 1<<2,
		NoRandomMoveAnimStart = 1<<3,
		HideOnImpact = 1<<4,
		PermanentSocket = 1<<5,
		SelfDamage = 1<<6,
		SecondaryBuildingQueue = 1<<7,

		// 0x356
		UseAutoParkingLot = 1<<0,
		UseBuildRotation = 1<<1,
		CarryNoRenderToChildren = 1<<2,
		UseRelaxedSpeedGroup = 1<<3,
		AppearsBelowDecals = 1<<4,
		IKTransitionToIdle = 1<<5,
		SyncAnimRateToPhysics = 1<<6,
		TurnInPlace = 1<<7,

		// 0x357
		StickyCam = 1<<3,
		//1<<4
		CheckLOSAgainstBase = 1<<5,
		//1<<6
		KillOnDetach = 1<<7,
	};

	//ChildObjectType
	//ParkingLot
	//Socket
	//Rally
	//OneTimeSpawnSquad, inner text is a proto squad, not proto object
	//Unit
	//Foundation
	//ChildObject.UserCiv
	//ChildObject.AttachBone (string)
	//ChildObject.Rotation (float)

	public enum BObjectSocketPlayerType
	{
		Any,
		Player,
		Team,
		Enemy,
		Gaia,
	};
	//BObjectSocket.AutoSocket bool

	public struct BProtoObjectVeterancy : IO.IPhxXmlStreamable, IComparable<BProtoObjectVeterancy>, IEqualityComparer<BProtoObjectVeterancy>
	{
		class _EqualityComparer : IEqualityComparer<BProtoObjectVeterancy>
		{
			#region IEqualityComparer<BProtoObjectVeterancy> Members
			public bool Equals(BProtoObjectVeterancy x, BProtoObjectVeterancy y)
			{
				return x.XP == y.XP && x.Damage == y.Damage && x.Velocity == y.Velocity && x.Accuracy == y.Accuracy &&
					x.WorkRate == y.WorkRate && x.WeaponRange == y.WeaponRange && x.DamageTaken == y.DamageTaken;
			}

			public int GetHashCode(BProtoObjectVeterancy obj)
			{
				return obj.XP.GetHashCode() ^ obj.Damage.GetHashCode() ^ obj.Velocity.GetHashCode() ^ obj.Accuracy.GetHashCode() ^
					obj.WorkRate.GetHashCode() ^ obj.WeaponRange.GetHashCode() ^ obj.DamageTaken.GetHashCode();
			}
			#endregion
		};
		public static readonly IEqualityComparer<BProtoObjectVeterancy> kEqualityComparer = new _EqualityComparer();

		#region Xml constants
		public static readonly Collections.BListExplicitIndexParams<BProtoObjectVeterancy> kBListExplicitIndexParams = new
			Collections.BListExplicitIndexParams<BProtoObjectVeterancy>("Veterancy", "Level", 5)
			{
				// We use a zero'd instance as the invalid format
				// Game considers Vets with XP = 0 as 'null' basically
				kTypeGetInvalid = () => new BProtoObjectVeterancy()
			};

		const string kXmlAttrXP = "XP";
		const string kXmlAttrDamage = "Damage";
		const string kXmlAttrVelocity = "Velocity";
		const string kXmlAttrAccuracy = "Accuracy";
		const string kXmlAttrWorkRate = "WorkRate";
		const string kXmlAttrWeaponRange = "WeaponRange";
		const string kXmlAttrDamageTaken = "DamageTaken";
		#endregion

		float mXP;
		public float XP { get { return mXP; } }
		float mDamage;
		public float Damage { get { return mDamage; } }
		float mVelocity;
		public float Velocity { get { return mVelocity; } }
		float mAccuracy;
		public float Accuracy { get { return mAccuracy; } }
		float mWorkRate;
		public float WorkRate { get { return mWorkRate; } }
		float mWeaponRange;
		public float WeaponRange { get { return mWeaponRange; } }
		float mDamageTaken;
		public float DamageTaken { get { return mDamageTaken; } }

		public bool IsNull { get { return mXP == 0.0f; } }

		#region IComparable<BProtoObjectVeterancy> Members
		int IComparable<BProtoObjectVeterancy>.CompareTo(BProtoObjectVeterancy other)
		{
			if (this.XP < other.XP) return -1;
			else if (this.XP > other.XP) return 1;

			return 0;
		}
		#endregion

		#region IXmlElementStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttributeOpt(mode, kXmlAttrXP, ref mXP, Util.kNotZeroPredicateSingle);
			s.StreamAttributeOpt(mode, kXmlAttrDamage, ref mDamage, Util.kNotZeroPredicateSingle);
			s.StreamAttributeOpt(mode, kXmlAttrVelocity, ref mVelocity, Util.kNotZeroPredicateSingle);
			s.StreamAttributeOpt(mode, kXmlAttrAccuracy, ref mAccuracy, Util.kNotZeroPredicateSingle);
			s.StreamAttributeOpt(mode, kXmlAttrWorkRate, ref mWorkRate, Util.kNotZeroPredicateSingle);
			s.StreamAttributeOpt(mode, kXmlAttrWeaponRange, ref mWeaponRange, Util.kNotZeroPredicateSingle);
			s.StreamAttributeOpt(mode, kXmlAttrDamageTaken, ref mDamageTaken, Util.kNotZeroPredicateSingle);
		}
		#endregion

		#region IEqualityComparer<BProtoObjectVeterancy> Members
		public bool Equals(BProtoObjectVeterancy x, BProtoObjectVeterancy y)
		{
			return kEqualityComparer.Equals(x, y);
		}

		public int GetHashCode(BProtoObjectVeterancy obj)
		{
			return kEqualityComparer.GetHashCode(obj);
		}
		#endregion
	};

	public class BProtoObject : DatabaseIdObject
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Object")
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
			FileName = "Objects.xml",
			RootName = kBListParams.RootName
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfoUpdate = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Update,
			Directory = GameDirectory.Data,
			FileName = "Objects_Update.xml",
			RootName = kBListParams.RootName
		};

		static readonly Collections.BBitSetParams kObjectTypesParams = new Collections.BBitSetParams("ObjectType",
			db => db.ObjectTypes);

		const string kXmlAttrId = "id";

		const string kXmlElementObjectClass = "ObjectClass";

		const string kXmlElementCostEscalation = "CostEscalation";
		const string kXmlElementHitpoints = "Hitpoints";
		internal const string kXmlElementAttackGradeDPS = "AttackGradeDPS";
		const string kXmlElementCombatValue = "CombatValue";
		const string kXmlElementBounty = "Bounty";

		const string kXmlElementTactics = "Tactics";

		const string kXmlElementAddRsrcAttrAmount = "Amount";

		const string kXmlElementPlacementRules = "PlacementRules"; // PlacementRules file name (sans extension)
		#endregion

		int mId = Util.kInvalidInt32;
		public int Id { get { return mId; } }

		BProtoObjectClassType mClassType;
		public BProtoObjectClassType ClassType { get { return mClassType; } }

		public Collections.BListExplicitIndex<BProtoObjectVeterancy> Veterancy { get; private set; }

		float mCostEscalation = Util.kInvalidSingle;
		/// <summary>see: UNSC reactors</summary>
		// Also, CostEscalationObject and Flag.LinearCostEscalation
		public float CostEscalation { get { return mCostEscalation; } }
		public bool HasCostEscalation { get { return Util.kNotInvalidPredicateSingle(CostEscalation); } }

		float mHitpoints = Util.kInvalidSingle;
		public float Hitpoints { get { return mHitpoints; } }
		float mAttackGradeDPS = Util.kInvalidSingle;
		public float AttackGradeDPS { get { return mAttackGradeDPS; } }
//		float mCombatValue = Util.kInvalidSingle;
//		/// <summary>Score value</summary>
//		public float CombatValue { get { return mCombatValue; } }
		float mBounty = Util.kInvalidSingle;
		/// <summary>Vet XP contribution value</summary>
		public float Bounty { get { return mBounty; } }
		public Collections.BTypeValuesSingle Populations { get; private set; }
		public Collections.BTypeValuesSingle PopulationsCapAddition { get; private set; }

		bool mHasTactics;
		//string mTactics;
		//public string Tactics { get { return mTactics; } }

		public Collections.BTypeValuesSingle Rates { get; private set; }
		public Collections.BTypeValuesSingleAttrHack AddResource { get; private set; }

		public Collections.BBitSet ObjectTypes { get; private set; }

		public BProtoObject() : base(BResource.kBListTypeValuesParams_Cost)
		{
			Veterancy = new Collections.BListExplicitIndex<BProtoObjectVeterancy>(BProtoObjectVeterancy.kBListExplicitIndexParams);

			Populations = new Collections.BTypeValuesSingle(BPopulation.kBListParamsSingle);
			PopulationsCapAddition = new Collections.BTypeValuesSingle(BPopulation.kBListParamsSingle_CapAddition);

			Rates = new Collections.BTypeValuesSingle(BGameData.kRatesBListTypeValuesParams);
			AddResource = new Collections.BTypeValuesSingleAttrHack(BResource.kBListTypeValuesParams_AddResource, kXmlElementAddRsrcAttrAmount);

			ObjectTypes = new Collections.BBitSet(kObjectTypesParams);
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			base.StreamXml(s, mode, db);

			s.StreamAttributeOpt(mode, kXmlAttrId, KSoft.NumeralBase.Decimal, ref mId, Util.kNotInvalidPredicate);
			s.StreamElementOpt(mode, kXmlElementObjectClass, ref mClassType, x => x != BProtoObjectClassType.Invalid);

			Veterancy.StreamXml(s, mode, db);

			s.StreamElementOpt(mode, kXmlElementCostEscalation, ref mCostEscalation, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementHitpoints, ref mHitpoints, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementAttackGradeDPS, ref mAttackGradeDPS, Util.kNotInvalidPredicateSingle);
//			s.StreamElementOpt(mode, kXmlElementCombatValue, ref mCombatValue, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementBounty, ref mBounty, Util.kNotInvalidPredicateSingle);
			Populations.StreamXml(s, mode, db);
			PopulationsCapAddition.StreamXml(s, mode, db);

			//s.StreamElementOpt(mode, kXmlElementTactics, ref mTactics, Util.kNotNullOrEmpty);
			db.StreamXmlTactic(s, mode, kXmlElementTactics, this, ref mHasTactics);

			Rates.StreamXml(s, mode, db);
			AddResource.StreamXml(s, mode,db );

			ObjectTypes.StreamXml(s, mode, db);
		}
		#endregion
	};
}