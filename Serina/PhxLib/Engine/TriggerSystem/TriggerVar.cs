using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib.Engine
{
	public enum BTriggerVarType : uint
	{
		#region 0x0
		None,

		Tech,
		/// <see cref="BProtoTechStatus"/>
		TechStatus,
		/// <see cref="BOperatorType"/>
		Operator,
		ProtoObject,
		ObjectType,
		ProtoSquad,
		Sound,
		Entity,
		EntityList,
		Trigger,
		Time,
		Player,
		UILocation,
		UIEntity,
		Cost,
		#endregion

		#region 0x10
		AnimType,
		/// <see cref="BActionStatus"/>
		ActionStatus,
		Power,
		Bool,
		Float,
		Iterator,
		Team,
		PlayerList,
		TeamList,
		PlayerState,
		Objective,
		Unit,
		UnitList,
		Squad,
		SquadList,
		UIUnit,
		#endregion

		#region 0x20
		UISquad,
		UISquadList,
		String,
		MessageIndex,
		MessageJustify,
		MessagePoint,
		Color,
		ProtoObjectList,
		ObjectTypeList,
		ProtoSquadList,
		TechList,
		/// <see cref="BMathOperatorType"/>
		MathOperator,
		/// <see cref="BObjectDataType"/>
		ObjectDataType,
		/// <see cref="BObjectDataRelative"/>
		ObjectDataRelative,
		Civ,
		ProtoObjectCollection,
		#endregion

		#region 0x30
		Object,
		ObjectList,
		Group,
		RefCountType,
		UnitFlag,
		/// <see cref="BLOSType"/>
		LOSType,
		EntityFilterSet,
		PopBucket,
		/// <see cref="BListPosition"/>
		ListPosition,
		/// <see cref="BDiplomacy"/>
		Diplomacy,
		/// <see cref="BExposedAction"/>
		ExposedAction,
		/// <see cref="BSquadMode"/>
		SquadMode,
		ExposedScript,
		KBBase,
		KBBaseList,
		/// <see cref="BDataScalar"/>
		DataScalar,
		#endregion

		#region 0x40
		KBBaseQuery,
		DesignLine,
		LocStringID,
		Leader,
		Cinematic,
		/// <see cref="BFlareType"/>
		FlareType,
		CinematicTag,
		IconType,
		Difficulty,
		Integer,
		HUDItem,
		/// <see cref="BControlType"/>
		ControlType,
		UIButton,
		/// <see cref="BMissionType"/>
		MissionType,
		/// <see cref="BMissionState"/>
		MissionState,
		/// <see cref="BMissionTargetType"/>
		MissionTargetType,
		#endregion

		#region 0x50
		IntegerList,
		/// <see cref="BBidType"/>
		BidType,
		/// <see cref="BBidState"/>
		BidState,
		BuildingCommandState,
		Vector,
		VectorList,
		PlacementRule,
		KBSquad,
		KBSquadList,
		KBSquadQuery,
		AISquadAnalysis,
		/// <see cref="BAISquadAnalysisComponent"/>
		AISquadAnalysisComponent,
		KBSquadFilterSet,
		ChatSpeaker,
		/// <see cref="BRumbleType"/>
		RumbleType,
		/// <see cref="BRumbleMotor"/>
		RumbleMotor,
		#endregion

		#region 0x60
		/// <see cref="BProtoObjectCommandType"/>
		CommandType,
		SquadDataType,
		EventType,
		TimeList,
		DesignLineList,
		/// <see cref="BGameStatePredicate"/>
		GameStatePredicate,
		FloatList,
		UILocationMinigame,
		SquadFlag,
		FlashableUIItem, // aka FlashableItems
		TalkingHead,
		Concept,
		ConceptList,
		UserClassType,


		#endregion

		Distance = Float,
		Percent = Float,
		Hitpoints = Float,

		Count = Integer,

		Location = Vector,
		Direction = Vector,

		LocationList = VectorList,
	};

	#region 0x00
	public enum BOperatorType
	{
		NotEqualTo,
		LessThan,
		LessThanOrEqualTo,
		EqualTo,
		GreaterThanOrEqualTo,
		GreaterThan,
	};
	#endregion

	#region 0x10
	public enum BActionStatus
	{
		NotDone,
		DoneSuccess,
		DoneFailure,
	};
	#endregion

	#region 0x20
	public enum BMathOperatorType
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
	};
	public enum BObjectDataType
	{
		#region 0x00
		Enable,
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
		Absolute,
		BasePercent,
		Percent,
		Assign,
		BasePercentAssign,

		Invalid,
	};
	#endregion

	#region 0x30
	public enum BLOSType
	{
		LOSDontCare,
		LOSNormal,
		LOSFullVisible,
	};
	public enum BListPosition
	{
		First,
		Last,
		Random,
	};
	public enum BDiplomacy
	{
		Any,
		Self,
		Ally,
		Enemy,
		Neutral,

		Invalid,
	};
	public enum BExposedAction
	{
		ExposedAction0,
		ExposedAction1,
		ExposedAction2,
	};
	public enum BSquadMode
	{
		Normal,
		StandGround,
		Lockdown,
		Sniper,
		HitAndRun,
		Passive,
		Cover,
		Ability,
		CarryingObject,
		Power,
		ScarabScan,
		ScarabTarget,
		ScarabKill,

		Invalid,
	};
	public enum BDataScalar
	{
		Accuracy,
		WorkRate,
		Damage,
		LOS,
		Velocity,
		WeaponRange,
		DamageTaken,
	};
	#endregion

	#region 0x40
	public enum BFlareType
	{
		Look,
		Help,
		Meet,
		Attack,
	};

	public enum BControlType
	{
		Invalid,

		ControlTilt,
		ControlZoom,
		ControlRotate,
		ControlPan,
		ControlCircleSelect,
		ControlCircleMultiSelect,
		ControlClearAllSelections,
		ControlModifierAction,
		ControlModifierSpeed,
		ControlResetCameraSettings,
		ControlGotoRally,
		ControlGotoBase,
		ControlGotoScout,
		ControlGotoNode,
		ControlGotoHero,
		ControlGotoAlert,
		ControlGotoSelected,
		ControlGroupSelect,
		ControlGroupGoto,
		ControlGroupAssign,
		ControlGroupAddTo,
		ControlScreenSelectMilitary,
		ControlGlobalSelect,
		ControlDoubleClickSelect,
		ControlFindCrowdMilitary,
		ControlFindCrowdVillager,
		ControlSetRallyPoint,
		Flare,
		FlareHelp,
		FlareMeet,
		FlareAttack,
		MenuShowCommand,
		MenuCloseCommand,
		MenuNavCommand,
		MenuCommandHasFocus0,
		MenuCommandHasFocus1,
		MenuCommandHasFocus2,
		MenuCommandHasFocus3,
		MenuCommandHasFocus4,
		MenuCommandHasFocus5,
		MenuCommandHasFocus6,
		MenuCommandHasFocus7,
		MenuCommandHasFocusN,
		MenuCommandClickmenuN,
		MenuCommandIsMenuOpen,
		MenuCommanndIsMenuNotOpen,
		MenuShowPower,
		MenuClosePower,
		MenuPowerHasFocusN,
		MenuPowerClickmenuN,
		MenuPowerIsMenuOpen,
		MenuPowerIsMenuNotOpen,
		MenuShowSelectPower,
		MenuShowAbility,
		MenuShowTribute,
		MenuShowObjectives,
		GameEntityBuilt,
		GameEntityKilled,
		ChatShown,
		ChatRemoved,
		ChatCompleted,
		CommandBowl,
		CommandAbility,
		CommandUnpack,
		CommandDoWork,
		CommandAttack,
		CommandMove,
		CommandTrainSquad,
		CommandTrainSquadCancel,
		CommandResearch,
		CommandResearchCancel,
		CommandBuildOther,
		CommandRecycle,
		CommandRecycleCancel,
		CameraLookingAt,
		SelectUnits,
		CinematicCompleted,
		FadeCompleted,
		UsedPower,
		Timer1Sec,
		ControlCircleSelectFullyGrown,
		PowerOrbitalComplete,
		GameEntityRammed,
		GameEntityJacked,
		GameEntityKilledByNonProjectile,
	};

	public enum BMissionType
	{
		Invalid,
		Attack,
		Defend,
		Scout,
		Claim,
		Power,
	};
	public enum BMissionState
	{
		Invalid,
		Success,
		Failure,
		Create,
		Working,
		Withdraw,
		Retreat,
	};
	public enum BMissionTargetType
	{
		Invalid,
		Area,
		KBBase,
		CaptureNode,
	};
	#endregion

	#region 0x50
	public enum BBidType
	{
		Invalid,
		None,
		Squad,
		Tech,
		Building,
		Power,
	};
	public enum BBidState
	{
		Invalid,
		Inactive,
		Waiting,
		Approved,
	};
	
	public enum BAISquadAnalysisComponent
	{
		Invalid,
		CVLight,
		CVLightArmored,
		CVMedium,
		CVMediumAir,
		CVHeavy,
		CVBuilding,
		CVTotal,
		HPLight,
		HPLightArmored,
		HPMedium,
		HPMediumAir,
		HPHeavy,
		HPBuilding,
		HPTotal,
		SPLight,
		SPLightArmored,
		SPMedium,
		SPMediumAir,
		SPHeavy,
		SPBuilding,
		SPTotal,
		DPSLight,
		DPSLightArmored,
		DPSMedium,
		DPSMediumAir,
		DPSHeavy,
		DPSBuilding,
		DPSTotal,
		CVPercentLight,
		CVPercentLightArmored,
		CVPercentMedium,
		CVPercentMediumAir,
		CVPercentHeavy,
		CVPercentBuilding,
		CVStarsLight,
		CVStarsLightArmored,
		CVStarsMedium,
		CVStarsMediumAir,
		CVStarsHeavy,
		CVStarsBuilding,
		CVStarsTotal,
	};

	public enum BRumbleType
	{
		Invalid,

		Fixed,
		SineWave,
		IntervalBurst,
		RandomNoise,
		Incline,
		Decline,
		BumpLRL,
	};
	public enum BRumbleMotor
	{
		Both,
		Left,
		Right,
	};
	#endregion

	#region 0x60
	public enum BGameStatePredicate
	{
		Invalid,

		SquadsSelected,
		HasSquads,
		HasBuildings,
		HasResources,
		HasTech,
		GameTime,
	};
	#endregion
}