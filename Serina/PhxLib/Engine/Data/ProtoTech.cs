using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;
using XmlIgnore = System.Xml.Serialization.XmlIgnoreAttribute;

namespace PhxLib.Engine
{
	public enum BProtoTechFlags
	{
		// 0x1C
		NoSound,// = 1<<0,
		[XmlIgnore] Forbid,// = 1<<1,
		Perpetual,// = 1<<2,
		OrPrereqs,// = 1<<3,
		Shadow,// = 1<<4,
		/// <summary>Tech applies to a unique, ie specific, unit</summary>
		UniqueProtoUnitInstance,// = 1<<5,
		Unobtainable,// = 1<<6,
		[XmlIgnore] OwnStaticData,// = 1<<7,

		// 0x1D
		Instant,// = 1<<7,

		// 0x78
		HiddenFromStats,// = 1<<0, // actually just appears to be a bool field
	};

	public enum BProtoTechStatus
	{
		UnObtainable,
		Obtainable,
		Available,
		Researching,
		Active,
		Disabled,
		CoopResearching,
	};

	public enum BProtoTechEffectType
	{
		Data,
		TransformUnit,
		TransformProtoUnit,
		TransformProtoSquad,
		Build,
		SetAge,
		GodPower,
		TechStatus,
		Ability,
		SharedLOS,
		AttachSquad,
	};
	public enum BProtoTechEffectTargetType
	{
		ProtoUnit,
		ProtoSquad,
		Unit,
		Tech,
		TechAll,
		Player,
	};

	public struct BProtoTechPrereq : IO.IPhxXmlStreamable
	{
		#region Xml constants
		const string kXmlRootName = "Prereqs";
		const string kXmlElementTechStatus = "TechStatus";

		// TODO: Nothing in HW uses this, so I'm not implementing this
		const string kXmlElementTypeCount = "TypeCount";
		const string kXmlElementTypeCountAttrUnit = "unit";
		//count
		//operator, gt, lt

		const string kXmlElement = "";
		#endregion

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			throw new NotImplementedException();
		}
		#endregion
	};
	public struct BProtoTechEffectTarget : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Target")
		{
			RootName = null,
			Flags = 0
		};

		const string kXmlAttrType = "type";
		#endregion

		BProtoTechEffectTargetType mType;
		public BProtoTechEffectTargetType Type { get { return mType; } }
		int mValueID;
		public int ValueID { get { return mValueID; } }

		public DatabaseObjectKind ObjectKind { get {
			switch (mType)
			{
				case BProtoTechEffectTargetType.ProtoUnit: return DatabaseObjectKind.Unit;
				case BProtoTechEffectTargetType.ProtoSquad: return DatabaseObjectKind.Squad;
				case BProtoTechEffectTargetType.Tech: return DatabaseObjectKind.Tech;

				default: return DatabaseObjectKind.None;
			}
		} }

		#region IXmlElementStreamable Members
		void StreamXmlValueID(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			DatabaseObjectKind kind = ObjectKind;

			if (kind != DatabaseObjectKind.None)
				db.StreamXmlForDBID(s, mode, null, ref mValueID, kind, false, Util.kSourceCursor);
		}
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttribute(mode, kXmlAttrType, ref mType);
			StreamXmlValueID(s, mode, db);
		}
		#endregion
	};
	// internal engine structure is only 0x34 bytes...
	public class BProtoTechEffect : IO.IPhxXmlStreamable
	{
		enum SetAgeLevel
		{
			Age2 = 2,
			Age3,
			Age4,
		};

		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Effect")
		{
			Flags = Collections.BCollectionParamsFlags.RequiresDataNamePreloading
		};

		const string kXmlAttrType = "type";
		/// <remarks></remarks>
		const string kXmlAttrAllActions = "allactions";
		const string kXmlAttrAction = "action";
		const string kXmlAttrSubType = "subtype";
		const string kXmlAttrAmount = "amount";
		const string kXmlAttrRelativity = "relativity";

		const string kXmlAttrODT_Command_Type = "commandType";
		const string kXmlAttrODT_Command_Data = "CommandData";

		const string kXmlAttrODT_Pop_PopType = "popType";
		const string kXmlAttrODT_Power_Power = "Power";

		const string kXmlAttrODT_AbilityRecoverTime_Ability = "Ability";

		const string kXmlAttrODT_Cost_Resource = "Resource";
		const string kXmlAttrODT_Cost_UnitType = "UnitType";

		// TransformProtoUnit, TransformProtoSquad
		const string kXmlTransformProto_AttrFromType = "FromType";
		const string kXmlTransformProto_AttrToType = "ToType";
		#endregion

		BProtoTechEffectType mType;
		public BProtoTechEffectType Type { get { return mType; } }

		bool mAllActions;

		string mAction;

		BObjectDataType mSubType;

		// Amount can be negative, so use NaN as the 'invalid' value instead
		float mAmount = Util.kInvalidSingleNaN;

		BObjectDataRelative mRelativity = BObjectDataRelative.Invalid;

		int mPowerID = Util.kInvalidInt32;

		public Collections.BListArray<BProtoTechEffectTarget> Targets { get; private set; }
		public bool HasTargets { get { return Targets != null || Targets.Count != 0; } }

		public BProtoTechEffect()
		{
		}

		#region IXmlElementStreamable Members
		void StreamXmlTargets(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			if (mode == FA.Read) Targets = new Collections.BListArray<BProtoTechEffectTarget>(BProtoTechEffectTarget.kBListParams);

			Targets.StreamXml(s, mode, db);
		}
		void StreamXmlObjectData(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			switch (mSubType)
			{
				case BObjectDataType.CommandEnable:
				case BObjectDataType.CommandSelectable:
					break;

				#region Unused
// 				case BObjectDataType.RateAmount:
// 				case BObjectDataType.RateMultiplier:
// 					break;
// 
// 				case BObjectDataType.DamageModifier:
// 					break;
				#endregion

				case BObjectDataType.PopCap:
				case BObjectDataType.PopMax:
					// TODO: kXmlAttrODT_Pop_PopType
					break;

				#region Unused
// 				case BObjectDataType.UnitTrainLimit:
// 					break;
// 
// 				case BObjectDataType.SquadTrainLimit:
// 					break;
				#endregion

				case BObjectDataType.PowerRechargeTime:
				case BObjectDataType.PowerUseLimit:
				case BObjectDataType.PowerLevel:
					{
						// TODO: kXmlAttrODT_Power_Power mProtoPowerID
					}
					break;

				#region Ignored
 				case BObjectDataType.ImpactEffect:
 					break;

 				case BObjectDataType.AmmoRegenRate:
 					break;
				#endregion
				#region Unused
// 				case BObjectDataType.DisplayNameID:
// 					break;
				#endregion
				#region Ignored
 				case BObjectDataType.TurretYawRate:
 				case BObjectDataType.TurretPitchRate:
 					break;
				#endregion

				case BObjectDataType.AbilityRecoverTime:
					break;

				#region Ignored
 				case BObjectDataType.HPBar:
 					break;
				#endregion
				#region Unused
// 				case BObjectDataType.DeathSpawn:
// 					break;
				#endregion

				case BObjectDataType.Cost:
					break;

				// assume everything else (sans ignored/unused) only uses amount
				default: //throw new Debug.UnreachableException(mSubType.ToString());
					break;
			}
		}
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttribute(mode, kXmlAttrType, ref mType);

			bool stream_targets = false;
			switch (mType)
			{
				case BProtoTechEffectType.Data:
					s.StreamAttributeOpt(mode, kXmlAttrAllActions, ref mAllActions, Util.kNotFalsePredicate);
					s.StreamAttributeOpt(mode, kXmlAttrAction, ref mAction, Util.kNotNullOrEmpty);
					s.StreamAttribute(mode, kXmlAttrSubType, ref mSubType);
					// e.g., SubType==Icon and these won't be used...TODO: is Icon the only one?
					s.StreamAttributeOpt(mode, kXmlAttrAmount, ref mAmount, Util.kNotInvalidPredicateSingleNaN);
					s.StreamAttributeOpt(mode, kXmlAttrRelativity, ref mRelativity, x => x != BObjectDataRelative.Invalid);
					StreamXmlObjectData(s, mode, db);
					stream_targets = true;
					break;
				case BProtoTechEffectType.TransformUnit:
				case BProtoTechEffectType.Build:
					// TODO: cursor proto object
					break;
				case BProtoTechEffectType.TransformProtoUnit:
					// TODO: proto objects
					break;
				case BProtoTechEffectType.TransformProtoSquad:
					// TODO: proto squads
					break;
				#region Unused
// 				case BProtoTechEffectType.SetAge:
//					// TODO: cursor level = SetAgeLevel
// 					break;
				#endregion
				case BProtoTechEffectType.GodPower:
					// TODO: cursor proto power
					s.StreamAttribute(mode, kXmlAttrAmount, ref mAmount);
					break;
				#region Unused
// 				case BProtoTechEffectType.TechStatus:
// 					// TODO: cursor proto tech
// 					break;
// 				case BProtoTechEffectType.Ability:
// 					break;
// 				case BProtoTechEffectType.SharedLOS:
// 					break;
// 				case BProtoTechEffectType.AttachSquad:
//					stream_targets = true;
// 					break;
				#endregion
			}

			if (stream_targets) StreamXmlTargets(s, mode, db);
		}
		#endregion
	};
	public class BProtoTech : DatabaseIdObject
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Tech")
		{
			RootName = "TechTree",
			DataName = "name",
			Flags = Collections.BCollectionParamsFlags.ToLowerDataNames |
				Collections.BCollectionParamsFlags.RequiresDataNamePreloading |
				Collections.BCollectionParamsFlags.SupportsUpdating
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Game,
			Directory = GameDirectory.Data,
			FileName = "Techs.xml",
			RootName = kBListParams.RootName
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfoUpdate = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Update,
			Directory = GameDirectory.Data,
			FileName = "Techs_Update.xml",
			RootName = kBListParams.RootName
		};

		const string kXmlElementFlag = "Flag";
		/// <remarks>This isn't always used, nor unique</remarks>
		const string kXmlElementDbId = "DBID";

		const string kXmlElementStatus = "Status";
		const string kXmlElementIcon = "Icon";
		#endregion

		BProtoTechStatus mStatus;

		public Collections.BListArray<BProtoTechEffect> Effects { get; private set; }

		public BProtoTech() : base(BResource.kBListTypeValuesParams_CostLowercaseType)
		{
			Effects = new Collections.BListArray<BProtoTechEffect>(BProtoTechEffect.kBListParams);
		}

		#region IXmlElementStreamable Members
		protected override void StreamXmlDbId(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamElementOpt(mode, kXmlElementDbId, KSoft.NumeralBase.Decimal, ref mDbId, Util.kNotInvalidPredicate);
		}

		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			base.StreamXml(s, mode, db);

			s.StreamElement(mode, kXmlElementStatus, ref mStatus);

			Effects.StreamXml(s, mode, db);
		}
		#endregion
	};
}