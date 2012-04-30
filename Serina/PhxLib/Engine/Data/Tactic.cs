using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;
using XmlIgnore = System.Xml.Serialization.XmlIgnoreAttribute;

namespace PhxLib.Engine
{
	public enum BActionType
	{
		#region 0x00
		[XmlIgnore] Idle, // entity
		[XmlIgnore] Listen, // entity
		Move, GaggleMove = Move, // unit...
		MoveAir,
		[XmlIgnore] MoveWarthog,
		[XmlIgnore] MoveGhost,
		RangedAttack, HandAttack = RangedAttack,
		[XmlIgnore] Building,
		[XmlIgnore] DOT,
		[XmlIgnore] UnitChangeMode,
		[XmlIgnore] Death,
		[XmlIgnore] InfectDeath,
		Garrison,
		Ungarrison,
		[XmlIgnore] ShieldRegen,
		Honk,
		#endregion
		#region 0x10
		SpawnSquad,
		Capture,
		Join,
		ChangeOwner,
		[XmlIgnore] AmmoRegen,
		Physics,
		[XmlIgnore] PlayBlockingAnimation,
		Mines,
		Detonate,
		Gather,
		CollisionAttack,
		AreaAttack,
		UnderAttack,
		SecondaryTurretAttack,
		RevealToTeam,
		AirTrafficControl,
		#endregion
		#region 0x20
		Hitch,
		Unhitch,
		SlaveTurretAttack,
		Thrown,
		Dodge,
		Deflect,
		AvoidCollisionAir,
		[XmlIgnore] PlayAttachmentAnims,
		Heal,
		Revive,
		Buff,
		Infect,
		HotDrop,
		TentacleDormant,
		[XmlIgnore] HeroDeath,
		Stasis,
		#endregion
		#region 0x30
		BubbleShield,
		Bomb,
		PlasmaShieldGen,
		Jump,
		AmbientLifeSpawner,
		JumpGather,
		JumpGarrison,
		JumpAttack,
		PointBlankAttack,
		Roar,
		EnergyShield,
		[XmlIgnore] _Unknown3B, // ScaleLOS or ChargedRangedAttack?
		Charge,
		TowerWall,
		AoeHeal,
		[XmlIgnore] Attack, // squad
		#endregion
		#region 0x40
		ChangeMode, // squad...
		[XmlIgnore] Repair,
		RepairOther,
		[XmlIgnore] SquadShieldRegen,
		[XmlIgnore] SquadGarrison,
		[XmlIgnore] SquadUngarrison,
		Transport,
		[XmlIgnore] SquadPlayBlockingAnimation,
		[XmlIgnore] SquadMove,
		[XmlIgnore] Reinforce,
		[XmlIgnore] Work,
		CarpetBomb,
		AirStrike,
		[XmlIgnore] SquadHitch,
		[XmlIgnore] SquadUnhitch,
		[XmlIgnore] SquadDetonate,
		#endregion
		#region 0x50
		Wander,
		Cloak,
		[XmlIgnore] CloakDetect,
		Daze,
		[XmlIgnore] SquadJump,
		AmbientLife,
		ReflectDamage,
		Cryo,
		[XmlIgnore] PlatoonMove,
		CoreSlide, // unit...
		InfantryEnergyShield,
		Dome,
		SpiritBond, // squad
		Rage, // unit
		CloakDetector, // ?
		//
		#endregion
	};

	public enum BWeaponFlags
	{
		// 0x58
		ThrowDamageParts,
		ThrowAliveUnits,
		ThrowUnits,
		UsesAmmo,
		PhysicsLaunchAxial,
		EnableHeightBonusDamage,
		AllowFriendlyFire,

		// 0x59
		UseGroupRange,
		//
		UseDPSasDPA,
		SmallArmsDeflectable,
		Deflectable,
		Dodgeable,
		FlailThrownUnits,
		//PulseObject, // Set automatically when the PulseObject element is streamed

		// 0x5A
		//
		//
		Tentacle,
		ApplyKnockback,
		StasisBomb,
		StasisDrain,
		//StasisSmartTargeting,
		CarriedObjectAsProjectileVisual,

		// 0x5B
		//
		//
		//
		//
		AOEIgnoresYAxis, // 1<<4
		OverridesRevive,
		//
		AOELinearDamage,

		// 0xDC
		//
		//
		//
		AirBurst, // 1<<3
		PullUnits,
		//
		KeepDPSRamp,
		TargetsFootOfUnit,
	};

	public enum BProtoActionFlags
	{
		// 0x138
		InstantAttack,
		MeleeRange,
		KillSelfOnAttack,
		DontCheckOrientTolerance,
		DontLoopAttackAnim,
		StopAttackingWhenAmmoDepleted,
		MainAttack,
		Stationary,

		// 0x139
		//Strafing, // 1<<5, set when the Strafing element is streamed
		//CanOrientOwner, // 1<<6, actually tests the inner text of the element for 'false'
		Infection, // 1<<7

		// 0x13C
		//
		//
		//
		//
		WaitForDodgeCooldown,
		MultiDeflect,
		WaitForDeflectCooldown,
		//

		// 0x13D
		AvoidOnly,
		HideSpawnUntilRelease,
		DoShakeOnAttackTag,
		SmallArms,

		// 0x13E
		DontAutoRestart, // 1<<7
	};

	public enum BTacticTargetRuleFlags
	{
		// 0x28
		//AutoTargetSquadMode, // set when a AutoTargetSquadMode element is used instead of SquadMode
		//
		ContainsUnits,
		TargetsGround,

		// 0x29
		//
		//
		//
		//
		//
		MeleeAttacker,
		MergeSquads,
		//OptionalAbility
	};

	[Flags]
	public enum BTacticTargetRuleTargetState
	{
		// 0x28
		//
		GaiaOwned = 1<<1,
		//
		//
		Capturable = 1<<4,
		Damaged = 1<<5,
		Unbuilt = 1<<6,
	};

	public class BDamageRatingOverride : IO.IPhxXmlStreamable, IEqualityComparer<BDamageRatingOverride>
	{
		#region Xml constants
		public static readonly Collections.BTypeValuesParams<BDamageRatingOverride> kBListParams = new
			Collections.BTypeValuesParams<BDamageRatingOverride>("DamageRatingOverride", "type", db => db.DamageTypes);
		#endregion

		float mRating = Util.kInvalidInt32;
		public float Rating { get { return mRating; } }

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamCursor(mode, ref mRating);
		}
		#endregion

		#region IEqualityComparer<BDamageRatingOverride> Members
		public bool Equals(BDamageRatingOverride x, BDamageRatingOverride y)
		{
			return x.Rating == y.Rating;
		}

		public int GetHashCode(BDamageRatingOverride obj)
		{
			return obj.Rating.GetHashCode();
		}
		#endregion
	};
	public class BTargetPriority : IO.IPhxXmlStreamable, IEqualityComparer<BTargetPriority>
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "TargetPriority",
		};

		const string kXmlAttrType = "type"; // proto unit or object type
		#endregion

		int mUnitTypeID = Util.kInvalidInt32;
		public int UnitTypeID { get { return mUnitTypeID; } }
		float mPriority = Util.kInvalidSingle;
		public float Priority { get { return mPriority; } }

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.StreamXmlForDBID(s, mode, kXmlAttrType, ref mUnitTypeID, DatabaseObjectKind.Unit, false, Util.kSourceAttr);
			s.StreamCursor(mode, ref mPriority);
		}
		#endregion

		#region IEqualityComparer<BTargetPriority> Members
		public bool Equals(BTargetPriority x, BTargetPriority y)
		{
			return x.UnitTypeID == y.UnitTypeID && x.Priority == y.Priority;
		}

		public int GetHashCode(BTargetPriority obj)
		{
			return obj.UnitTypeID.GetHashCode() ^ obj.Priority.GetHashCode();
		}
		#endregion
	};
	public class BWeapon : Collections.BListAutoIdObject
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "Weapon",
			DataName = "Name",
			Flags = Collections.BCollectionParamsFlags.UseElementForData |
				Collections.BCollectionParamsFlags.ForceNoRootElementStreaming,
		};

		const string kXmlElementDamagePerSecond = "DamagePerSecond"; // float
		const string kXmlElementDOTRate = "DOTrate"; // float
		const string kXmlElementDOTDuration = "DOTduration"; // float

		const string kXmlElementAttackRate = "AttackRate"; // float
		const string kXmlElementProjectile = "Projectile"; // proto object, inner text

		const string kXmlElementWeaponType = "WeaponType"; // weapon type id, inner text
		const string kXmlElementVisualAmmo = "VisualAmmo"; // integer
		const string kXmlElementTriggerScript = "TriggerScript"; // 

		const string kXmlElementMinRange = "MinRange"; // float
		const string kXmlElementMaxRange = "MaxRange"; // float

		const string kXmlElementReflectDamageFactor = "ReflectDamageFactor"; // float
		const string kXmlElementMovingAccuracy = "MovingAccuracy"; // float
		const string kXmlElementMaxDeviation = "MaxDeviation"; // float
		const string kXmlElementMovingMaxDeviation = "MovingMaxDeviation"; // float
		const string kXmlElementAccuracyDistanceFactor = "AccuracyDistanceFactor"; // float
		const string kXmlElementAccuracyDeviationFactor = "AccuracyDeviationFactor"; // float
		const string kXmlElementMaxVelocityLead = "MaxVelocityLead"; // float
		const string kXmlElementAirBurstSpan = "AirBurstSpan"; // float


		const string kXmlElementStasis = "Stasis";
		const string kXmlElementStasisAttrSmartTargeting = "SmartTargeting"; // bool
		const string kXmlElementStasisHealToDrainRatio = "StasisHealToDrainRatio"; // float

		const string kXmlElementBounces = "Bounces"; // sbyte
		const string kXmlElementBounceRange = "BounceRange"; // float

		const string kXmlElementMaxPullRange = "MaxPullRange"; // float
		#endregion

		float mDamagePerSecond = Util.kInvalidSingle;
		float mDOTRate = Util.kInvalidSingle;
		float mDOTDuration = Util.kInvalidSingle;

		float mAttackRate = Util.kInvalidSingle;
		int mProjectileObjectID = Util.kInvalidInt32;

		int mWeaponTypeID = Util.kInvalidInt32;
		int mVisualAmmo = Util.kInvalidInt32;
		int mTriggerScriptID = Util.kInvalidInt32;

		float mMinRange = Util.kInvalidSingle;
		float mMaxRange = Util.kInvalidSingle;

		float mReflectDamageFactor = Util.kInvalidSingle;
		float mMovingAccuracy = Util.kInvalidSingle;
		float mMaxDeviation = Util.kInvalidSingle;
		float mMovingMaxDeviation = Util.kInvalidSingle;
		float mAccuracyDistanceFactor = Util.kInvalidSingle;
		float mAccuracyDeviationFactor = Util.kInvalidSingle;
		float mMaxVelocityLead = Util.kInvalidSingle;
		float mAirBurstSpan = Util.kInvalidSingle;

		public Collections.BTypeValues<BDamageRatingOverride> DamageOverrides { get; private set; }
		public Collections.BListArray<BTargetPriority> TargetPriorities { get; private set; }

		bool mStasisSmartTargeting;
		float mStasisHealToDrainRatio = Util.kInvalidSingle;

		sbyte mBounces = (sbyte)Util.kInvalidInt32;
		float mBounceRange = Util.kInvalidSingle;

		float mMaxPullRange = Util.kInvalidSingle;

		public BWeapon()
		{
			DamageOverrides = new Collections.BTypeValues<BDamageRatingOverride>(BDamageRatingOverride.kBListParams);
			TargetPriorities = new Collections.BListArray<BTargetPriority>(BTargetPriority.kBListParams);
		}

		#region BListAutoIdObject Members
		bool ShouldStreamStasis(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read) return s.ElementsExists(kXmlElementStasis);
			else if (mode == FA.Write) return mStasisSmartTargeting;

			return false;
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamElementOpt(mode, kXmlElementDamagePerSecond, ref mDamagePerSecond, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementDOTRate, ref mDOTRate, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementDOTDuration, ref mDOTDuration, Util.kNotInvalidSinglePredicate);

			s.StreamElementOpt(mode, kXmlElementAttackRate, ref mAttackRate, Util.kNotInvalidSinglePredicate);
			db.StreamXmlForDBID(s, mode, kXmlElementProjectile, ref mProjectileObjectID, DatabaseObjectKind.Object);

			db.StreamXmlForDBID(s, mode, kXmlElementWeaponType, ref mWeaponTypeID, DatabaseObjectKind.WeaponType);
			s.StreamElementOpt(mode, kXmlElementVisualAmmo, KSoft.NumeralBase.Decimal, ref mVisualAmmo, Util.kNotInvalidPredicate);
			//mTriggerScriptID

			s.StreamElementOpt(mode, kXmlElementMinRange, ref mMinRange, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementMaxRange, ref mMaxRange, Util.kNotInvalidSinglePredicate);

			s.StreamElementOpt(mode, kXmlElementReflectDamageFactor, ref mReflectDamageFactor, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementMovingAccuracy, ref mMovingAccuracy, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementMaxDeviation, ref mMaxDeviation, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementMovingMaxDeviation, ref mMovingMaxDeviation, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementAccuracyDistanceFactor, ref mAccuracyDistanceFactor, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementAccuracyDeviationFactor, ref mAccuracyDeviationFactor, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementMaxVelocityLead, ref mMaxVelocityLead, Util.kNotInvalidSinglePredicate);
			s.StreamElementOpt(mode, kXmlElementAirBurstSpan, ref mAirBurstSpan, Util.kNotInvalidSinglePredicate);

			DamageOverrides.StreamXml(s, mode, db);
			TargetPriorities.StreamXml(s, mode, db);

			if (ShouldStreamStasis(s, mode))
			{
				using (s.EnterCursorBookmark(mode, kXmlElementStasis))
					s.StreamAttribute(mode, kXmlElementStasisAttrSmartTargeting, ref mStasisSmartTargeting);
			}
			s.StreamElementOpt(mode, kXmlElementStasisHealToDrainRatio, ref mStasisHealToDrainRatio, Util.kNotInvalidSinglePredicate);

			s.StreamElementOpt(mode, kXmlElementBounces, KSoft.NumeralBase.Decimal, ref mBounces, Util.kNotInvalidPredicateSByte);
			s.StreamElementOpt(mode, kXmlElementBounceRange, ref mBounceRange, Util.kNotInvalidSinglePredicate);

			s.StreamElementOpt(mode, kXmlElementMaxPullRange, ref mMaxPullRange, Util.kNotInvalidSinglePredicate);
		}
		#endregion
	};

	public class BTacticState // suicide grunts use this...name and action are omitted, so fuck this
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "State",
			DataName = "Name",
			Flags = Collections.BCollectionParamsFlags.UseElementForData |
				Collections.BCollectionParamsFlags.ForceNoRootElementStreaming,
		};

		//////////////////////////////////////////////////////////////////////////
		// anim names
		const string kXmlElementIdleAnim = "IdleAnim";
		const string kXmlElementWalkAnim = "WalkAnim";
		const string kXmlElementJogAnim = "JogAnim";
		const string kXmlElementRunAnim = "RunAnim";
		const string kXmlElementDeathAnim = "DeathAnim";
		//////////////////////////////////////////////////////////////////////////
		const string kXmlElementAction = "Action";
		#endregion
	};

	public class BProtoAction
	{
		enum BJoinType
		{
			Follow,
			Merge,
			Board,
			FollowAttack,
		};
		enum BMergeType
		{
			None,

			Ground,
			Air,
		};

		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "Action",
			DataName = "Name",
			Flags = Collections.BCollectionParamsFlags.UseElementForData |
				Collections.BCollectionParamsFlags.ForceNoRootElementStreaming,
		};

		const string kXmlElementActionType = "ActionType";
		const string kXmlElementProjectileSpread = "ProjectileSpread"; // float

		const string kXmlElementSquadType = "SquadType"; // proto squad
		const string kXmlElementWeapon = "Weapon"; // BWeapon
		const string kXmlElementLinkedAction = "LinkedAction"; // BProtoAction

		const string kXmlElementSquadMode = "SquadMode"; // BSquadMode
		const string kXmlElementNewSquadMode = "NewSquadMode"; // BSquadMode
		const string kXmlElementNewTacticState = "NewTacticState"; // BTacticState?

		const string kXmlElementWorkRate = "WorkRate"; // float
		const string kXmlElementWorkRateVariance = "WorkRateVariance"; // float
		const string kXmlElementWorkRange = "WorkRange"; // float

		const string kXmlElementDamageModifiers = "DamageModifiers";
		const string kXmlElementDamageModifiersAttrDamage = "damage"; // float
		const string kXmlElementDamageModifiersAttrDamageTaken = "damageTaken"; // float
		const string kXmlElementDamageModifiersAttrByCombatValue = "byCombatValue"; // bool

		const string kXmlElementResource = "Resource"; // ResourceID
		const string kXmlElementDefault = "Default"; // bool, if element equals 'true' this is the default action

		const string kXmlElementSlaveAttackAction = "SlaveAttackAction"; // BProtoAction
		const string kXmlElementBaseDPSWeapon = "BaseDPSWeapon"; // BWeapon

		const string kXmlElementPersistentActionType = "PersistentActionType";

		const string kXmlElementDuration = "Duration"; // float
		const string kXmlElementDurationAttrSpread = "DurationSpread"; // float

		const string kXmlElementAutoRepair = "AutoRepair";
		const string kXmlElementAutoRepairAttrIdleTime = "AutoRepairIdleTime"; // int
		const string kXmlElementAutoRepairAttrThreshold = "AutoRepairThreshold"; // float
		const string kXmlElementAutoRepairAttrSearchDistance = "AutoRepairSearchDistance"; // float
		const string kXmlElementInvalidTarget = "InvalidTarget"; // proto object

		const string kXmlElementProtoObject = "ProtoObject";
		const string kXmlElementProtoObjectAttrSquad = "Squad"; // inner text: if 0, proto object, if not, proto squad
		const string kXmlElementCount = "Count"; // StringID
		const string kXmlElementMaxNumUnitsPerformAction = "MaxNumUnitsPerformAction"; // int
		const string kXmlElementDamageCharge = "DamageCharge"; // float
		#endregion
	};

	public class BTacticTargetRule
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "TargetRule",
			Flags = Collections.BCollectionParamsFlags.ForceNoRootElementStreaming
		};

		const string kXmlElementRelation = "Relation"; // BDiplomacy
		const string kXmlElementSquadMode = "SquadMode"; // BSquadMode
		const string kXmlElementAutoTargetSquadMode = "AutoTargetSquadMode"; // BSquadMode
		const string kXmlElementDamageType = "DamageType"; // BDamageType
		const string kXmlElementTargetType = "TargetType"; // object type or proto unit
		const string kXmlElementAction = "Action"; // BProtoAction
		const string kXmlElementTargetState = "TargetState"; // BTacticTargetRuleTargetState
		const string kXmlElementAbility = "Ability"; // BAbility
		const string kXmlElementOptionalAbility = "OptionalAbility"; // BAbility
		const string kXmlElement = "";
		#endregion

		BDiplomacy mRelation = BDiplomacy.Invalid;

		BSquadMode mSquadMode = BSquadMode.Invalid,
			mAutoTargetSquadMode = BSquadMode.Invalid;
	};
	public class BTactic
	{
		#region Xml constants
		const string kXmlRoot = "Tactic";

		const string kXmlElementPersistentAction = "PersistentAction"; // BProtoAction
		const string kXmlElementPersistentSquadAction = "PersistentSquadAction"; // BProtoAction
		const string kXmlElement = "";
		#endregion
	};

	public class BTacticData : IO.IPhxXmlStreamable
	{
		#region Xml constants
		const string kXmlRoot = "TacticData";
		#endregion

		public Collections.BListAutoId<BWeapon> Weapons { get; private set; }

		public BTactic Tactic { get; private set; }

		public BTacticData()
		{
			Weapons = new Collections.BListAutoId<BWeapon>(BWeapon.kBListParams);
			
			Tactic = new BTactic();
		}

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			Weapons.StreamXml(s, mode, db);
		}
		#endregion
	};
}