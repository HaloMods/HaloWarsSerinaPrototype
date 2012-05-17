using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using XmlIgnore = System.Xml.Serialization.XmlIgnoreAttribute;

namespace PhxLib.Engine
{
	public enum BObjectDataType
	{
		Invalid = -1,

		#region 0x00
		Enable = 0,
		Hitpoints,
		Shieldpoints,
		AmmoMax,
		LOS,
		MaximumVelocity,
		MaximumRange,
		ResearchPoints,
		ResourceTrickleRate,
		MaximumResourceTrickleRate,
		RateAmount,
		RateMultiplier,
		Resource,
		Projectile,
		Damage,
		MinRange,
		#endregion
		#region 0x10
		AOERadius,
		AOEPrimaryTargetFactor,
		AOEDistanceFactor,
		AOEDamageFactor,
		Accuracy,
		MovingAccuracy,
		MaxDeviation,
		MovingMaxDeviation,
		DataAccuracyDistanceFactor,
		AccuracyDeviationFactor,
		MaxVelocityLead,
		WorkRate,
		BuildPoints,
		Cost,
		AutoCloak,
		MoveWhileCloaked,
		#endregion
		#region 0x20
		AttackWhileCloaked,
		ActionEnable,
		CommandEnable,
		BountyResource,
		TributeCost,
		ShieldRegenRate,
		ShieldRegenDelay,
		DamageModifier,
		PopCap,
		PopMax,
		UnitTrainLimit,
		SquadTrainLimit,
		RepairCost,
		RepairTime,
		PowerRechargeTime,
		PowerUseLimit,
		#endregion
		#region 0x30
		Level,
		Bounty,
		MaxContained,
		MaxDamagePerRam,
		ReflectDamageFactor,
		AirBurstSpan,
		AbilityDisabled,
		DOTrate,
		DOTduration,
		ImpactEffect,
		AmmoRegenRate,
		CommandSelectable,
		DisplayNameID,
		Icon,
		AltIcon,
		Stasis,
		#endregion
		#region 0x40
		TurretYawRate,
		TurretPitchRate,
		PowerLevel,
		BoardTime,
		AbilityRecoverTime,
		TechLevel,
		HPBar,
		WeaponPhysicsMultiplier,
		DeathSpawn,
		






		#endregion
	};
	public enum BObjectDataRelative
	{
		Invalid = -1,

		Absolute = 0,
		BasePercent,
		Percent,
		Assign,
		BasePercentAssign,
	};

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
		Invalid = -1,

		Research = 0,
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
	public enum BProtoObjectFlags
	{
		// 0x88
		AttackWhileCloaked, // 0
		MoveWhileCloaked, // 1
		AutoCloak, // 2
		// 3?
		AbilityDisabled, // 4

		// 0x89
		SoundBehindFOW,// = 1<<0,
		VisibleToAll,// = 1<<1,
		Doppled,// = 1<<2,
		GrayMapDoppled,// = 1<<3,
		NoTieToGround,// = 1<<4,
		ForceToGaiaPlayer,// = 1<<5,

		// 0x8A
		NoGrayMapDoppledInCampaign, // 6
		Neutral,// = 1<<7,

		// 0x348
		ManualBuild, // 0
		Build, // 1
		// HasTrainSquadCommand // 2
		SelectedRect, // 3
		OrientUnitWithGround, // 4
		NonCollideable, // 5, actually "Collideable" in code
		PlayerOwnsObstruction, // 6
		DontRotateObstruction, // 7, actually "RotateObstruction" in code

		// 0x349
		HasHPBar, // 0
		UnlimitedResources, // 1
		DieAtZeroResources, // 2
		// 3?
		// 4?
		Immoveable, // 5

		// 0x34A
		HighArc, // 0
		IsAffectedByGravity, // 1
		// 2?
		// 3?
		Invulnerable, // 4
		AutoRepair, // 5
		BlockMovement, // 6
		BlockLOS, // 7

		// 0x34B
		KillGarrisoned, // 0
		DamageGarrisoned, // 1
		Tracking, // 2
		ShowRange, // 3
		PassiveGarrisoned, // 4
		UngarrisonToGaia, // 5
		Capturable, // 6
		// 7?

		#region 0x34C
		RocketOnDeath, // 0
		VisibleForTeamOnly, // 1
		VisibleForOwnerOnly, // 2
		Destructible, // 3
		KBCreatesBase, // 4
		ExternalShield, // 5
		KBAware, // 6
		UIDecal, // 7
		#endregion

		// 0x34D
		StartAtMaxAmmo, // 0
		TargetsFootOfUnit,// = 1<<1,
		// 2?
		HasTrackMask, // 3
		NoCull, // 4
		FadeOnDeath, // 5
		DontAttackWhileMoving, // 6
		Beam, // 7

		#region 0x34E
		Repairable,// = 1<<0,
		NoRender,// = 1<<1,
		Obscurable,// = 1<<2,
		AlwaysVisibleOnMinimap,// = 1<<3,
		ForceAnimRate, // 4
		NoActionOverrideMove, // 5
		Update, // 6
		InvulnerableWhenGaia, // 7
		#endregion

		// 0x34F
		ForceCreateObstruction, // 0
		DamageLinkedSocketsFirst,// = 1<<1,
		NoBuildUnderAttack,// = 1<<2,
		// 3?
		AirMovement,// = 1<<4,
		WalkToTurn,// = 1<<5,
		ScaleBuildAnimRate, // 6
		DoNotFilterOrient, // 7, actually "FilterOrient" in code

		// 0x350
		IsSticky,// = 1<<0,
		ExpireOnTimer,// = 1<<1,
		ExplodeOnTimer,// = 1<<2,
		CommandableByAnyPlayer,// = 1<<3,
		SingleSocketBuilding,// = 1<<4,
		AlwaysAttackReviveUnits,// = 1<<5,
		DontAutoAttackMe,// = 1<<6,
		// 7?

		#region 0x351
		UseBuildingAction, // 0
		NonRotatable, // 1, actually "Rotatable" in code
		ShatterDeathReplacement, // 2
		DamagedDeathReplacement,// = 1<<3,
		HasPivotingEngines,// = 1<<4,
		OverridesRevive,// = 1<<5,
		LinearCostEscalation,// = 1<<6,
		IsNeedler,// = 1<<7,
		#endregion

		#region 0x352
		ForceUpdateContainedUnits,// = 1<<0,
		IsFlameEffect,// = 1<<1,
		SingleStick,// = 1<<2,
		DieLast, // 3
		ChildForDamageTakenScalar, // 4
		SelfParkingLot, // 5
		KillChildObjectsOnDeath, // 6
		LockdownMenu, // 7
		#endregion

		#region 0x353
		TriggersBattleMusicWhenAttacked,// = 1<<0,
		NoRenderForOwner,// = 1<<1,
		NoCorpse,// = 1<<2,
		ShowRescuedCount,// = 1<<3,
		MustOwnToSelect,// = 1<<4,
		AbilityAttacksMeleeOnly,// = 1<<5,
		RegularAttacksMeleeOnly,// = 1<<6,
		FlattenTerrain,// = 1<<7,
		#endregion

		#region 0x354
		CanSetAsRallyPoint,// = 1<<0,
		IgnoreSquadAI,// = 1<<1,
		NotSelectableWhenChildObject,// = 1<<2,
		Teleporter,// = 1<<3,
		OneSquadContainment,// = 1<<4,
		ProjectileTumbles,// = 1<<5,
		ProjectileObstructable,// = 1<<6,
		AutoExplorationGroup,// = 1<<7,
		#endregion

		#region 0x355
		SelectionDontConformToTerrain, // 0
		PhysicsDetonateOnDeath,// = 1<<1,
		ObstructsAir,// = 1<<2,
		NoRandomMoveAnimStart,// = 1<<3, actually "RandomMoveAnimStart" in code
		HideOnImpact,// = 1<<4,
		PermanentSocket,// = 1<<5,
		SelfDamage,// = 1<<6,
		SecondaryBuildingQueue,// = 1<<7,
		#endregion

		#region 0x356
		UseAutoParkingLot,// = 1<<0,
		UseBuildRotation,// = 1<<1,
		CarryNoRenderToChildren,// = 1<<2,
		UseRelaxedSpeedGroup,// = 1<<3,
		AppearsBelowDecals,// = 1<<4,
		IKTransitionToIdle,// = 1<<5,
		SyncAnimRateToPhysics,// = 1<<6,
		TurnInPlace,// = 1<<7,
		#endregion

		// 0x357
		NoStickyCam,// = 1<<3, actually "StickyCam" in code
		// 4?
		CheckLOSAgainstBase,// = 1<<5,
		// 6?
		KillOnDetach,// = 1<<7,

		//[Obsolete] NonCollidable = NonCollideable, // Fixed in HW's XmlFiles.cs
		[Obsolete, XmlIgnore] NonSolid,
		[Obsolete, XmlIgnore] RenderBelowDecals,
	};

	public enum BProtoObjectSelectType
	{
		None,

		Unit,
		Command,
		Target,
		SingleUnit,
		SingleType,
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

	public class BProtoObjectVeterancy : IO.IPhxXmlStreamable, IComparable<BProtoObjectVeterancy>, IEqualityComparer<BProtoObjectVeterancy>
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

		static readonly BProtoObjectVeterancy kInvalid = new BProtoObjectVeterancy(),
			kDefaultLevel1 = new BProtoObjectVeterancy()
			{
				mDamage = 1.15f, mVelocity = 1, mAccuracy = 1.6f, mWorkRate = 1.2f, mWeaponRange = 1f, mDamageTaken = 0.87f
			},
			kDefaultLevel2 = new BProtoObjectVeterancy()
			{
				mDamage = 1.15f, mVelocity = 1, mAccuracy = 1.7f, mWorkRate = 1.2f, mWeaponRange = 1f, mDamageTaken = 0.80f
			},
			kDefaultLevel3 = new BProtoObjectVeterancy()
			{
				mDamage = 1.15f, mVelocity = 1, mAccuracy = 1.8f, mWorkRate = 1.2f, mWeaponRange = 1f, mDamageTaken = 0.74f
			},
			kDefaultLevel4 = new BProtoObjectVeterancy()
			{
				mDamage = 2.00f, mVelocity = 1, mAccuracy = 1.1f, mWorkRate = 2.0f, mWeaponRange = 1f, mDamageTaken = 0.50f
			},
			kDefaultLevel5 = new BProtoObjectVeterancy()
			{
				mDamage = 2.00f, mVelocity = 1, mAccuracy = 1.2f, mWorkRate = 2.0f, mWeaponRange = 1f, mDamageTaken = 0.50f
			};

		public static IEnumerable<BProtoObjectVeterancy> GetLevelDefaults()
		{
			yield return kDefaultLevel1;
			yield return kDefaultLevel2;
			yield return kDefaultLevel3;
			yield return kDefaultLevel4;
			yield return kDefaultLevel5;
		}

		#region Xml constants
		public static readonly Collections.BListExplicitIndexParams<BProtoObjectVeterancy> kBListExplicitIndexParams = new
			Collections.BListExplicitIndexParams<BProtoObjectVeterancy>(5)
			{
				// We use a zero'd instance as the invalid format
				// Game considers Vets with XP = 0 as 'null' basically
				kTypeGetInvalid = () => kInvalid
			};
		public static readonly XML.BListExplicitIndexXmlParams<BProtoObjectVeterancy> kBListExplicitIndexXmlParams = new
			XML.BListExplicitIndexXmlParams<BProtoObjectVeterancy>("Veterancy", "Level");

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

		public bool IsInvalid { get { return object.ReferenceEquals(this, kInvalid); } }
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
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs)
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
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Object")
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
			FileName = "Objects.xml",
			RootName = kBListXmlParams.RootName
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfoUpdate = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Update,
			Directory = GameDirectory.Data,
			FileName = "Objects_Update.xml",
			RootName = kBListXmlParams.RootName
		};

		static readonly Collections.CodeEnum<BProtoObjectFlags> kFlagsProtoEnum = new Collections.CodeEnum<BProtoObjectFlags>();
		static readonly Collections.BBitSetParams kFlagsParams = new Collections.BBitSetParams(() => kFlagsProtoEnum);

		static readonly Collections.BBitSetParams kObjectTypesParams = new Collections.BBitSetParams(db => db.ObjectTypes);
		static readonly XML.BBitSetXmlParams kObjectTypesXmlParams = new XML.BBitSetXmlParams("ObjectType");

		const string kXmlAttrIs = "is"; // boolean int, only streamed when '0', only used by tools?
		const string kXmlAttrId = "id";

		const string kXmlElementObjectClass = "ObjectClass";

		const string kXmlElementCostEscalation = "CostEscalation";
		const string kXmlElementHitpoints = "Hitpoints";
		const string kXmlElementShieldpoints = "Shieldpoints";
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
		float mShieldpoints = Util.kInvalidSingle;
		public float Shieldpoints { get { return mShieldpoints; } }
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

		internal bool mHasTactics;

		public Collections.BTypeValuesSingle Rates { get; private set; }
		public Collections.BTypeValuesSingle AddResource { get; private set; }

		public Collections.BBitSet Flags { get; private set; }
		public Collections.BBitSet ObjectTypes { get; private set; }

		public BProtoObject() : base(BResource.kBListTypeValuesParams, BResource.kBListTypeValuesXmlParams_Cost)
		{
			Veterancy = new Collections.BListExplicitIndex<BProtoObjectVeterancy>(BProtoObjectVeterancy.kBListExplicitIndexParams);

			Populations = new Collections.BTypeValuesSingle(BPopulation.kBListParamsSingle);
			PopulationsCapAddition = new Collections.BTypeValuesSingle(BPopulation.kBListParamsSingle);

			Rates = new Collections.BTypeValuesSingle(BGameData.kRatesBListTypeValuesParams);
			AddResource = new Collections.BTypeValuesSingle(BResource.kBListTypeValuesParams);

			Flags = new Collections.BBitSet(kFlagsParams);
			ObjectTypes = new Collections.BBitSet(kObjectTypesParams);
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamAttributeOpt(mode, kXmlAttrId, KSoft.NumeralBase.Decimal, ref mId, Util.kNotInvalidPredicate);
			s.StreamElementOpt(mode, kXmlElementObjectClass, ref mClassType, x => x != BProtoObjectClassType.Invalid);

			XML.Util.Serialize(s, mode, xs, Veterancy, BProtoObjectVeterancy.kBListExplicitIndexXmlParams);

			s.StreamElementOpt(mode, kXmlElementCostEscalation, ref mCostEscalation, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementHitpoints, ref mHitpoints, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementAttackGradeDPS, ref mAttackGradeDPS, Util.kNotInvalidPredicateSingle);
//			s.StreamElementOpt(mode, kXmlElementCombatValue, ref mCombatValue, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementBounty, ref mBounty, Util.kNotInvalidPredicateSingle);
			XML.Util.Serialize(s, mode, xs, Populations, BPopulation.kBListXmlParamsSingle);
			XML.Util.Serialize(s, mode, xs, PopulationsCapAddition, BPopulation.kBListXmlParamsSingle_CapAddition);

			xs.StreamXmlTactic(s, mode, kXmlElementTactics, this, ref mHasTactics);

			XML.Util.Serialize(s, mode, xs, Rates, BGameData.kRatesBListTypeValuesXmlParams);
			XML.Util.Serialize(s, mode, xs, AddResource, BResource.kBListTypeValuesXmlParams_AddResource, kXmlElementAddRsrcAttrAmount);

			XML.Util.Serialize(s, mode, xs, Flags, XML.BBitSetXmlParams.kFlagsSansRoot);
			XML.Util.Serialize(s, mode, xs, ObjectTypes, kObjectTypesXmlParams);
		}
		#endregion
	};
}