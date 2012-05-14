using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BPowerType
	{
		None,

		Cleansing,
		Orbital,
		CarpetBombing,
		Cryo,
		Rage,
		Wave,
		Disruption,
		Transport,
		ODST,
		Repair,
	};
	public enum BPowerFlags
	{
		// 0xCC
		SequentialRecharge,// = 1<<0,
		LeaderPower,// = 1<<1,
		ShowTargetHighlight,// = 1<<2,
		ShowLimit,// = 1<<3,
		MultiRechargePower,// = 1<<4,
		UnitPower,// = 1<<5,
		InfiniteUses,// = 1<<6,

		// 0xCD, camera flags
		CameraEnableUserYaw,// = 1<<0,
		CameraEnableUserZoom,// = 1<<1,
		CameraEnableUserScroll,// = 1<<2,
		
		// 0xCE
		NotDisruptable,// = 1<<4, // actually "Disruptable" in code
	};
	public enum BMinigameType
	{
		None,

		OneButtonPress,
		TwoButtonPress,
		ThreeButtonPress,
	};

	public class BProtoPower : DatabaseNamedObject
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Power")
		{
			DataName = DatabaseNamedObject.kXmlAttrName,
			Flags = 0
		};
		public static readonly PhxEngine.XmlFileInfo kXmlFileInfo = new PhxEngine.XmlFileInfo
		{
			Directory = GameDirectory.Data,
			FileName = "Powers.xml",
			RootName = kBListXmlParams.RootName
		};

		const string kXmlElementAttributes = "Attributes";

		// can have multiple dynamic costs
//		const string kXmlElementDynamicCost = "DynamicCost"; // inner text is a float (the actual cost)
//		const string kXmlElementDynamicCostAttrObjectType = "ObjectType"; // GameObjectTypes
		// can have multiple
//		const string kXmlElementTargetEffectiveness = "TargetEffectiveness"; // inner text is an integer
//		const string kXmlElementTargetEffectivenessAttrObjectType = "ObjectType"; // GameObjectTypes
//		const string kXmlElementUIRadius = "UIRadius"; // float
		const string kXmlElementPowerType = "PowerType";
		const string kXmlElementAutoRecharge = "AutoRecharge";
		const string kXmlElementUseLimit = "UseLimit"; // integer
//		const string kXmlElementIcon = "Icon";
		// can have multiple
//		const string kXmlElementIconLocation = "IconLocation"; // string id
		// can have multiple
		const string kXmlElementTechPrereq = "TechPrereq"; // proto tech
//		const string kXmlElementChooseTextID = "ChooseTextID";
//		const string kXmlElementAction = "Action"; // BActionType
//		const string kXmlElementMinigame = "Minigame"; // BMinigameType
//		const string kXmlElementCameraZoomMin = "CameraZoomMin"; // float
//		const string kXmlElementCameraZoomMax = "CameraZoomMax"; // float
//		const string kXmlElementCameraPitchMin = "CameraPitchMin"; // float
//		const string kXmlElementCameraPitchMax = "CameraPitchMax"; // float
//		const string kXmlElementCameraEffectIn = "CameraEffectIn"; // camera effect name
//		const string kXmlElementCameraEffectOut = "CameraEffectOut"; // camera effect name
//		const string kXmlElementMinDistanceToSquad = "MinDistanceToSquad"; // float
//		const string kXmlElementMaxDistanceToSquad = "MaxDistanceToSquad"; // float
//		const string kXmlElementCameraEnableUserScroll = "CameraEnableUserScroll"; // bool
//		const string kXmlElementCameraEnableUserYaw = "CameraEnableUserYaw"; // bool
//		const string kXmlElementCameraEnableUserZoom = "CameraEnableUserZoom"; // bool
//		const string kXmlElementCameraEnableAutoZoomInstant = "CameraEnableAutoZoomInstant"; // bool
//		const string kXmlElementCameraEnableAutoZoom = "CameraEnableAutoZoom"; // bool
//		const string kXmlElementShowTargetHighlight = "ShowTargetHighlight";
//		const string kXmlElementShowTargetHighlightAttrObjectType = "ObjectType"; // GameObjectTypes
//		const string kXmlElementShowTargetHighlightAttrRelation = "Relation"; // BDiplomacy
//		const string kXmlElementShowInPowerMenu = "ShowInPowerMenu"; // bool
//		const string kXmlElementShowTransportArrows = "ShowTransportArrows"; // bool
//		const string kXmlElementChildObjects = "ChildObjects";
//		const string kXmlElementChildObjectsObject = "Object"; // proto object name
		const string kXmlElementDataLevel = "DataLevel";
		const string kXmlElementBaseDataLevel = "BaseDataLevel";

		const string kXmlElementTriggerScript = "TriggerScript";
		const string kXmlElementCommandTriggerScript = "CommandTriggerScript";

		class BCostTypeValuesSingleAttrHackXmlSerializer : XML.BListExplicitIndexXmlSerializerBase<float>
		{
			// Just an alias for less typing and code
			static readonly XML.BTypeValuesXmlParams<float> kParams = BResource.kBListTypeValuesXmlParams_Cost;

			Collections.BTypeValuesSingle mList;

			public override Collections.BListExplicitIndexBase<float> ListExplicitIndex { get { return mList; } }

			public BCostTypeValuesSingleAttrHackXmlSerializer(Collections.BTypeValuesSingle list) : base(kParams)
			{
				mList = list;
			}

			#region IXmlElementStreamable Members
			protected override void ReadXml(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs, int iteration) { throw new NotImplementedException(); }
			protected override void WriteXml(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs, float data) { throw new NotImplementedException(); }
			protected override int ReadExplicitIndex(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs) { throw new NotImplementedException(); }
			protected override void WriteExplicitIndex(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs, int index) { throw new NotImplementedException(); }

			protected override void ReadXmlNodes(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs)
			{
				var penum = mList.TypeValuesParams.kGetProtoEnumFromDB(xs.Database);

				foreach (XmlAttribute attr in s.Cursor.Attributes)
				{
					// The only attributes in this are actual member names so we don't waste time calling
					// penum.IsValidMemberName only to call GetMemberId when we can just compare id to -1
					int index = penum.GetMemberId(attr.Name);
					if (index == Util.kInvalidInt32) continue;

					mList.InitializeItem(index);
					float value = Util.kInvalidSingle;
					s.ReadAttribute(attr.Name, ref value);
					mList[index] = value;
				}
			}
			protected override void WriteXmlNodes(KSoft.IO.XmlElementStream s, XML.BDatabaseXmlSerializerBase xs)
			{
				var tvp = mList.TypeValuesParams;

				var penum = tvp.kGetProtoEnumFromDB(xs.Database);
				float k_invalid = tvp.kTypeGetInvalid();

				for (int x = 0; x < mList.Count; x++)
				{
					float data = mList[x];

					if (tvp.kComparer.Compare(data, k_invalid) != 0)
					{
						string name = penum.GetMemberName(x);
						s.WriteAttribute(name, data);
					}
				}
			}
			#endregion

			public static void Serialize(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase db,
				Collections.BTypeValuesSingle list)
			{
				Contract.Requires(s != null);
				Contract.Requires(db != null);
				Contract.Requires(list != null);

				var xs = new BCostTypeValuesSingleAttrHackXmlSerializer(list);
				{
					xs.StreamXml(s, mode, db);
				}
			}
		};
		#endregion

		public Collections.BTypeValuesSingle Cost { get; private set; }

		public Collections.BTypeValuesSingle Populations { get; private set; }

		BPowerType mPowerType = BPowerType.None;
		float mAutoRecharge = Util.kInvalidSingle;
		int mUseLimit = Util.kInvalidInt32;
		BPowerFlags mFlags;

		string mTriggerScript, mCommandTriggerScript;

		public BProtoPower()
		{
			Cost = new Collections.BTypeValuesSingle(BResource.kBListTypeValuesParams);

			Populations = new Collections.BTypeValuesSingle(BPopulation.kBListParamsSingle);
		}

		#region IXmlElementStreamable Members
		void StreamXmlFlags(KSoft.IO.XmlElementStream s, FA mode)
		{
		}
		public override void StreamXml(KSoft.IO.XmlElementStream s, FA mode, XML.BDatabaseXmlSerializerBase xs)
		{
			using (s.EnterCursorBookmark(mode, kXmlElementAttributes))
			{
				base.StreamXml(s, mode, xs);

				BCostTypeValuesSingleAttrHackXmlSerializer.Serialize(s, mode, xs, Cost);
				// DynamicCost
				// TargetEffectiveness
				XML.Util.Serialize(s, mode, xs, Populations, BPopulation.kBListXmlParamsSingle_LowerCase);

				s.StreamElementOpt(mode, kXmlElementPowerType, ref mPowerType, e => e != BPowerType.None);
				s.StreamElementOpt(mode, kXmlElementAutoRecharge, ref mAutoRecharge, Util.kNotInvalidPredicateSingle);
				s.StreamElementOpt(mode, kXmlElementUseLimit, ref mUseLimit, Util.kNotInvalidPredicate);
				StreamXmlFlags(s, mode);
			}
			s.StreamElementOpt(mode, kXmlElementTriggerScript, ref mTriggerScript, Util.kNotNullOrEmpty);
			s.StreamElementOpt(mode, kXmlElementCommandTriggerScript, ref mCommandTriggerScript, Util.kNotNullOrEmpty);
		}
		#endregion
	};
}