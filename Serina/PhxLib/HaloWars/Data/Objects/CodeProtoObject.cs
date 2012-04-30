using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PhxLib.HaloWars
{
	public enum BUnitFlag
	{
		AttackBlocked,
	};
	public enum BSquadFlag
	{
		AttackBlocked,
	};

	public enum BCodeProtoObject
	{
		Revealer,
		Blocker,
		AIEyeIcon,
		UnitStart,
		RallyStart,
		Obstruction,
		ObjectiveArrow,
		ObjectiveLocArrow,
		PhysicsThrownObject,
		SkirmishEmptyBaseObject,
		Scn07Scarab,
		SkirmishScarab,
		Cobra,
		Vampire,
		LeaderPowerCaster,
		SPCHeroForge,
		SPCHeroForgeWarthog,
		SPCHeroAnders,
		SPCHeroSpartanRocket,
		SPCHeroSpartanMG,
		SPCHeroSpartanSniper,
		SmallSnowMound,
		MediumSnowMound,
		LargeSnowMound,
		Stun,
		EMP,
		HotdropPadBeam,
	};

	public enum BCodeObjectType
	{
		Building,
		BuildingSocket,
		TurretSocket,
		TurretBuilding,
		Settlement,
		Base,
		Icon,
		Gatherable,
		Infantry,
		Transporter,
		Transportable,
		Flood,
		Cover,
		LOSObstructable,
		ObjectiveArrow,
		CampaignHero,
		Hero,
		HeroDeath,
		HotDropPickup,
		TeleportDropoff,
		TeleportPickup,
		GroundVehicle,
		Garrison,
		UnscSupplyPad,
		CovSupplyPad,
		BaseShield,
		WallShield,
		Leader,
		Covenant,
		Unsc,
		BlackBox,
		Skull,
		BirthOnTop,
		CanCryo,
		Hook,
	};
}