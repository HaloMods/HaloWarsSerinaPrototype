using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BAbilityType
	{
		Work, // this isn't explicitly parsed by the engine, so it's only logical that it is the 0 member
		ChangeMode,
		Unload,
		Unpack,
		CommandMenu,
		Power,

		Invalid,
	};
	public enum BAbilityTargetType
	{
		Invalid,

		Location,
		Unit,
		UnitOrLocation,
	};
	public enum BRecoverType
	{
		Invalid,

		Attack,
		Ability,
	};
	public enum BMovementModifierType
	{
		Ability,
		Mode,
	};

	/// <summary></summary>
	/// <remarks>
	/// * Has no PrereqTextID property
	/// </remarks>
	public class BAbility : DatabaseNamedObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			RootName = "Abilities",
			ElementName = "Ability",
			DataName = DatabaseNamedObject.kXmlAttrNameN,
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "Abilities.xml",
			RootName = kBListXmlParams.RootName
		};

		const string kXmlElementType = "Type";
		//const string kXmlElementAmmoCost = "AmmoCost";
		//const string kXmlElementObject = "Object";
		const string kXmlElementSquadMode = "SquadMode";
		//const string kXmlElementRecoverStart = "RecoverStart"; // BRecoverType
		//const string kXmlElementRecoverType = "RecoverType"; // BRecoverType
		//const string kXmlElementRecoverTime = "RecoverTime"; // float
		//const string kXmlElementMovementSpeedModifier = "MovementSpeedModifier"; // float
		//const string kXmlElementMovementModifierType = "MovementModifierType"; // BMovementModifierType
		const string kXmlElementDamageTakenModifier = "DamageTakenModifier";
		//const string kXmlElementDodgeModifier = "DodgeModifier"; // float
		//const string kXmlElementIcon = "Icon";
		//const string kXmlElementDisplayName2ID = "DisplayName2ID";
		const string kXmlElementTargetType = "TargetType";
		//const string kXmlElementRecoverAnimAttachment = "RecoverAnimAttachment"; // string, AttachmentType
		//const string kXmlElementRecoverStartAnim = "RecoverStartAnim"; // string
		//const string kXmlElementRecoverEndAnim = "RecoverEndAnim"; // string
		//const string kXmlElementSprinting = "Sprinting"; // bool
		//const string kXmlElementDontInterruptAttack = "DontInterruptAttack"; // bool
		//const string kXmlElementKeepSquadMode = "KeepSquadMode"; // bool
		//const string kXmlElementAttackSquadMode = "AttackSquadMode"; // bool
		const string kXmlElementDuration = "Duration";
		//const string kXmlElementSmartTargetRange = "SmartTargetRange"; // float
		//const string kXmlElementCanHeteroCommand = "CanHeteroCommand"; // bool
		//const string kXmlElementNoAbilityReticle = "NoAbilityReticle"; // bool
		#endregion

		BAbilityType mType = BAbilityType.Invalid;
		public BAbilityType Type { get { return mType; } }

		BSquadMode mSquadMode = BSquadMode.Invalid;
		public BSquadMode SquadMode { get { return mSquadMode; } }

		float mDamageTakenModifier = Util.kInvalidSingle;
		public float DamageTakenModifier { get { return mDamageTakenModifier; } }

		BAbilityTargetType mTargetType = BAbilityTargetType.Invalid;
		public BAbilityTargetType TargetType { get { return mTargetType; } }

		float mDuration = Util.kInvalidSingle;
		public float Duration { get { return mDuration; } }

		public BAbility()
		{
		}

		#region IXmlElementStreamable Members
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamElementOpt(mode, kXmlElementType, ref mType, e => e != BAbilityType.Invalid);
			s.StreamElementOpt(mode, kXmlElementSquadMode, ref mSquadMode, e => e != BSquadMode.Invalid);
			s.StreamElementOpt(mode, kXmlElementDamageTakenModifier, ref mDamageTakenModifier, Util.kNotInvalidPredicateSingle);
			s.StreamElementOpt(mode, kXmlElementTargetType, ref mTargetType, e => e != BAbilityTargetType.Invalid);
			s.StreamElementOpt(mode, kXmlElementDuration, ref mDuration, Util.kNotInvalidPredicateSingle);
		}
		#endregion
	};
}