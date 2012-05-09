using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

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

		Invalid,
	};
	public enum BProtoTechTypeCountOperator : short
	{
		None, // '0' isn't explicitly parsed
		gt,
		lt,
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
	public enum BProtoTechEffectSetAgeLevel
	{
		None,

		Age1, // not explicitly parsed by the engine
		Age2,
		Age3,
		Age4,
	};

	public struct BProtoTechPrereqTechStatus : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "TechStatus",
		};

		// Not actually parsed by the engine
		const string kXmlAttrOperator = "status";
		#endregion

		BProtoTechStatus mTechStatus;// = BProtoTechStatus.Invalid;
		int mTechID;// = Util.kInvalidInt32;

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			s.StreamAttribute(mode, kXmlAttrOperator, ref mTechStatus);
			db.StreamXmlForDBID(s, mode, null, ref mTechID, DatabaseObjectKind.Object, false, Util.kSourceCursor);
		}
		#endregion
	};
	// TODO: Nothing in HW uses this
	public struct BProtoTechPrereqTypeCount : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams
		{
			ElementName = "TypeCount",
		};

		const string kXmlAttrUnit = "unit"; // ProtoObject
		const string kXmlAttrOperator = "operator";
		const string kXmlAttrCount = "count";
		#endregion

		int mUnitID;
		BProtoTechTypeCountOperator mOperator;
		short mCount;

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			db.StreamXmlForDBID(s, mode, kXmlAttrUnit, ref mUnitID, DatabaseObjectKind.Object, false, Util.kSourceAttr);
			if (!s.StreamAttributeOpt(mode, kXmlAttrOperator, ref mOperator, e => e != BProtoTechTypeCountOperator.None))
				mOperator = BProtoTechTypeCountOperator.None;
			if (!s.StreamAttributeOpt(mode, kXmlAttrCount, KSoft.NumeralBase.Decimal, ref mCount, Util.kNotInvalidPredicateInt16))
				mCount = Util.kInvalidInt32;
		}
		#endregion
	};
	public class BProtoTechPrereqs : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public const string kXmlRootName = "Prereqs";
		#endregion

		public Collections.BListArray<BProtoTechPrereqTechStatus> TechStatus { get; private set; }
		public Collections.BListArray<BProtoTechPrereqTypeCount> TypeCounts { get; private set; }

		public BProtoTechPrereqs()
		{
			TechStatus = new Collections.BListArray<BProtoTechPrereqTechStatus>(BProtoTechPrereqTechStatus.kBListParams);
			TypeCounts = new Collections.BListArray<BProtoTechPrereqTypeCount>(BProtoTechPrereqTypeCount.kBListParams);
		}

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			TechStatus.StreamXml(s, mode, db);
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
		#region Xml constants
		public static readonly Collections.BListParams kBListParams = new Collections.BListParams("Effect")
		{
			Flags = 0
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

		const string kXmlAttachSquadAttrType = "squadType";
		#endregion

		BProtoTechEffectType mType;
		public BProtoTechEffectType Type { get { return mType; } }

		#region ObjectData
		bool mAllActions;

		string mAction;

		BObjectDataType mSubType;

		// Amount can be negative, so use NaN as the 'invalid' value instead
		float mAmount = Util.kInvalidSingleNaN;

		BObjectDataRelative mRelativity = BObjectDataRelative.Invalid;

		BProtoObjectCommandType mCommandType = BProtoObjectCommandType.Invalid;
		#endregion

		#region ID variants
		int mID = Util.kInvalidInt32;

		public int TransformUnitID { get {
			Contract.Requires(Type == BProtoTechEffectType.TransformUnit);
			return mID;
		} }
		public int TransformProtoToID { get {
			Contract.Requires(Type == BProtoTechEffectType.TransformProtoUnit || Type == BProtoTechEffectType.TransformProtoSquad);
			return mID;
		} }
		public int BuildObjectID { get {
			Contract.Requires(Type == BProtoTechEffectType.Build);
			return mID;
		} }
		public int GodPowerID { get {
			Contract.Requires(Type == BProtoTechEffectType.GodPower);
			return mID;
		} }
		public int TechStatusTechID { get {
			Contract.Requires(Type == BProtoTechEffectType.TechStatus);
			return mID;
		} }
		public int AbilityID { get {
			Contract.Requires(Type == BProtoTechEffectType.Ability);
			return mID;
		} }
		public int AttachSquadTypeObjectID { get {
			Contract.Requires(Type == BProtoTechEffectType.AttachSquad);
			return mID;
		} }
		#endregion

		#region TransformProto*
		int mTransformProtoFromID;
		public int TransformProtoFromID { get {
			Contract.Requires(Type == BProtoTechEffectType.TransformProtoUnit || Type == BProtoTechEffectType.TransformProtoSquad);
			return mTransformProtoFromID;
		} }
		#endregion
		#region SetAgeLevel
		BProtoTechEffectSetAgeLevel mSetAgeLevel = BProtoTechEffectSetAgeLevel.None;
		public BProtoTechEffectSetAgeLevel SetAgeLevel { get { return mSetAgeLevel; } }
		#endregion

		public Collections.BListArray<BProtoTechEffectTarget> Targets { get; private set; }
		public bool HasTargets { get { return Targets != null || Targets.Count != 0; } }

		#region IXmlElementStreamable Members
		DatabaseObjectKind CommandDataObjectKind { get {
			return mType == BProtoTechEffectType.TransformProtoUnit ? DatabaseObjectKind.Object : DatabaseObjectKind.Squad;
		} }
		DatabaseObjectKind TransformProtoObjectKind { get {
			return mType == BProtoTechEffectType.TransformProtoUnit ? DatabaseObjectKind.Object : DatabaseObjectKind.Squad;
		} }

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
				case BObjectDataType.CommandSelectable: // Unused
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
					db.StreamXmlForDBID(s, mode, null, ref mID, DatabaseObjectKind.Object, false, Util.kSourceCursor);
					break;
				case BProtoTechEffectType.TransformProtoUnit:
				case BProtoTechEffectType.TransformProtoSquad:
					db.StreamXmlForDBID(s, mode, kXmlTransformProto_AttrFromType, ref mTransformProtoFromID, TransformProtoObjectKind, false, Util.kSourceAttr);
					db.StreamXmlForDBID(s, mode, kXmlTransformProto_AttrToType, ref mID, TransformProtoObjectKind, false, Util.kSourceAttr);
					break;
				#region Unused
				case BProtoTechEffectType.SetAge:
					s.StreamCursor(mode, ref mSetAgeLevel);
					break;
				#endregion
				case BProtoTechEffectType.GodPower:
					db.StreamXmlForDBID(s, mode, null, ref mID, DatabaseObjectKind.Power, false, Util.kSourceCursor);
					s.StreamAttribute(mode, kXmlAttrAmount, ref mAmount);
					break;
				#region Unused
				case BProtoTechEffectType.TechStatus:
					db.StreamXmlForDBID(s, mode, null, ref mID, DatabaseObjectKind.Tech, false, Util.kSourceCursor);
					break;
				case BProtoTechEffectType.Ability:
					db.StreamXmlForDBID(s, mode, null, ref mID, DatabaseObjectKind.Ability, false, Util.kSourceCursor);
					break;
// 				case BProtoTechEffectType.SharedLOS:
// 					break;
				case BProtoTechEffectType.AttachSquad:
					db.StreamXmlForDBID(s, mode, kXmlAttachSquadAttrType, ref mID, TransformProtoObjectKind, false, Util.kSourceAttr);
					stream_targets = true;
					break;
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

		const string kXmlElementStatsObject = "StatsObject"; // ProtoObject
		#endregion

		BProtoTechStatus mStatus;

		public BProtoTechPrereqs Prereqs { get; private set; }
		public bool HasPrereqs { get { return Prereqs != null; } }

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

		bool ShouldStreamPrereqs(KSoft.IO.XmlElementStream s, FA mode)
		{
			if (mode == FA.Read)
			{
				bool has_prereqs = s.ElementsExists(BProtoTechPrereqs.kXmlRootName);

				if (has_prereqs) Prereqs = new BProtoTechPrereqs();
				return has_prereqs;
			}
			else if (mode == FA.Write) return HasPrereqs;

			return false;
		}
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, BDatabaseBase db)
		{
			base.StreamXml(s, mode, db);

			s.StreamElement(mode, kXmlElementStatus, ref mStatus);

			if (ShouldStreamPrereqs(s, mode))
				Prereqs.StreamXml(s, mode, db);

			Effects.StreamXml(s, mode, db);
		}
		#endregion
	};
}