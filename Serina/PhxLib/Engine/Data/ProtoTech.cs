using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using Interop = System.Runtime.InteropServices;
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
		Invalid = Util.kInvalidInt32,

		UnObtainable = 0,
		Obtainable,
		Available,
		Researching,
		Active,
		Disabled,
		CoopResearching,
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
		Invalid = Util.kInvalidInt32,
		None = 0,

		Age1, // not explicitly parsed by the engine
		Age2,
		Age3,
		Age4,
	};
	public enum BProtoTechEffectDisplayNameIconType
	{
		Unit,
		Building,
		Misc,
		Tech,
	};

	public struct BProtoTechPrereqTechStatus : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
		{
			ElementName = "TechStatus",
		};

		// Not actually parsed by the engine
		const string kXmlAttrOperator = "status";
		#endregion

		BProtoTechStatus mTechStatus;// = BProtoTechStatus.Invalid;
		int mTechID;// = Util.kInvalidInt32;

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrOperator, ref mTechStatus);
			xs.StreamXmlForDBID(s, mode, null, ref mTechID, DatabaseObjectKind.Object, false, XML.Util.kSourceCursor);
		}
		#endregion
	};
	// TODO: Nothing in HW uses this
	public struct BProtoTechPrereqTypeCount : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams
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
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			xs.StreamXmlForDBID(s, mode, kXmlAttrUnit, ref mUnitID, DatabaseObjectKind.Object, false, XML.Util.kSourceAttr);
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
			TechStatus = new Collections.BListArray<BProtoTechPrereqTechStatus>();
			TypeCounts = new Collections.BListArray<BProtoTechPrereqTypeCount>();
		}

		#region IPhxXmlStreamable Members
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			XML.Util.Serialize(s, mode, xs, TechStatus, BProtoTechPrereqTechStatus.kBListXmlParams);
			XML.Util.Serialize(s, mode, xs, TypeCounts, BProtoTechPrereqTypeCount.kBListXmlParams);
		}
		#endregion
	};
	public struct BProtoTechEffectTarget : IO.IPhxXmlStreamable
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Target")
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
		void StreamXmlValueID(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			DatabaseObjectKind kind = ObjectKind;

			if (kind != DatabaseObjectKind.None)
				xs.StreamXmlForDBID(s, mode, null, ref mValueID, kind, false, XML.Util.kSourceCursor);
		}
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrType, ref mType);
			StreamXmlValueID(s, mode, xs);
		}
		#endregion
	};
	// internal engine structure is only 0x34 bytes...
	public class BProtoTechEffect : IO.IPhxXmlStreamable
	{
		[Interop.StructLayout(Interop.LayoutKind.Explicit, Size = DataUnion.kSizeOf)]
		struct DataUnion
		{
			internal const int kSizeOf = 12;
			/// <summary>Offset of the first parameter</summary>
			const int kFirstParam = 4;
			/// <summary>Offset of the second parameter</summary>
			const int kSecondParam = 8;

			[Interop.FieldOffset(0)] public BObjectDataType SubType;
			[Interop.FieldOffset(kFirstParam)] public int ID;
			[Interop.FieldOffset(kSecondParam)] public int ID2;

			[Interop.FieldOffset(kFirstParam)] public int Cost_Type;
			[Interop.FieldOffset(kSecondParam)] public int Cost_UnitType; // proto object or type ID

			[Interop.FieldOffset(kFirstParam)] public BProtoObjectCommandType CommandType;
			[Interop.FieldOffset(kSecondParam)] public int CommandData;
			[Interop.FieldOffset(kSecondParam)] public BSquadMode CommandDataSM;

			[Interop.FieldOffset(kFirstParam)] public int DmgMod_WeapType;
			[Interop.FieldOffset(kSecondParam)] public int DmgMod_DmgType;

			[Interop.FieldOffset(kSecondParam)] public int TrainLimitType; // proto object or squad ID

			[Interop.FieldOffset(kFirstParam)] public int FromTypeID;
			[Interop.FieldOffset(kSecondParam)] public int ToTypeID;

			[Interop.FieldOffset(kFirstParam)] public BProtoTechEffectSetAgeLevel SetAgeLevel;

			public void Initialize()
			{
				SubType = BObjectDataType.Invalid;
				ID = ID2 = Util.kInvalidInt32;
			}

			public void StreamCost(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
			{
				xs.StreamXmlForTypeName(s, mode, kXmlAttrODT_Cost_Resource, ref Cost_Type, DatabaseTypeKind.Cost, false, XML.Util.kSourceAttr);
				xs.StreamXmlForDBID(s, mode, kXmlAttrODT_Cost_UnitType, ref Cost_UnitType, DatabaseObjectKind.Unit, true, XML.Util.kSourceAttr);
			}
			void StreamCommandData(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
			{
				const string attr_name = kXmlAttrODT_Command_Data;

				switch (CommandType)
				{
					case BProtoObjectCommandType.Research: // proto tech
						xs.StreamXmlForDBID(s, mode, attr_name, ref CommandData, DatabaseObjectKind.Tech, false, XML.Util.kSourceAttr);
						break;
					case BProtoObjectCommandType.TrainUnit: // proto object
					case BProtoObjectCommandType.Build:
					case BProtoObjectCommandType.BuildOther:
						xs.StreamXmlForDBID(s, mode, attr_name, ref CommandData, DatabaseObjectKind.Object, false, XML.Util.kSourceAttr);
						break;
					case BProtoObjectCommandType.TrainSquad: // proto squad
						xs.StreamXmlForDBID(s, mode, attr_name, ref CommandData, DatabaseObjectKind.Squad, false, XML.Util.kSourceAttr);
						break;

					case BProtoObjectCommandType.ChangeMode: // unused
						s.StreamAttribute(mode, attr_name, ref CommandDataSM);
						break;

					case BProtoObjectCommandType.Ability:
						xs.StreamXmlForDBID(s, mode, attr_name, ref CommandData, DatabaseObjectKind.Ability, false, XML.Util.kSourceAttr);
						break;
					case BProtoObjectCommandType.Power:
						xs.StreamXmlForDBID(s, mode, attr_name, ref CommandData, DatabaseObjectKind.Power, false, XML.Util.kSourceAttr);
						break;
				}
			}
			public void StreamCommand(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
			{
				if (s.StreamAttributeOpt(mode, kXmlAttrODT_Command_Type, ref CommandType, e => e != BProtoObjectCommandType.Invalid))
					StreamCommandData(s, mode, xs);
			}
			public void StreamDamageModifier(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
			{
				xs.StreamXmlForDBID(s, mode, kXmlAttrODT_DamageModifier_WeapType, ref DmgMod_WeapType, DatabaseObjectKind.WeaponType, false, XML.Util.kSourceAttr);
				xs.StreamXmlForDBID(s, mode, kXmlAttrODT_DamageModifier_DmgType, ref DmgMod_DmgType, DatabaseObjectKind.DamageType, false, XML.Util.kSourceAttr);
			}
			public void StreamTrainLimit(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs, DatabaseObjectKind kind)
			{
				if (kind == DatabaseObjectKind.Object)
					xs.StreamXmlForDBID(s, mode, kXmlAttrODT_TrainLimit_Unit, ref TrainLimitType, DatabaseObjectKind.Object, false, XML.Util.kSourceAttr);
				else if (kind == DatabaseObjectKind.Squad)
					xs.StreamXmlForDBID(s, mode, kXmlAttrODT_TrainLimit_Squad, ref TrainLimitType, DatabaseObjectKind.Squad, false, XML.Util.kSourceAttr);
			}
		};
		#region ID variants
		public int WeaponTypeID { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.DamageModifier);
			return mDU.DmgMod_WeapType;
		} }
		public int DamageTypeID { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.DamageModifier);
			return mDU.DmgMod_DmgType;
		} }

		public int RateID { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.RateAmount || SubType == BObjectDataType.RateMultiplier);
			return mDU.ID;
		} }

		public int PopID { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.PopCap || SubType == BObjectDataType.PopMax);
			return mDU.ID;
		} }

		public int PowerID { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.PowerRechargeTime || SubType == BObjectDataType.PowerUseLimit || SubType == BObjectDataType.PowerLevel);
			return mDU.ID;
		} }

		public int TransformUnitID { get {
			Contract.Requires(Type == BProtoTechEffectType.TransformUnit);
			return mDU.ToTypeID;
		} }
		public int TransformProtoFromID { get {
			Contract.Requires(Type == BProtoTechEffectType.TransformProtoUnit || Type == BProtoTechEffectType.TransformProtoSquad);
			return mDU.FromTypeID;
		} }
		public int TransformProtoToID { get {
			Contract.Requires(Type == BProtoTechEffectType.TransformProtoUnit || Type == BProtoTechEffectType.TransformProtoSquad);
			return mDU.ToTypeID;
		} }
		public int BuildObjectID { get {
			Contract.Requires(Type == BProtoTechEffectType.Build);
			return mDU.ToTypeID;
		} }
		public int GodPowerID { get {
			Contract.Requires(Type == BProtoTechEffectType.GodPower);
			return mDU.ID;
		} }
		public int TechStatusTechID { get {
			Contract.Requires(Type == BProtoTechEffectType.TechStatus);
			return mDU.ID;
		} }
		public int AbilityID { get {
			Contract.Requires(Type == BProtoTechEffectType.Ability);
			return mDU.ID;
		} }
		public int AttachSquadTypeObjectID { get {
			Contract.Requires(Type == BProtoTechEffectType.AttachSquad);
			return mDU.ID;
		} }
		#endregion

		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Effect")
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

		const string kXmlAttrODT_Rate = "Rate";

		const string kXmlAttrODT_Cost_Resource = "Resource";
		const string kXmlAttrODT_Cost_UnitType = "UnitType";

		const string kXmlAttrODT_Command_Type = "commandType";
		const string kXmlAttrODT_Command_Data = "CommandData";

		const string kXmlAttrODT_DamageModifier_WeapType = "WeaponType";
		const string kXmlAttrODT_DamageModifier_DmgType = "DamageType";

		const string kXmlAttrODT_Pop_PopType = "popType";

		const string kXmlAttrODT_TrainLimit_Unit = "unitType";
		const string kXmlAttrODT_TrainLimit_Squad = "squadType";

		const string kXmlAttrODT_Power_Power = "power";

		const string kXmlAttrODT_AbilityRecoverTime_Ability = "Ability";

		// TransformProtoUnit, TransformProtoSquad
		const string kXmlTransformProto_AttrFromType = "FromType";
		const string kXmlTransformProto_AttrToType = "ToType";

		const string kXmlAttachSquadAttrType = "squadType";
		#endregion

		BProtoTechEffectType mType;
		public BProtoTechEffectType Type { get { return mType; } }

		DataUnion mDU;

		#region ObjectData
		bool mAllActions;

		string mAction;

		public BObjectDataType SubType { get { return mDU.SubType; } }

		// Amount can be negative, so use NaN as the 'invalid' value instead
		float mAmount = Util.kInvalidSingleNaN;

		BObjectDataRelative mRelativity = BObjectDataRelative.Invalid;

		#region Command
		public BProtoObjectCommandType CommandType { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.CommandEnable || SubType == BObjectDataType.CommandSelectable);
			return mDU.CommandType;
		} }

		public int CommandDataID { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.CommandEnable || SubType == BObjectDataType.CommandSelectable);
			return mDU.CommandData;
		} }
		public BSquadMode CommandDataSquadMode { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.CommandEnable || SubType == BObjectDataType.CommandSelectable);
			return mDU.CommandDataSM;
		} }

		public DatabaseObjectKind CommandDataObjectKind { get {
			Contract.Requires(Type == BProtoTechEffectType.Data);
			Contract.Requires(SubType == BObjectDataType.CommandEnable || SubType == BObjectDataType.CommandSelectable);
			switch (mDU.CommandType)
			{
				case BProtoObjectCommandType.Research:		return DatabaseObjectKind.Tech;
				case BProtoObjectCommandType.TrainUnit:
				case BProtoObjectCommandType.Build:			return DatabaseObjectKind.Object;
				case BProtoObjectCommandType.TrainSquad:
				case BProtoObjectCommandType.BuildOther:	return DatabaseObjectKind.Squad;
				case BProtoObjectCommandType.Ability:		return DatabaseObjectKind.Ability;
				case BProtoObjectCommandType.Power:			return DatabaseObjectKind.Power;

				default: throw new Debug.UnreachableException(mDU.CommandType.ToString());
			}
		} }
		#endregion
		#endregion

		public BProtoTechEffectSetAgeLevel SetAgeLevel { get { return mDU.SetAgeLevel; } }

		public Collections.BListArray<BProtoTechEffectTarget> Targets { get; private set; }
		public bool HasTargets { get { return Targets != null || Targets.Count != 0; } }

		public BProtoTechEffect()
		{
			mDU.Initialize();
		}

		#region IXmlElementStreamable Members
		DatabaseObjectKind TransformProtoObjectKind { get {
			return mType == BProtoTechEffectType.TransformProtoUnit ? DatabaseObjectKind.Object : DatabaseObjectKind.Squad;
		} }

		void StreamXmlTargets(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			if (mode == FA.Read) Targets = new Collections.BListArray<BProtoTechEffectTarget>();

			XML.Util.Serialize(s, mode, xs, Targets, BProtoTechEffectTarget.kBListXmlParams);
		}
		void StreamXmlObjectData(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			switch (mDU.SubType)
			{
				#region Unused
				case BObjectDataType.RateAmount:
				case BObjectDataType.RateMultiplier:
					xs.StreamXmlForTypeName(s, mode, kXmlAttrODT_Rate, ref mDU.ID, DatabaseTypeKind.Rate, false, XML.Util.kSourceAttr);
					break;
				#endregion

				case BObjectDataType.CommandEnable:
				case BObjectDataType.CommandSelectable: // Unused
					mDU.StreamCommand(s, mode, xs);
					break;

				case BObjectDataType.Cost:
					mDU.StreamCost(s, mode, xs);
					break;

				#region Unused
				case BObjectDataType.DamageModifier:
					mDU.StreamDamageModifier(s, mode, xs);
					break;
				#endregion

				case BObjectDataType.PopCap:
				case BObjectDataType.PopMax:
					xs.StreamXmlForTypeName(s, mode, kXmlAttrODT_Pop_PopType, ref mDU.ID, DatabaseTypeKind.Pop, false, XML.Util.kSourceAttr);
					break;

				#region Unused
				case BObjectDataType.UnitTrainLimit:
					mDU.StreamTrainLimit(s, mode, xs, DatabaseObjectKind.Object);
					break;
				case BObjectDataType.SquadTrainLimit:
					mDU.StreamTrainLimit(s, mode, xs, DatabaseObjectKind.Squad);
					break;
				#endregion

				case BObjectDataType.PowerRechargeTime:
				case BObjectDataType.PowerUseLimit:
				case BObjectDataType.PowerLevel:
					xs.StreamXmlForDBID(s, mode, kXmlAttrODT_Power_Power, ref mDU.ID, DatabaseObjectKind.Power, false, XML.Util.kSourceAttr);
					break;

				#region Ignored
 				case BObjectDataType.ImpactEffect:
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
					xs.StreamXmlForDBID(s, mode, kXmlAttrODT_AbilityRecoverTime_Ability, ref mDU.ID, DatabaseObjectKind.Ability, false, XML.Util.kSourceAttr);
					break;

				#region Ignored
				case BObjectDataType.HPBar:
					break;
				#endregion
				#region Unused
// 				case BObjectDataType.DeathSpawn:
// 					break;
				#endregion

				// assume everything else (sans ignored/unused) only uses amount
				default: //throw new Debug.UnreachableException(mSubType.ToString());
					break;
			}
		}
		public void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			s.StreamAttribute(mode, kXmlAttrType, ref mType);

			bool stream_targets = false;
			switch (mType)
			{
				case BProtoTechEffectType.Data:
					s.StreamAttributeOpt(mode, kXmlAttrAllActions, ref mAllActions, Util.kNotFalsePredicate);
					XML.Util.StreamInternStringOpt(s, mode, kXmlAttrAction, ref mAction, false);
					s.StreamAttribute(mode, kXmlAttrSubType, ref mDU.SubType);
					// e.g., SubType==Icon and these won't be used...TODO: is Icon the only one?
					s.StreamAttributeOpt(mode, kXmlAttrAmount, ref mAmount, Util.kNotInvalidPredicateSingleNaN);
					s.StreamAttributeOpt(mode, kXmlAttrRelativity, ref mRelativity, x => x != BObjectDataRelative.Invalid);
					StreamXmlObjectData(s, mode, xs);
					stream_targets = true;
					break;
				case BProtoTechEffectType.TransformUnit:
				case BProtoTechEffectType.Build:
					xs.StreamXmlForDBID(s, mode, null, ref mDU.ToTypeID, DatabaseObjectKind.Object, false, XML.Util.kSourceCursor);
					break;
				case BProtoTechEffectType.TransformProtoUnit:
				case BProtoTechEffectType.TransformProtoSquad:
					xs.StreamXmlForDBID(s, mode, kXmlTransformProto_AttrFromType, ref mDU.FromTypeID, TransformProtoObjectKind, false, XML.Util.kSourceAttr);
					xs.StreamXmlForDBID(s, mode, kXmlTransformProto_AttrToType, ref mDU.ToTypeID, TransformProtoObjectKind, false, XML.Util.kSourceAttr);
					break;
				#region Unused
				case BProtoTechEffectType.SetAge:
					s.StreamCursor(mode, ref mDU.SetAgeLevel);
					break;
				#endregion
				case BProtoTechEffectType.GodPower:
					xs.StreamXmlForDBID(s, mode, null, ref mDU.ID, DatabaseObjectKind.Power, false, XML.Util.kSourceCursor);
					s.StreamAttribute(mode, kXmlAttrAmount, ref mAmount);
					break;
				#region Unused
				case BProtoTechEffectType.TechStatus:
					xs.StreamXmlForDBID(s, mode, null, ref mDU.ID, DatabaseObjectKind.Tech, false, XML.Util.kSourceCursor);
					break;
				case BProtoTechEffectType.Ability:
					xs.StreamXmlForDBID(s, mode, null, ref mDU.ID, DatabaseObjectKind.Ability, false, XML.Util.kSourceCursor);
					break;
 				case BProtoTechEffectType.SharedLOS: // no extra parsed data
 					break;
				case BProtoTechEffectType.AttachSquad:
					xs.StreamXmlForDBID(s, mode, kXmlAttachSquadAttrType, ref mDU.ID, TransformProtoObjectKind, false, XML.Util.kSourceAttr);
					stream_targets = true;
					break;
				#endregion
			}

			if (stream_targets) StreamXmlTargets(s, mode, xs);
		}
		#endregion
	};
	public class BProtoTech : DatabaseIdObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Tech")
		{
			RootName = "TechTree",
			DataName = "name",
			Flags = //XML.BCollectionXmlParamsFlags.ToLowerDataNames |
				XML.BCollectionXmlParamsFlags.RequiresDataNamePreloading |
				XML.BCollectionXmlParamsFlags.SupportsUpdating
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Game,
			Directory = GameDirectory.Data,
			FileName = "Techs.xml",
			RootName = kBListXmlParams.RootName
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfoUpdate = new PhxEngine.XmlFileInfo
		{
			Location = ContentStorage.Update,
			Directory = GameDirectory.Data,
			FileName = "Techs_Update.xml",
			RootName = kBListXmlParams.RootName
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

		public BProtoTech() : base(BResource.kBListTypeValuesParams, BResource.kBListTypeValuesXmlParams_CostLowercaseType)
		{
			Effects = new Collections.BListArray<BProtoTechEffect>();
		}

		#region IXmlElementStreamable Members
		protected override void StreamXmlDbId(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
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
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BXmlSerializerInterface xs)
		{
			base.StreamXml(s, mode, xs);

			s.StreamElement(mode, kXmlElementStatus, ref mStatus);

			if (ShouldStreamPrereqs(s, mode))
				Prereqs.StreamXml(s, mode, xs);

			XML.Util.Serialize(s, mode, xs, Effects, BProtoTechEffect.kBListXmlParams);
		}
		#endregion
	};
}